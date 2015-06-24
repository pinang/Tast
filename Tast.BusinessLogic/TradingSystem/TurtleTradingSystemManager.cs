﻿using NLog;
using SqlFu;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Tast.BusinessLogic.Index;
using Tast.Entities;
using Tast.Entities.Index;
using Tast.Entities.Stock;
using Tast.Entities.TradingSystem;

namespace Tast.BusinessLogic.TradingSystem
{
	/// <summary>
	/// 海龟交易系统
	/// </summary>
	public class TurtleTradingSystemManager
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		#region 模拟交易的中间状态量

		/// <summary>
		/// 当前的趋势
		/// </summary>
		private static TurtleTradingSystemTrend CurrentTrend = null;
		/// <summary>
		/// 当前的持仓
		/// </summary>
		private static List<TurtleTradingSystemHolding> CurrentHoldings = null;

		private static List<TurtleTradingSystemTrend> TestTrends = null;

		private static List<TurtleTradingSystemHolding> TestHoldings = null;
		#endregion

		/// <summary>
		/// 系统验证初始化
		/// </summary>
		public static void SystemVerificationInit()
		{
			try
			{
				logger.Info("海龟交易系统初始化开始");

				List<TurtleTradingSystem> systems = null;
				using (var db = SqlFuDao.GetConnection())
				{
					systems = db.Query<TurtleTradingSystem>(s => s.Enable).ToList();
					if (systems == null || systems.Count == 0)
					{
						//	增加默认的设置
						var defaultSystem = AddDefault(db);
						if (defaultSystem == null)
							throw new Exception("初始化海龟交易系统出错");

						systems.Add(defaultSystem);
					}

					db.Close();
				}

				System.Threading.Tasks.Task.Factory.StartNew(() =>
				{
					foreach (var system in systems)
					{
						//	海龟交易系统验证
						SystemVerification(system);
					}
				});

				logger.Info("海龟交易系统初始化结束");
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
			}
		}

		/// <summary>
		/// 增加默认的海龟交易系统设置
		/// </summary>
		/// <param name="db"></param>
		/// <returns></returns>
		private static TurtleTradingSystem AddDefault(DbConnection db)
		{
			TurtleTradingSystem result = null;
			try
			{
				result = new TurtleTradingSystem
				{
					SystemId = Guid.NewGuid().ToString("N"),
					Code = "AAPL",
					StartAmount = 100000m,
					Commission = 7,
					StartDate = "20120101",
					EndDate = "20141231",
					StartHolding = 2,
					StartN = 2,
					StartEnter = 2,
					StartExit = 2,
					StartStop = 2,
					EndHolding = 20,
					EndN = 50,
					EndEnter = 50,
					EndExit = 50,
					EndStop = 20,
					CurrentHolding = 2,
					CurrentN = 2,
					CurrentEnter = 2,
					CurrentExit = 2,
					CurrentStop = 0,
					CurrentProfit = 0m,
					CurrentProfitPercent = 0m,
					BestHolding = 2,
					BestN = 2,
					BestEnter = 2,
					BestExit = 2,
					BestStop = 2,
					BestProfit = decimal.MinValue,
					BestProfitPercent = 0m,
					Enable = true,
					TotalAmount = 0,
					FinishedAmount = 0,
					RemainTips = string.Empty
				};

				//	获取这一区间段的股票历史记录数
				var historyCount = db.WithSql("SELECT COUNT(0) FROM StockHistory WHERE Code = @0 AND Date >= @1 AND Date <= @2", 
					result.Code, 
					result.StartDate, 
					result.EndDate)
					.QuerySingle<int>();

				result.TotalAmount = (result.EndHolding - result.StartHolding + 1) *
					(result.EndN - result.StartN + 1) *
					(result.EndEnter - result.StartEnter + 1) *
					(result.EndStop - result.StartStop + 1) *
					historyCount;

				//	保存
				db.Insert<TurtleTradingSystem>(result);
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
				result = null;
			}

			return result;
		}

		/// <summary>
		/// 海龟交易系统验证
		/// </summary>
		/// <param name="system"></param>
		private static void SystemVerification(TurtleTradingSystem system)
		{
			try
			{
				TestTrends = new List<TurtleTradingSystemTrend>();
				TestHoldings = new List<TurtleTradingSystemHolding>();

				//var collectionSystem = db.GetCollection<TurtleTradingSystem>("TurtleTradingSystem");
				//var collectionTest = db.GetCollection<TurtleTradingSystemTest>("TurtleTradingSystemTest");
				//var collectionTrend = db.GetCollection<TurtleTradingSystemTrend>("TurtleTradingSystemTrend");
				//var collectionHolding = db.GetCollection<TurtleTradingSystemHolding>("TurtleTradingSystemHolding");

				using (var db = SqlFuDao.GetConnection())
				{
					var histories = db.Query<StockHistory>(h => h.Code == system.Code && h.Date.CompareTo(system.StartDate) >= 0 && h.Date.CompareTo(system.EndDate) <= 0).ToList();
					if (histories.Count == 0)
						throw new Exception("没有符合查询条件的历史股价");

					Dictionary<int, Dictionary<string, PeroidExtermaIndex>> dictPeroidExtermaIndex;
					var result = PreparePeroidExtermaIndex(system, histories[0].PrevDate, histories[histories.Count - 1].Date, db, out dictPeroidExtermaIndex);
					if (!result.Success)
						throw new Exception("查询区间极值指标出错");

					Dictionary<int, Dictionary<string, TurtleIndex>> dictTurtleIndex;
					result = PrepareTurtleIndex(system, histories[0].PrevDate, histories[histories.Count - 1].Date, db, out dictTurtleIndex);
					if (!result.Success)
						throw new Exception("查询海龟指标出错");

					var update = false;
					var sw = new Stopwatch();
					sw.Start();
					while (Next(system))
					{
						var test = new TurtleTradingSystemTest
						{
							TestId = Guid.NewGuid().ToString("N"),
							SystemId = system.SystemId,
							Holding = system.CurrentHolding,
							N = system.CurrentN,
							Enter = system.CurrentEnter,
							Exit = system.CurrentExit,
							Stop = system.CurrentStop,
							StartAmount = system.StartAmount,
							EndAmount = system.StartAmount,
							Commission = system.Commission,
							Profit = 0m,
							ProfitPercent = 0m
						};

						var ti = dictTurtleIndex[test.N];
						var pein = dictPeroidExtermaIndex[test.Enter];
						var peix = dictPeroidExtermaIndex[test.Exit];

						//	清空状态量
						CurrentTrend = null;
						CurrentHoldings = new List<TurtleTradingSystemHolding>();

						//	循环日期演算结果
						result = Result.OK;
						foreach (var history in histories)
						{
							////	获取前一天设定下的指标
							//result = TurtleIndexManager.Get(system.Code, test.N, history.PrevDate, out ti);
							//if (!result.Success)
							//	throw new Exception("获取TurtleIndex出错, peroid:" + test.N + " date:" + history.Date);

							//result = PeroidExtermaIndexManager.Get(system.Code, test.Enter, history.PrevDate, out peiEnter);
							//if (!result.Success)
							//	throw new Exception("获取PeroidExtermaIndex出错, peroid:" + test.Enter + " date:" + history.Date);

							//result = PeroidExtermaIndexManager.Get(system.Code, test.Exit, history.PrevDate, out peiExit);
							//if (!result.Success)
							//	throw new Exception("获取PeroidExtermaIndex出错, peroid:" + test.Exit + " date:" + history.Date);

							if (!ti.ContainsKey(history.PrevDate))
								throw new Exception("ti:" + history.PrevDate);

							if (!pein.ContainsKey(history.PrevDate))
								throw new Exception("pein:" + history.PrevDate);

							if (!peix.ContainsKey(history.PrevDate))
								throw new Exception("peix:" + history.PrevDate);

							//	模拟
							result = SimulateDay(test,
								history,
								ti[history.PrevDate],
								pein[history.PrevDate],
								peix[history.PrevDate]);
							if (!result.Success)
								throw new Exception(string.Format("海龟交易系统模拟失败, Code:{0} Date:{1} Holding:{2} N:{3} Enter:{4} Exit:{5} Stop:{6}。错误:", system.Code, history.Date, test.Holding, test.N, test.Enter, test.Exit, test.Stop));
						}

						//	如果演算结束时趋势还未结束，则强行终止
						if (CurrentTrend != null)
						{
							var lastHistory = histories[histories.Count - 1];

							var reason = string.Format("终止: 触及验算结束日期{0},卖出价格为收盘价{1}", lastHistory.Date, lastHistory.Close);

							//	清仓
							result = SimulateExit(test, lastHistory.Date, lastHistory.Close, reason);
							if (!result.Success)
								throw new Exception("演算结束终止交易时发生错误:" + result.Message);
						}

						//	计算利润
						test.Profit = test.EndAmount - test.StartAmount;
						test.ProfitPercent = test.StartAmount == 0 ? 0m : test.Profit / test.StartAmount;
						system.CurrentProfit = test.Profit;
						system.CurrentProfitPercent = test.ProfitPercent;
						system.FinishedAmount++;

						update = false;
						var tasks = new List<System.Threading.Tasks.Task>();

						//	判断是否达成利润率新高
						if (test.Profit > system.BestProfit)
						{
							system.BestHolding = system.CurrentHolding;
							system.BestN = system.CurrentN;
							system.BestEnter = system.CurrentEnter;
							system.BestExit = system.CurrentExit;
							system.BestStop = system.CurrentStop;
							system.BestProfit = system.CurrentProfit;
							system.BestProfitPercent = system.CurrentProfitPercent;

							logger.Info("海龟交易系统演算获得了更高的利润率{0:P2}, Holding:{1} N:{2} Enter:{3} Exit:{4} Stop:{5}",
								system.BestProfitPercent,
								system.BestHolding,
								system.BestN,
								system.BestEnter,
								system.BestExit,
								system.BestStop);

							
							//	只有突破原有记录时才记录明细
							db.Insert<TurtleTradingSystemTest>(test);

							foreach (var trend in TestTrends)
							{
								db.Insert<TurtleTradingSystemTrend>(trend);
							}

							foreach (var holding in TestHoldings)
							{
								db.Insert<TurtleTradingSystemHolding>(holding);
							}

							update = true;
						}

						if (system.FinishedAmount % 1024 == 0)
						{
							var elapsedSeconds = sw.Elapsed.TotalSeconds;
							var totalSeconds = elapsedSeconds * ((double)system.TotalAmount / system.FinishedAmount);
							var remainSeconds = totalSeconds - elapsedSeconds;
							var ts = TimeSpan.FromSeconds(remainSeconds);

							system.RemainTips = string.Format("剩余{0}天{1}小时{2}分钟{3}秒", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);

							update = true;
						}

						if (update)
						{
							//	保存演算结果
							db.Update<TurtleTradingSystem>(system, system.SystemId);
						}

						if (tasks.Count > 0)
						{
							System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
						}

						TestTrends.Clear();
						TestHoldings.Clear();
					}

					db.Close();
				}

				logger.Info("演算结束");
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
			}
		}

		/// <summary>
		/// 准备区间极值指标
		/// </summary>
		/// <param name="system"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="db"></param>
		/// <param name="dict"></param>
		/// <returns></returns>
		private static Result PreparePeroidExtermaIndex(TurtleTradingSystem system, string startDate, string endDate, DbConnection db, out Dictionary<int, Dictionary<string, PeroidExtermaIndex>> dict)
		{
			dict = null;
			var result = Result.OK;
			try
			{
				var min = Math.Min(system.StartEnter, system.StartExit);
				var max = Math.Max(system.EndEnter, system.EndExit);

				PeroidExtermaIndex pei;
				for (var index = min; index <= max; index++)
				{
					PeroidExtermaIndexManager.Get(system.Code, index, startDate, out pei);
				}

				var indexes = db.WithSql("SELECT * FROM PeroidExtermaIndex WHERE Code = @0 AND Date >= @1 AND Date <= @2 AND Peroid >= @3 AND Peroid <= @4",
					system.Code,
					startDate,
					endDate,
					system.StartN,
					system.EndN)
					.Query<PeroidExtermaIndex>().ToList();

				dict = new Dictionary<int, Dictionary<string, PeroidExtermaIndex>>();
				for (var index = min; index <= max; index++)
				{
					dict[index] = indexes.Where(i => i.Peroid == index).ToDictionary(i => i.Date);
				}
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
				result = Result.Create(ex);
			}

			return result;
		}

		/// <summary>
		/// 准备海龟指标
		/// </summary>
		/// <param name="system"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="db"></param>
		/// <param name="dict"></param>
		/// <returns></returns>
		private static Result PrepareTurtleIndex(TurtleTradingSystem system, string startDate, string endDate, DbConnection db, out Dictionary<int, Dictionary<string, TurtleIndex>> dict)
		{
			dict = null;
			var result = Result.OK;
			try
			{
				logger.Info("start:{0} end:{1} min:{2} max:{3}", startDate, endDate, system.StartN, system.EndN);

				TurtleIndex ti;
				for (var index = system.StartN; index <= system.EndN; index++)
				{
					result = TurtleIndexManager.Get(system.Code, index, startDate, out ti);
					if (!result.Success) return result;
				}

				var indexes = db.WithSql("SELECT * FROM TurtleIndex WHERE Code = @0 AND Date >= @1 AND Date <= @2 AND Peroid >= @3 AND Peroid <= @4",
					system.Code,
					startDate,
					endDate,
					system.StartN,
					system.EndN)
					.Query<TurtleIndex>().ToList();

				if (!indexes.Exists(i => i.Date == startDate && i.Peroid == 2))
					throw new Exception("not exists");

				dict = new Dictionary<int, Dictionary<string, TurtleIndex>>();
				for (var index = system.StartN; index <= system.EndN; index++)
				{
					dict[index] = indexes.Where(i => i.Peroid == index).ToDictionary(i => i.Date);
				}
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
				result = Result.Create(ex);
			}

			return result;
		}

		/// <summary>
		/// 迭代
		/// </summary>
		/// <param name="system"></param>
		/// <returns></returns>
		private static bool Next(TurtleTradingSystem system)
		{
			//	是否进位
			bool carry;
			system.CurrentStop = Increase(system.CurrentStop, system.StartStop, system.EndStop, out carry);
			if (!carry) return true;

			system.CurrentN = Increase(system.CurrentN, system.StartN, system.EndN, out carry);
			if (!carry) return true;

			system.CurrentHolding = Increase(system.CurrentHolding, system.StartHolding, system.EndHolding, out carry);
			if (!carry) return true;

			system.CurrentExit = Increase(system.CurrentExit, system.StartExit, system.EndExit, out carry);
			if (!carry) return true;

			system.CurrentEnter = Increase(system.CurrentEnter, system.StartEnter, system.EndEnter, out carry);
			if (!carry) return true;

			return false;
		}

		/// <summary>
		/// 进位
		/// </summary>
		/// <param name="current"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="carry"></param>
		/// <returns></returns>
		private static int Increase(int current, int min, int max, out bool carry)
		{
			carry = false;
			current++;

			if (current < min)
				current = min;

			if (current > max)
			{
				current = min;
				carry = true;
			}

			return current;
		}

		/// <summary>
		/// 演算
		/// </summary>
		/// <param name="test"></param>
		/// <param name="date"></param>
		/// <param name="history"></param>
		/// <param name="ti"></param>
		/// <param name="peiEnter"></param>
		/// <param name="peiExit"></param>
		/// <param name="peiStop"></param>
		/// <returns></returns>
		private static Result SimulateDay(TurtleTradingSystemTest test, StockHistory history, TurtleIndex ti, PeroidExtermaIndex peiEnter, PeroidExtermaIndex peiExit)
		{
			const int separeteCount = 10;
			var result = Result.OK;
			try
			{
				decimal startPrice = history.Low, endPrice = history.High, increasement = (history.High - history.Low) / separeteCount;
				if (history.Open > history.Close)
				{
					startPrice = history.High;
					endPrice = history.Low;
					increasement = -increasement;
				}

				for (var price = startPrice; price <= endPrice; price += increasement)
				{
					result = SimulateSection(test, history.Date, price, ti, peiEnter, peiExit);
					if (!result.Success) return result;
				}
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
				result = Result.Create(ex);
			}

			return result;
		}

		/// <summary>
		/// 分时演算
		/// </summary>
		/// <param name="test"></param>
		/// <param name="date"></param>
		/// <param name="price"></param>
		/// <param name="ti"></param>
		/// <param name="peiEnter"></param>
		/// <param name="peiExit"></param>
		/// <returns></returns>
		private static Result SimulateSection(TurtleTradingSystemTest test, string date, decimal price, TurtleIndex ti, PeroidExtermaIndex peiEnter, PeroidExtermaIndex peiExit)
		{
			var result = Result.OK;
			try
			{
				if (CurrentTrend == null)
				{
					#region 尚未有趋势的时候等待时机启动一波新趋势
					if (price > peiEnter.Max || price < peiEnter.Min)
					{
						//	启动新的一波趋势
						//	方向: true-多 false-空
						var direcion = price > peiEnter.Max;

						CurrentTrend = new TurtleTradingSystemTrend
						{
							TrendId = Guid.NewGuid().ToString("N"),
							SystemId = test.SystemId,
							TestId = test.TestId,
							Direction = direcion,
							StartDate = date,
							StartPrice = price,
							StartReason = string.Format("入市:交易价{0}突破{1}天{2}{3}", price, peiEnter.Peroid, direcion ? "最高价" : "最低价", direcion ? peiEnter.Max : peiEnter.Min),
							MaxHolding = 0,
							Profit = 0m
						};

						//	每个头寸的金额
						var holdingAmount = test.EndAmount / test.Holding;

						//	第一个头寸
						CurrentHoldings.Add(new TurtleTradingSystemHolding
						{
							HoldingId = Guid.NewGuid().ToString("N"),
							SystemId = test.SystemId,
							TestId = test.TestId,
							TrendId = CurrentTrend.TrendId,
							Direction = CurrentTrend.Direction,
							StartDate = date,
							StartPrice = price,
							EndPrice = price + (direcion ? -1 : 1) * ti.N * test.Stop,
							Quantity = (int)(holdingAmount / price)
						});
					}
					#endregion
				}
				else
				{
					#region 在趋势中等待时机增持和退场
					var lastHolding = CurrentHoldings[CurrentHoldings.Count - 1];

					#region 增持
					if (CurrentHoldings.Count < test.Holding &&
						((CurrentTrend.Direction && price >= (lastHolding.StartPrice + ti.N * 0.5m)) ||
						(!CurrentTrend.Direction && price <= (lastHolding.StartPrice - ti.N * 0.5m))))
					{
						//	每个头寸的金额
						var holdingAmount = test.EndAmount / test.Holding;

						CurrentHoldings.Add(new TurtleTradingSystemHolding
						{
							HoldingId = Guid.NewGuid().ToString("N"),
							SystemId = test.SystemId,
							TestId = test.TestId,
							TrendId = CurrentTrend.TrendId,
							Direction = CurrentTrend.Direction,
							StartDate = date,
							StartPrice = price,
							EndPrice = price + (CurrentTrend.Direction ? -1 : 1) * ti.N * test.Stop,
							Quantity = (int)(holdingAmount / price)
						});

						return result;
					};
					#endregion

					#region 止损
					if ((CurrentTrend.Direction && price <= lastHolding.EndPrice) ||
						(!CurrentTrend.Direction && price >= lastHolding.EndPrice))
					{
						var reason = string.Format("止损: 交易价{0}触及止损价{1}", price, lastHolding.EndPrice);

						//	清仓
						return SimulateExit(test, date, price, reason);
					}

					#endregion

					#region 止盈
					if ((CurrentTrend.Direction && price < peiExit.Min) || (!CurrentTrend.Direction && price > peiExit.Max))
					{
						var reason = string.Format("止盈: 交易价{0}跌破{1}天{2}{3}",
							price,
							peiExit.Peroid,
							CurrentTrend.Direction ? "最低价" : "最高价",
							CurrentTrend.Direction ? peiExit.Min : peiExit.Max);

						//	清仓
						return SimulateExit(test, date, price, reason);
					}
					#endregion

					#endregion
				}
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
				result = Result.Create(ex);
			}

			return result;
		}

		/// <summary>
		/// 清仓
		/// </summary>
		/// <param name="test"></param>
		/// <param name="date"></param>
		/// <param name="price"></param>
		/// <param name="reason"></param>
		/// <returns></returns>
		private static Result SimulateExit(TurtleTradingSystemTest test, string date, decimal price, string reason)
		{
			var result = Result.OK;
			try
			{
				//	所有持仓退出
				CurrentHoldings.ForEach(holding =>
				{
					holding.EndDate = date;
					holding.EndPrice = price;
					holding.Profit = CurrentTrend.Direction ? (holding.EndPrice - holding.StartPrice) * holding.Quantity : (holding.StartPrice - holding.EndPrice) * holding.Quantity;
				});

				//	趋势结束
				CurrentTrend.EndReason = reason;
				CurrentTrend.EndDate = date;
				CurrentTrend.EndPrice = price;
				CurrentTrend.MaxHolding = CurrentHoldings.Count;
				CurrentTrend.Profit = CurrentHoldings.Sum(holding => holding.Profit);

				//	资金变化 = 原始资金 + 利润 - 手续费
				test.EndAmount += CurrentTrend.Profit - CurrentHoldings.Count * test.Commission * 2;

				TestTrends.Add(CurrentTrend);
				TestHoldings.AddRange(CurrentHoldings);

				CurrentTrend = null;
				CurrentHoldings.Clear();
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