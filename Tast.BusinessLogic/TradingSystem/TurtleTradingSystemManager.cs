using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tast.BusinessLogic.Index;
using Tast.BusinessLogic.MongoDB;
using Tast.Entities;
using Tast.Entities.Index;
using Tast.Entities.Stock;
using Tast.Entities.TradingSystem;
using System.Linq;

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

				var db = MongoDBManager.GetDatabaseInstance();
				var collection = db.GetCollection<TurtleTradingSystem>("TurtleTradingSystem");

				//	获取所有的海龟交易系统
				var task = collection.Find(s => s.Enable).ToListAsync();
				task.Wait();

				var systems = task.Result;
				if (systems.Count == 0)
				{
					//	增加默认的设置
					var defaultSystem = AddDefault(db);
					if (defaultSystem == null)
						throw new Exception("初始化海龟交易系统出错");

					systems.Add(defaultSystem);
				}

				//	并行
				Parallel.ForEach(systems, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (system) =>
				{
					//	海龟交易系统验证
					SystemVerification(system, db);
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
		private static TurtleTradingSystem AddDefault(IMongoDatabase db)
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
					StartDate = "20060101",
					EndDate = "20141231",
					Start = new TurtleTradingSystemParameter { Holding = 2, N = 2, Enter = 2, Exit = 2, Stop = 2 },
					End = new TurtleTradingSystemParameter { Holding = 10, N = 50, Enter = 50, Exit = 50, Stop = 50 },
					Current = new TurtleTradingSystemParameter { Holding = 2, N = 2, Enter = 2, Exit = 2, Stop = 0 },
					CurrentProfit = 0m,
					CurrentProfitPercent = 0m,
					Best = new TurtleTradingSystemParameter { Holding = 2, N = 2, Enter = 2, Exit = 2, Stop = 2 },
					BestProfit = 0m,
					BestProfitPercent = 0m,
					Enable = true,
					TotalAmount = 0,
					FinishedAmount = 0
				};

				//	获取这一区间段的股票历史
				var historyCollection = db.GetCollection<StockHistory>("StockHistory");
				var historyTask = historyCollection.CountAsync(Builders<StockHistory>.Filter.And(
					Builders<StockHistory>.Filter.Eq(h => h.Code, result.Code),
					Builders<StockHistory>.Filter.Gte(h => h.Date, result.StartDate),
					Builders<StockHistory>.Filter.Lte(h => h.Date, result.EndDate)));

				historyTask.Wait();
				var historyCount = historyTask.Result;

				result.TotalAmount = (result.End.Holding - result.Start.Holding + 1) *
					(result.End.N - result.Start.N + 1) *
					(result.End.Enter - result.Start.Enter + 1) *
					(result.End.Stop - result.Start.Stop + 1) *
					historyCount;

				//	保存
				var collection = db.GetCollection<TurtleTradingSystem>("TurtleTradingSystem");
				collection.InsertOneAsync(result).Wait();
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
		private static void SystemVerification(TurtleTradingSystem system, IMongoDatabase db)
		{
			try
			{
				//	获取这一区间段的股票历史
				var historyCollection = db.GetCollection<StockHistory>("StockHistory");
				var historyTask = historyCollection.Find(Builders<StockHistory>.Filter.And(
					Builders<StockHistory>.Filter.Eq(h => h.Code, system.Code),
					Builders<StockHistory>.Filter.Gte(h => h.Date, system.StartDate),
					Builders<StockHistory>.Filter.Lte(h => h.Date, system.EndDate)))
					.SortBy(s => s.Date).ToListAsync();

				historyTask.Wait();
				var histories = historyTask.Result;

				TestTrends = new List<TurtleTradingSystemTrend>();
				TestHoldings = new List<TurtleTradingSystemHolding>();

				while (Next(system))
				{
					var test = new TurtleTradingSystemTest
					{
						TestId = Guid.NewGuid().ToString("N"),
						SystemId = system.SystemId,
						Holding = system.Current.Holding,
						N = system.Current.N,
						Enter = system.Current.Enter,
						Exit = system.Current.Exit,
						Stop = system.Current.Stop,
						StartAmount = system.StartAmount,
						EndAmount = system.StartAmount,
						Commission = system.Commission,
						Profit = 0m,
						ProfitPercent = 0m
					};

					PeroidExtermaIndex peiEnter, peiExit;
					TurtleIndex ti;

					//	循环日期演算结果
					var result = Result.OK;
					foreach (var history in histories)
					{
						//	获取前一天设定下的指标
						result = TurtleIndexManager.Get(system.Code, test.N, history.PrevDate, out ti);
						if (!result.Success)
							throw new Exception("获取TurtleIndex出错, peroid:" + test.N + " date:" + history.Date);

						result = PeroidExtermaIndexManager.Get(system.Code, test.Enter, history.PrevDate, out peiEnter);
						if (!result.Success)
							throw new Exception("获取PeroidExtermaIndex出错, peroid:" + test.Enter + " date:" + history.Date);

						result = PeroidExtermaIndexManager.Get(system.Code, test.Exit, history.PrevDate, out peiExit);
						if (!result.Success)
							throw new Exception("获取PeroidExtermaIndex出错, peroid:" + test.Exit + " date:" + history.Date);

						//	清空状态量
						CurrentTrend = null;
						CurrentHoldings = new List<TurtleTradingSystemHolding>();

						//	模拟
						result = SimulateDay(test, history, ti, peiEnter, peiExit);
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

					//	判断是否达成利润率新高
					if (test.Profit > system.BestProfit)
					{
						system.Best = system.Current;
						system.BestProfit = system.CurrentProfit;
						system.BestProfitPercent = system.CurrentProfitPercent;

						logger.Info("海龟交易系统演算获得了更高的利润率{0:P2}, TestId:{1}", system.BestProfitPercent, test.TestId);
					}

					//	保存演算结果
					var tasks = new List<System.Threading.Tasks.Task>();

					var collectionSystem = db.GetCollection<TurtleTradingSystem>("TurtleTradingSystem");
					var collectionTest = db.GetCollection<TurtleTradingSystemTest>("TurtleTradingSystemTest");
					var collectionTrend = db.GetCollection<TurtleTradingSystemTrend>("TurtleTradingSystemTrend");
					var collectionHolding = db.GetCollection<TurtleTradingSystemHolding>("TurtleTradingSystemHolding");

					logger.Info("System:{0} Test:{1} Trend:{2} Holding:{3}", system != null, test != null, TestTrends.Count, TestHoldings.Count);

					tasks.Add(collectionSystem.ReplaceOneAsync(Builders<TurtleTradingSystem>.Filter.Where(s => s.SystemId == system.SystemId), system));
					tasks.Add(collectionTest.InsertOneAsync(test));
					tasks.Add(collectionTrend.InsertManyAsync(TestTrends));
					tasks.Add(collectionHolding.InsertManyAsync(TestHoldings));

					System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

					TestTrends.Clear();
					TestHoldings.Clear();

					break;
				}
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
			}
		}

		/// <summary>
		/// 迭代
		/// </summary>
		/// <param name="system"></param>
		/// <returns></returns>
		private static bool Next(TurtleTradingSystem system)
		{
			if (system.Start == null || system.End == null || system.Current == null)
				return false;

			//	是否进位
			bool carry;
			system.Current.Stop = Increase(system.Current.Stop, system.Start.Stop, system.End.Stop, out carry);
			if (!carry) return true;

			system.Current.Exit = Increase(system.Current.Exit, system.Start.Exit, system.End.Exit, out carry);
			if (!carry) return true;

			system.Current.Enter = Increase(system.Current.Enter, system.Start.Enter, system.End.Enter, out carry);
			if (!carry) return true;

			system.Current.N = Increase(system.Current.N, system.Start.N, system.End.N, out carry);
			if (!carry) return true;

			system.Current.Holding = Increase(system.Current.Holding, system.Start.Holding, system.End.Holding, out carry);
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
			const int separeteCount = 100;
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
				//	每个头寸的金额
				var holdingAmount = test.EndAmount / test.Holding;

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
						((CurrentTrend.Direction && price >= (lastHolding.StartPrice + ti.N / 2)) ||
						(!CurrentTrend.Direction && price <= (lastHolding.StartPrice - ti.N / 2))))
					{
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