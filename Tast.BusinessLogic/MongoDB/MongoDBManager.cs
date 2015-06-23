using MongoDB.Driver;
using System.Configuration;

namespace Tast.BusinessLogic.MongoDB
{
	public class MongoDBManager
	{
		/// <summary>
		/// 数据库地址
		/// </summary>
		private static string mongoAddress = null;
		/// <summary>
		/// 数据库名称
		/// </summary>
		private static string mongoDatabase = null;

		static MongoDBManager()
		{
			mongoAddress = ConfigurationManager.AppSettings["mongoAddress"];
			mongoDatabase = ConfigurationManager.AppSettings["mongoDatabase"];
		}

		/// <summary>
		/// 获取MongoDB数据库连接
		/// </summary>
		/// <returns></returns>
		public static IMongoDatabase GetDatabaseInstance()
		{
			return new MongoClient(mongoAddress).GetDatabase(mongoDatabase);
		}
	}
}