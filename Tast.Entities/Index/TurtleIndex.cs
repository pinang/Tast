using SqlFu;
using System;

namespace Tast.Entities.Index
{
	#region 指标-海龟波动性
	/// <summary>
	/// 指标-海龟波动性
	/// </summary>
	[Table("TurtleIndex", PrimaryKey = "IndexId")]
	public class TurtleIndex
	{
		/// <summary>
		/// 编号
		/// </summary>
		public string IndexId { get; set; }

		/// <summary>
		/// 历史编号
		/// </summary>
		public string HistoryId { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 区间
		/// </summary>
		public int Peroid { get; set; }

		/// <summary>
		/// 日期
		/// </summary>
		public string Date { get; set; }

		/// <summary>
		/// 波动性均值
		/// </summary>
		public decimal N { get; set; }

		/// <summary>
		/// 真实波动性
		/// </summary>
		public decimal TR { get; set; }
	}
	#endregion
}