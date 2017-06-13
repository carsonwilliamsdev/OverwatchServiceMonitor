using System.ServiceProcess;

namespace OverwatchPriorityService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new OverwatchPriorityService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
