using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Tast.Entities.Stock
{
	/// <summary>
	/// 股票
	/// </summary>
	public class Stock
	{
		/// <summary>
		/// 股票编号
		/// </summary>
		[BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
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