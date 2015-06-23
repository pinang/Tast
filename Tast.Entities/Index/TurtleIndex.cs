using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.ComponentModel;

namespace Tast.Entities.Index
{
	#region 指标-海龟波动性
	/// <summary>
	/// 指标-海龟波动性
	/// </summary>
	[Serializable]
	public class TurtleIndex
	{
		/// <summary>
		/// 编号
		/// </summary>
		[BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
		public string IndexId { get; set; }

		/// <summary>
		/// 趋势编号
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