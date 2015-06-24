using nZAI.Database.Attributes;

namespace Tast.Entities.TradingSystem
{
	[DbTable(TableName = "TurtleTradingSystem", PrimaryKey = "SystemId")]
	public class TurtleTradingSystem
	{
		/// <summary>
		/// 系统编号
		/// </summary>
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
		///	起始头寸
		/// </summary>
		public int StartHolding {get;set;}

		/// <summary>
		/// 起始真实波动幅度区间
		/// </summary>
		public int StartN { get; set; }

		/// <summary>
		/// 起始入市区间
		/// </summary>
		public int StartEnter { get; set; }

		/// <summary>
		/// 起始退市区间
		/// </summary>
		public int StartExit { get; set; }

		/// <summary>
		/// 起始止损区间
		/// </summary>
		public int StartStop { get; set; }

		/// <summary>
		///	结束头寸
		/// </summary>
		public int EndHolding { get; set; }

		/// <summary>
		/// 结束真实波动幅度区间
		/// </summary>
		public int EndN { get; set; }

		/// <summary>
		/// 结束入市区间
		/// </summary>
		public int EndEnter { get; set; }

		/// <summary>
		/// 结束退市区间
		/// </summary>
		public int EndExit { get; set; }

		/// <summary>
		/// 结束止损区间
		/// </summary>
		public int EndStop { get; set; }

		/// <summary>
		///	当前头寸
		/// </summary>
		public int CurrentHolding { get; set; }

		/// <summary>
		/// 当前真实波动幅度区间
		/// </summary>
		public int CurrentN { get; set; }

		/// <summary>
		/// 当前入市区间
		/// </summary>
		public int CurrentEnter { get; set; }

		/// <summary>
		/// 当前退市区间
		/// </summary>
		public int CurrentExit { get; set; }

		/// <summary>
		/// 当前止损区间
		/// </summary>
		public int CurrentStop { get; set; }

		/// <summary>
		/// 当前利润额
		/// </summary>
		public decimal CurrentProfit { get; set; }

		/// <summary>
		/// 当前利润率
		/// </summary>
		public decimal CurrentProfitPercent { get; set; }

		/// <summary>
		///	最佳头寸
		/// </summary>
		public int BestHolding { get; set; }

		/// <summary>
		/// 最佳真实波动幅度区间
		/// </summary>
		public int BestN { get; set; }

		/// <summary>
		/// 最佳入市区间
		/// </summary>
		public int BestEnter { get; set; }

		/// <summary>
		/// 最佳退市区间
		/// </summary>
		public int BestExit { get; set; }

		/// <summary>
		/// 最佳止损区间
		/// </summary>
		public int BestStop { get; set; }

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

		/// <summary>
		/// 已经运行的时间
		/// </summary>
		public long PassedSeconds { get; set; }

		/// <summary>
		/// 剩余时间提示
		/// </summary>
		public string RemainTips { get; set; }
	}
}