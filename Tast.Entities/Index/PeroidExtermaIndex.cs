using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.ComponentModel;

namespace Tast.Entities.Index
{
	#region 指标-区间极值
	/// <summary>
	/// 指标-区间极值
	/// </summary>
	[Serializable]
	public class PeroidExtermaIndex
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
		/// 最小值
		/// </summary>
		public decimal Min { get; set; }

		/// <summary>
		/// 最大值
		/// </summary>
		public decimal Max { get; set; }
	}
	#endregion
}