using FluentScheduler;
using Tast.BusinessLogic.Stock;

namespace Tast.BusinessLogic.Task
{
	public class TaskScheduleRegistry: Registry
	{
		public TaskScheduleRegistry()
		{
			//	每晚4点定时更新股票数据
			Schedule(() =>
			{
				StockHistoryManager.RefreshAllStockHistory();
			}).ToRunEvery(1).Days().At(1, 47);
		}
	}
}