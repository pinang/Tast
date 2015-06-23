using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		}
	}
}
