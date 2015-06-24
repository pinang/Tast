using SqlFu;
using System.Configuration;

namespace Tast.BusinessLogic.Database
{
	public class DatabaseManager
	{
		public static void InitDatabaseConnection()
		{
			SqlFuDao.ConnectionStringIs(ConfigurationManager.ConnectionStrings["tast"].ConnectionString, DbEngine.MySql);
		}
	}
}