using NLog;
using SqlFu;
using System;
using System.Collections.Generic;
using System.Linq;

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
				using (var db = SqlFuDao.GetConnection())
				{
					result = db.Query<Tast.Entities.Stock.Stock>(s => s.Enable).ToList();

					db.Close();
				}
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
				using (var db = SqlFuDao.GetConnection())
				{
					db.Insert(stock);

					db.Close();
				}
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
			}
		}
	}
}