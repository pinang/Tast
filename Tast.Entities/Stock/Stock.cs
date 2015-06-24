using nZAI.Database.Attributes;

namespace Tast.Entities.Stock
{
	/// <summary>
	/// 股票
	/// </summary>
	[DbTable(TableName = "Stock", PrimaryKey = "Code")]
	public class Stock
	{
		/// <summary>
		/// 股票编号
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 股票名称
		/// </summary>
		public string ChineseName { get; set; }

		/// <summary>
		/// 最后更新日期
		/// </summary>
		public string LastHistoryDate { get; set; }

		/// <summary>
		/// 是否启用
		/// </summary>
		public bool Enable { get; set; }
	}
}