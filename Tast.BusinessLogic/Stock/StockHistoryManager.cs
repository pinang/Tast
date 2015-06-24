using MySql.Data.MySqlClient;
using NLog;
using nZAI.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
				AllHistories[code] = DBO.Query<StockHistory>("SELECT * FROM StockHistory WHERE Code = @Code ORDER BY Date", new MySqlParameter("@Code", code)).ToArray();
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

				List<Tast.Entities.Stock.Stock> stocks;
				using (var conn = DBO.GetConnection())
				{
					conn.Open();

					var today = DateTime.Today.ToString("yyyyMMdd");
					stocks = DBO.Query<Tast.Entities.Stock.Stock>("SELECT * FROM Stock WHERE Enable = 1 AND LastHistoryDate <> @Date ORDER BY Code", conn, new MySqlParameter("@Date", today));

					var loopResult = Parallel.ForEach<Tast.Entities.Stock.Stock>(stocks, new ParallelOptions { MaxDegreeOfParallelism = 4 }, stock =>
					{
						var ret = RefreshStockHistory(stock.Code);
						if (ret.Success)
						{
							//	更新股票最后更新日期
							stock.LastHistoryDate = today;
							if (!DBO.Update<Tast.Entities.Stock.Stock>(stock))
								throw new Exception("更新最后更新时间时发生错误");
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

					conn.Close();
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
			MySqlTransaction dbts = null;
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

							using (var conn = DBO.GetConnection())
							{
								conn.Open();

								//	启动事务
								using (dbts = conn.BeginTransaction())
								{
									//	先删除原有记录
									DBO.ExecuteNonQuery("DELETE FROM StockHistory WHERE Code = @Code", conn, new MySqlParameter("@Code", code));

									//	新增新记录
									if (!DBO.BatchInsert<StockHistory>(list, conn))
										throw new Exception("新增记录时发生错误");

									//	提交
									dbts.Commit();
								}

								conn.Close();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
				result = Result.Create(ex);
				if (dbts != null)
					dbts.Rollback();
			}

			return result;
		}
	}
}