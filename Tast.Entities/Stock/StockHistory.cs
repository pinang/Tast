using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Tast.Entities.Stock
{
	/// <summary>
	/// 股票历史
	/// </summary>
	public class StockHistory
	{
		/// <summary>
		/// 编号
		/// </summary>
		[BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
		public string HistoryId { get; set; }

		/// <summary>
		/// 前一天编号
		/// </summary>
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
		public string PrevDate { get; set; }

		/// <summary>
		/// 开盘价
		/// </summary>
		[BsonRepresentation(BsonType.Double)]
		public decimal Open { get; set; }

		/// <summary>
		/// 最高价
		/// </summary>
		[BsonRepresentation(BsonType.Double)]
		public decimal High { get; set; }

		/// <summary>
		/// 最低价
		/// </summary>
		[BsonRepresentation(BsonType.Double)]
		public decimal Low { get; set; }

		/// <summary>
		/// 收盘价
		/// </summary>
		[BsonRepresentation(BsonType.Double)]
		public decimal Close { get; set; }

		/// <summary>
		/// 交易量
		/// </summary>
		[BsonRepresentation(BsonType.Int64)]
		public long Volume { get; set; }
	}
}