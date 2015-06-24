﻿using SqlFu;

namespace Tast.Entities.TradingSystem
{
	[Table("TurtleTradingSystemTest", PrimaryKey = "TestId")]
	public class TurtleTradingSystemTest
	{
		/// <summary>
		/// 测试编号
		/// </summary>
		public string TestId { get; set; }

		/// <summary>
		/// 系统编号
		/// </summary>
		public string SystemId { get; set; }

		/// <summary>
		/// 头寸
		/// </summary>
		public int Holding { get; set; }

		/// <summary>
		/// 真实波动幅度区间
		/// </summary>
		public int N { get; set; }

		/// <summary>
		/// 入市区间
		/// </summary>
		public int Enter { get; set; }

		/// <summary>
		/// 退市区间
		/// </summary>
		public int Exit { get; set; }

		/// <summary>
		/// 止损区间
		/// </summary>
		public int Stop { get; set; }

		/// <summary>
		/// 起始资金量
		/// </summary>
		public decimal StartAmount { get; set; }

		/// <summary>
		/// 结束资金量
		/// </summary>
		public decimal EndAmount { get; set; }

		/// <summary>
		/// 佣金
		/// </summary>
		public decimal Commission { get; set; }

		/// <summary>
		/// 利润额
		/// </summary>
		public decimal Profit { get; set; }

		/// <summary>
		/// 利润率
		/// </summary>
		public decimal ProfitPercent { get; set; }
	}
}