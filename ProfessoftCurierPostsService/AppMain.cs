using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ProfessoftCurierPostsService
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
                new ProCurierPostsService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        private static void MainConsole()
        {
            App app = new App();
            app.Run();
        }
    }
}
