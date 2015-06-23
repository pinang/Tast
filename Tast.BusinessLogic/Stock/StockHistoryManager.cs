using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tast.BusinessLogic.MongoDB;
using Tast.Entities;
using Tast.Entities.Stock;

namespace Tast.BusinessLogic.Stock
{
	public class StockHistoryManager
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// 指数值
		/// </summary>
		private static Dictionary<string, StockHistory[]> AllHistories = new Dictionary<string, StockHistory[]>();

		public static Result Get(string stockName, out StockHistory[] histories)
		{
			histories = null;

			var result = Result.OK;
			try
			{
				if (!AllHistories.ContainsKey(stockName))
				{
					//	从数据库中载入指数
					result = Load(stockName);
					if (!result.Success)
						return result;
				}

				histories = AllHistories[stockName];
			}
			catch (Exception ex)
			{
				result = Result.Create(ex);
			}

			return result;
		}

		/// <summary>
		/// 载入指标
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		private static Result Load(string code)
		{
			var result = Result.OK;
			try
			{
				var db = MongoDBManager.GetDatabaseInstance();
				var collection = db.GetCollection<StockHistory>("StockHistory");
				var task = collection.Find(i => i.Code == code).SortBy(i => i.Date).ToListAsync();
				task.Wait();

				//	查询
				AllHistories[code] = task.Result.ToArray();
			}
			catch (Exception ex)
			{
				result = Result.Create(ex);
			}

			return result;
		}

		public static Result RefreshAllStockHistory()
		{
			var result = Result.OK;
			try
			{
				logger.Info("开始更新股票历史");

				var db = MongoDBManager.GetDatabaseInstance();
				var collection = db.GetCollection<Tast.Entities.Stock.Stock>("Stock");

				//	查找未更新的股票
				var today = DateTime.Today.ToString("yyyyMMdd");
				var task = collection.Find(s => s.Enable && s.LastHistoryDate != today).ToListAsync();
				task.Wait();

				var stocks = task.Result;

				var loopResult = Parallel.ForEach<Tast.Entities.Stock.Stock>(stocks, new ParallelOptions { MaxDegreeOfParallelism = 4 }, stock =>
				{
					var ret = RefreshStockHistory(stock.Code);
					if (ret.Success)
					{
						//	更新股票最后更新日期
						 collection.UpdateOneAsync(
							Builders<Tast.Entities.Stock.Stock>.Filter.Eq(s => s.Code, stock.Code),
							Builders<Tast.Entities.Stock.Stock>.Update.Set(s => s.LastHistoryDate, today)
						).Wait();
					}
					else
					{
						logger.Error(ret.Message);
					}
				});

				if (loopResult.IsCompleted)
				{
					logger.Info("股票历史更新成功");
				}
				else
				{
					logger.Info("股票历史更新失败");
				}
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
				result = Result.Create(ex);
			}

			return result;
		}

		public static Result RefreshStockHistory(string code)
		{
			var result = Result.OK;
			try
			{
				const string queryPattern = "http://www.nasdaq.com/symbol/{0}/historical";
				const string matchPattern = @"<tr>\s+<td>\s+(?<date>[\d\/]+)\s+</td>\s+<td>\s+(?<open>[\d\.]+)\s+</td>\s+<td>\s+(?<high>[\d\.]+)\s+</td>\s+<td>\s+(?<low>[\d\.]+)\s+</td>\s+<td>\s+(?<close>[\d\.]+)\s+</td>\s+<td>\s+(?<volume>[\d\.,]+)\s+</td>\s+</tr>";

				var url = string.Format(queryPattern, code.ToLower());
				var request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = "POST";
				request.ContentType = "application/json";

				using (var requestStream = request.GetRequestStream())
				{
					var payload = Encoding.UTF8.GetBytes(string.Format("10y|false|{0}", code.ToUpper()));
					requestStream.Write(payload, 0, payload.Length);

					using (var responseStream = request.GetResponse().GetResponseStream())
					{
						using (var sr = new StreamReader(responseStream))
						{
							var html = sr.ReadToEnd();
							var matches = Regex.Matches(html, matchPattern);

							var list = new List<StockHistory>();
							foreach (Match m in matches)
							{
								list.Add(new StockHistory
								{
									HistoryId = Guid.NewGuid().ToString("N"),
									Code = code.ToUpper(),
									Date = Convert.ToDateTime(m.Groups["date"].Value).ToString("yyyyMMdd"),
									Open = Convert.ToDecimal(m.Groups["open"].Value),
									High = Convert.ToDecimal(m.Groups["high"].Value),
									Low = Convert.ToDecimal(m.Groups["low"].Value),
									Close = Convert.ToDecimal(m.Groups["close"].Value),
									Volume = Convert.ToInt32(m.Groups["volume"].Value.Replace(",", "")),
								});
							}

							//	下载的数据是日期倒序排序的，需要重新排序一下
							for (var index = 0; index < list.Count - 1; index++)
							{
								list[index].PrevHistoryId = list[index + 1].HistoryId;
								list[index].PrevDate = list[index + 1].Date;
							}

							var db = MongoDBManager.GetDatabaseInstance();
							var collection = db.GetCollection<StockHistory>("StockHistory");

							//	先删除原有数据
							collection.DeleteManyAsync(Builders<StockHistory>.Filter.Eq(s => s.Code, code)).Wait();

							//	再新增新的数据
							collection.InsertManyAsync(list).Wait();
						}
					}
				}
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
				result = Result.Create(ex);
			}

			return result;
		}
	}
}