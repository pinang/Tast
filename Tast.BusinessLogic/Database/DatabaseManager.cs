using MySql.Data.MySqlClient;
using nZAI.Database;
using System.Configuration;

namespace Tast.BusinessLogic.Database
{
	public class DatabaseManager
	{
		public static void InitDatabaseConnection()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["tast"].ConnectionString;
			DBO.RegisterConn(SupportDatabaseType.MySql, () => { return new MySqlConnection(connectionString); });
		}
	}
}