using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using Tast.BusinessLogic.MongoDB;

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
				var db = MongoDBManager.GetDatabaseInstance();
				var collection = db.GetCollection<Tast.Entities.Stock.Stock>("Stock");
				var task = collection.Find(s => s.Enable).ToListAsync();
				task.Wait();

				result = task.Result;
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
				var db = MongoDBManager.GetDatabaseInstance();
				var collection = db.GetCollection<Tast.Entities.Stock.Stock>("Stock");

				collection.InsertOneAsync(stock).Wait();
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
			}
		}
	}
}