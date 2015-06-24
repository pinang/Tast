using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tast.BusinessLogic.Database;
using Tast.BusinessLogic.Stock;

namespace Tast.BusinessLogic.Test
{
	[TestClass]
	public class StockHistoryManagerTest
	{
		public StockHistoryManagerTest()
		{
			DatabaseManager.InitDatabaseConnection();
		}

		[TestMethod]
		public void TestRefreshAllStockHistory()
		{
			var result = StockHistoryManager.RefreshAllStockHistory();

			Assert.IsTrue(result.Success);
		}
	}
}
