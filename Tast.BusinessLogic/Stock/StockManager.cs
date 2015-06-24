using NLog;
using nZAI.Database;
using System;
using System.Collections.Generic;

namespace Tast.BusinessLogic.Stock
{
	public class StockManager
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public static List<Tast.Entities.Stock.Stock> GetAllEnabled()
		{
			List<Tast.Entities.Stock.Stock> result = null;
			try
			{
				result = DBO.Query<Tast.Entities.Stock.Stock>("SELECT * FROM Stock WHERE Enable = 1 ORDER BY Code");
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
			}

			return result;
		}

		public static void Insert(Tast.Entities.Stock.Stock stock)
		{
			try
			{
				DBO.Insert<Tast.Entities.Stock.Stock>(stock);
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
			}
		}
	}
}