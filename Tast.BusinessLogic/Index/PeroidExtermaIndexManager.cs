using MySql.Data.MySqlClient;
using NLog;
using nZAI.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Tast.BusinessLogic.Stock;
using Tast.Entities;
using Tast.Entities.Index;
using Tast.Entities.Stock;

namespace Tast.BusinessLogic.Index
{
	public class PeroidExtermaIndexManager
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// 指数值
		/// </summary>
		private static Dictionary<string, Dictionary<string, PeroidExtermaIndex>> AllIndexs = new Dictionary<string, Dictionary<string, PeroidExtermaIndex>>();

		/// <summary>
		/// 获取指标
		/// </summary>
		/// <param name="code"></param>
		/// <param name="peroid"></param>
		/// <param name="date"></param>
		/// <param name="ipe"></param>
		/// <returns></returns>
		public static Result Get(string code, int peroid, string date, out PeroidExtermaIndex ipe)
		{
			ipe = null;

			var key = code + peroid.ToString();
			var result = Result.OK;
			try
			{
				if (!AllIndexs.ContainsKey(key))
				{
					//	从数据库中载入指数
					result = Load(code, peroid);
					if (!result.Success)
						return result;
				}

				if (!AllIndexs[key].ContainsKey(date))
				{
					//	刷新指数
					result = Refresh(code, peroid);
					if (!result.Success)
						return result;

					if (!AllIndexs[key].ContainsKey(date))
						return Result.RecordNotFound;
				}

				ipe = AllIndexs[key][date];
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
		/// <param name="peroid"></param>
		/// <returns></returns>
		private static Result Load(string code, int peroid)
		{
			var result = Result.OK;
			try
			{
				//	查询
				var key = code + peroid.ToString();

				AllIndexs[key] = DBO.Query<PeroidExtermaIndex>("SELECT * FROM PeroidExtermaIndex WHERE Code = @Code AND Peroid = @Peroid",
					new MySqlParameter("@Code", code),
					new MySqlParameter("@Peroid", peroid))
					.ToDictionary(i => i.Date);
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
				result = Result.Create(ex);
			}

			return result;
		}

		/// <summary>
		/// 保存指标
		/// </summary>
		/// <param name="code"></param>
		/// <param name="peroid"></param>
		/// <returns></returns>
		private static Result Save(string code, int peroid)
		{
			MySqlTransaction dbts = null;
			var result = Result.OK;
			try
			{
				var key = code + peroid.ToString();
				var list = AllIndexs[key].Values.ToArray();

				using (var conn = DBO.GetConnection())
				{
					conn.Open();

					//	启动事务
					using (dbts = conn.BeginTransaction())
					{
						//	先删除原有记录
						DBO.ExecuteNonQuery("DELETE FROM PeroidExtermaIndex WHERE Code = @Code AND Peroid = @Peroid", conn,
							new MySqlParameter("@Code", code),
							new MySqlParameter("@Peroid", peroid));

						//	新增新记录
						if (!DBO.BatchInsert<PeroidExtermaIndex>(list, conn))
							throw new Exception("新增记录时发生错误");

						//	提交
						dbts.Commit();
					}

					conn.Close();
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

		/// <summary>
		/// 刷新指标
		/// </summary>
		/// <param name="code"></param>
		/// <param name="peroid"></param>
		/// <returns></returns>
		private static Result Refresh(string code, int peroid)
		{
			var result = Result.OK;
			try
			{
				//	读取股票历史
				StockHistory[] histories;
				result = StockHistoryManager.Get(code, out histories);
				if (!result.Success) return result;

				//	计算
				PeroidExtermaIndex[] indexes;
				result = Calculate(histories, peroid, out indexes);
				if (!result.Success) return result;

				//	更新指数
				var key = code + peroid.ToString();
				AllIndexs[key] = indexes.ToDictionary(i => i.Date);

				//	保存
				result = Save(code, peroid);
				if (!result.Success) return result;
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
				result = Result.Create(ex);
			}

			return result;
		}

		/// <summary>
		/// 计算指标
		/// </summary>
		/// <param name="histories"></param>
		/// <param name="peroid"></param>
		/// <param name="indexes"></param>
		/// <returns></returns>
		private static Result Calculate(StockHistory[] histories, int peroid, out PeroidExtermaIndex[] indexes)
		{
			indexes = null;

			//	阶段内的价格历史
			var peroidHistory = new Queue<StockHistory>();

			//	前一天的N
			var list = new List<PeroidExtermaIndex>();
			for (var index = 0; index < histories.Length; index++)
			{
				if (index >= peroid)
				{
					//	移除最早的
					peroidHistory.Dequeue();
				}
				//	加入当前
				peroidHistory.Enqueue(histories[index]);

				list.Add(new PeroidExtermaIndex
				{

					IndexId = Guid.NewGuid().ToString("N"),
					HistoryId = histories[index].HistoryId,
					Code = histories[index].Code,
					Peroid = peroid,
					Date = histories[index].Date,
					Min = peroidHistory.Min(hp => hp.Low),
					Max = peroidHistory.Max(hp => hp.High)
				});
			}

			indexes = list.ToArray();

			return Result.OK;
		}
	}
}