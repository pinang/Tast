using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tast.BusinessLogic.Database;
using Tast.BusinessLogic.TradingSystem;

namespace Tast.BusinessLogic.Test
{
	[TestClass]
	public class TurtleTradingSystemManagerTest
	{
		public TurtleTradingSystemManagerTest()
		{
			DatabaseManager.InitDatabaseConnection();
		}

		[TestMethod]
		public void TestSystemVerificationInit()
		{
			TurtleTradingSystemManager.SystemVerificationInit();

			Assert.IsTrue(true);
		}
	}
}