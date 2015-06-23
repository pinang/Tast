using System.ServiceProcess;

namespace Tast.WindowsService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase.Run(new MainService());
		}
	}
}