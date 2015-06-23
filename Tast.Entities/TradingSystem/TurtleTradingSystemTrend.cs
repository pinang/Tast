using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Tast.Entities.TradingSystem
{
	/// <summary>
	/// 海龟交易系统趋势
	/// </summary>
	public class TurtleTradingSystemTrend
	{
		/// <summary>
		/// 系统编号
		/// </summary>
		[BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
		public string TrendId { get; set; }

		/// <summary>
		/// 系统编号
		/// </summary>
		public string SystemId { get; set; }

		/// <summary>
		/// 测试编号
		/// </summary>
		public string TestId { get; set; }

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
		[BsonRepresentation(BsonType.Double)]
		public decimal StartPrice { get; set; }

		/// <summary>
		/// 开始原因
		/// </summary>
		public string StartReason { get; set; }

		/// <summary>
		/// 结束日期
		/// </summary>
		public string EndDate { get; set; }

		/// <summary>
		/// 结束价格
		/// </summary>
		[BsonRepresentation(BsonType.Double)]
		public decimal EndPrice { get; set; }

		/// <summary>
		/// 结束原因
		/// </summary>
		public string EndReason { get; set; }

		/// <summary>
		/// 最大头寸
		/// </summary>
		[BsonRepresentation(BsonType.Int32)]
		public int MaxHolding { get; set; }

		/// <summary>
		/// 利润
		/// </summary>
		[BsonRepresentation(BsonType.Double)]
		public decimal Profit { get; set; }
	}
}