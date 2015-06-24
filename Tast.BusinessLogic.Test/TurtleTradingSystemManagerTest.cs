using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tast.BusinessLogic.TradingSystem;

namespace Tast.BusinessLogic.Test
{
	[TestClass]
	public class TurtleTradingSystemManagerTest
	{
		[TestMethod]
		public void TestSystemVerificationInit()
		{
			TurtleTradingSystemManager.SystemVerificationInit();

			Assert.IsTrue(true);
		}
	}
}