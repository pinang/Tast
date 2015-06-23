using FluentScheduler;
using NLog;
using Tast.BusinessLogic.Stock;

namespace Tast.BusinessLogic.Task
{
	public class TaskScheduleManager
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public static void TaskScheduleInit()
		{
			TaskManager.Initialize(new TaskScheduleRegistry());
			logger.Info("初始化任务计划成功");
		}
	}
}