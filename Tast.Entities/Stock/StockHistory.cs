using nZAI.Database.Attributes;

namespace Tast.Entities.Stock
{
	/// <summary>
	/// 股票历史
	/// </summary>
	[DbTable(TableName = "StockHistory", PrimaryKey = "HistoryId")]
	public class StockHistory
	{
		/// <summary>
		/// 编号
		/// </summary>
		public string HistoryId { get; set; }

		/// <summary>
		/// 前一天编号
		/// </summary>
		[DbColumn(Nullable = true)]
		public string PrevHistoryId { get; set; }

		/// <summary>
		/// 股票编号
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 日期
		/// </summary>
		public string Date { get; set; }

		/// <summary>
		/// 日期
		/// </summary>
		[DbColumn(Nullable = true)]
		public string PrevDate { get; set; }

		/// <summary>
		/// 开盘价
		/// </summary>
		public decimal Open { get; set; }

		/// <summary>
		/// 最高价
		/// </summary>
		public decimal High { get; set; }

		/// <summary>
		/// 最低价
		/// </summary>
		public decimal Low { get; set; }

		/// <summary>
		/// 收盘价
		/// </summary>
		public decimal Close { get; set; }

		/// <summary>
		/// 交易量
		/// </summary>
		public long Volume { get; set; }
	}
}