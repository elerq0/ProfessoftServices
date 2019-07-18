using System;
using System.ServiceProcess;

namespace ProfessoftServices
{
    static class AppMain
    {
        static void Main()
        {
            if (!Environment.UserInteractive)
            {
                MainServices();
            }
            else
            {
                MainConsole();
            }

        }

        private static void MainServices()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ServiceTester()
            };
            ServiceBase.Run(ServicesToRun);
        }

        private static void MainConsole()
        {
            AppTester tester = new AppTester();
            tester.Run();
        }
    } 
}
