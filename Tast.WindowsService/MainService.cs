using NLog;
using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using Tast.BusinessLogic.Stock;
using Tast.BusinessLogic.Task;
using Tast.BusinessLogic.TradingSystem;

namespace Tast.WindowsService
{
	partial class MainService : ServiceBase
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public MainService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				logger.Info("服务启动");

				//StockManager.Insert(new Entities.Stock.Stock { Code = "AAPL", ChineseName = "苹果", Enable = true, LastHistoryDate = null });

				//var stocks = StockManager.GetAllEnabled();
				//foreach(var stock in stocks)
				//{
				//	logger.Info("Code:{0} ChineseName:{1}", stock.Code, stock.ChineseName);
				//}

				//Task.Factory.StartNew(() =>
				//{
				//	StockHistoryManager.RefreshAllStockHistory();
				//});

				//	初始化计划任务
				TaskScheduleManager.TaskScheduleInit();

				//	初始化海龟交易演算系统
				TurtleTradingSystemManager.SystemVerificationInit();

				logger.Info("服务启动成功");
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
			}
		}

		protected override void OnStop()
		{
			try
			{
				logger.Info("开始停止服务");
				logger.Info("服务已停止");
			}
			catch (Exception ex)
			{
				logger.Error<Exception>(ex);
			}
		}
	}
}