
namespace Tast.Entities.TradingSystem
{
	public class TurtleTradingSystemParameter
	{
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
	}
}