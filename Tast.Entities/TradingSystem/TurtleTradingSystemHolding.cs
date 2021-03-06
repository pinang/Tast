﻿using nZAI.Database.Attributes;

namespace Tast.Entities.TradingSystem
{
	/// <summary>
	/// 海龟交易系统头寸
	/// </summary>
	[DbTable(TableName = "TurtleTradingSystemHolding", PrimaryKey = "HoldingId")]
	public class TurtleTradingSystemHolding
	{
		/// <summary>
		/// 头寸编号
		/// </summary>
		public string HoldingId { get; set; }

		/// <summary>
		/// 系统编号
		/// </summary>
		public string SystemId { get; set; }

		/// <summary>
		/// 测试编号
		/// </summary>
		public string TestId { get; set; }

		/// <summary>
		/// 趋势编号
		/// </summary>
		public string TrendId { get; set; }

		/// <summary>
		/// 方向: 0-多 1-空
		/// </summary>
		public bool Direction { get; set; }

		/// <summary>
		/// 开始日期
		/// </summary>
		public string StartDate { get; set; }

		/// <summary>
		/// 开始价格
		/// </summary>
		public decimal StartPrice { get; set; }

		/// <summary>
		/// 结束日期
		/// </summary>
		public string EndDate { get; set; }

		/// <summary>
		/// 结束价格
		/// </summary>
		public decimal EndPrice { get; set; }

		/// <summary>
		/// 数量
		/// </summary>
		public int Quantity { get; set; }

		/// <summary>
		/// 利润
		/// </summary>
		public decimal Profit { get; set; }
	}
}