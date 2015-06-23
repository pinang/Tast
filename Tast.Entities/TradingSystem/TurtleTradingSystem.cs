using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Tast.Entities.TradingSystem
{
	public class TurtleTradingSystem
	{
		/// <summary>
		/// 系统编号
		/// </summary>
		[BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
		public string SystemId { get; set; }

		/// <summary>
		/// 股票编号
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 起始资金
		/// </summary>
		public decimal StartAmount { get; set; }

		/// <summary>
		/// 佣金
		/// </summary>
		public decimal Commission { get; set; }

		/// <summary>
		/// 起始日期
		/// </summary>
		public string StartDate { get; set; }

		/// <summary>
		/// 终止日期
		/// </summary>
		public string EndDate { get; set; }

		/// <summary>
		/// 起始
		/// </summary>
		public TurtleTradingSystemParameter Start { get; set; }

		/// <summary>
		/// 结束
		/// </summary>
		public TurtleTradingSystemParameter End { get; set; }

		/// <summary>
		/// 当前
		/// </summary>
		public TurtleTradingSystemParameter Current { get; set; }

		/// <summary>
		/// 当前利润额
		/// </summary>
		public decimal CurrentProfit { get; set; }

		/// <summary>
		/// 当前利润率
		/// </summary>
		public decimal CurrentProfitPercent { get; set; }

		/// <summary>
		/// 最佳
		/// </summary>
		public TurtleTradingSystemParameter Best { get; set; }

		/// <summary>
		/// 最佳利润额
		/// </summary>
		public decimal BestProfit { get; set; }

		/// <summary>
		/// 最佳利润率
		/// </summary>
		public decimal BestProfitPercent { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		public bool Enable { get; set; }

		/// <summary>
		/// 总计算量
		/// </summary>
		public long TotalAmount { get; set; }

		/// <summary>
		/// 已完成计算量
		/// </summary>
		public long FinishedAmount { get; set; }

		///// <summary>
		///// 迭代
		///// </summary>
		///// <returns></returns>
		//public bool MoveNext()
		//{
		//	//	Holding
		//	CurrentHolding = CurrentHolding < StartHolding ? StartHolding : CurrentHolding + 1;
		//	if (CurrentHolding > EndHolding)
		//	{
		//		CurrentN = CurrentN < StartN ? StartN : CurrentN + 1;
		//		CurrentHolding = StartHolding;
		//	}
		//	else if (CurrentHolding < StartHolding)
		//	{
		//		CurrentHolding = StartHolding;
		//	}

		//	//	N
		//	if (CurrentN > EndN)
		//	{
		//		CurrentEnter = CurrentEnter < StartEnter ? StartEnter : CurrentEnter + 1;
		//		CurrentN = StartN;
		//		CurrentHolding = StartHolding;
		//	}
		//	else if (CurrentN < StartN)
		//	{
		//		CurrentN = StartN;
		//	}

		//	// Enter
		//	if (CurrentEnter > EndEnter)
		//	{
		//		CurrentExit = CurrentExit < StartExit ? StartExit : CurrentExit + 1;
		//		CurrentEnter = StartEnter;
		//		CurrentN = StartN;
		//		CurrentHolding = StartHolding;
		//	}
		//	else if (CurrentEnter < StartN)
		//	{
		//		CurrentEnter = StartEnter;
		//	}

		//	// Exit
		//	if (CurrentExit > EndExit)
		//	{
		//		CurrentStop = CurrentStop < StartStop ? StartStop : CurrentStop + 1;
		//		CurrentExit = StartExit;
		//		CurrentEnter = StartEnter;
		//		CurrentN = StartN;
		//		CurrentHolding = StartHolding;
		//	}
		//	else if (CurrentExit < StartEnter)
		//	{
		//		CurrentExit = StartExit;
		//	}

		//	// Stop
		//	if (CurrentStop > EndStop)
		//	{
		//		CurrentStockIndex = CurrentStockIndex < 0 ? 0 : CurrentStockIndex + 1;
		//		CurrentStop = StartStop;
		//		CurrentExit = StartExit;
		//		CurrentEnter = StartEnter;
		//		CurrentN = StartN;
		//		CurrentHolding = StartHolding;
		//	}
		//	else if (CurrentStop < StartStop)
		//	{
		//		CurrentStop = StartStop;
		//	}

		//	//	Stock
		//	if (StockNames == null || StockNames.Length == 0 || CurrentStockIndex > StockNames.Length - 1)
		//		return false;

		//	if (CurrentStockIndex < 0)
		//	{
		//		CurrentStockIndex = 0;
		//	}
		//	CurrentStockName = StockNames[CurrentStockIndex];

		//	return true;
		//}

		///// <summary>
		///// 重置
		///// </summary>
		//public void Reset()
		//{
		//	CurrentStockIndex = 0;
		//	CurrentStockName = null;
		//	CurrentHolding = 0;
		//	CurrentN = 0;
		//	CurrentEnter = 0;
		//	CurrentExit = 0;
		//	CurrentStop = 0;
		//	CurrentProfit = 0;
		//	CurrentProfitPercent = 0;
		//	BestStockName = null;
		//	BestHolding = 0;
		//	BestN = 0;
		//	BestEnter = 0;
		//	BestExit = 0;
		//	BestStop = 0;
		//	BestProfit = 0;
		//	BestProfitPercent = 0;
		//	FinishedAmount = 0;
		//	Status = TradingStatus.New;
		//}
	}
}
