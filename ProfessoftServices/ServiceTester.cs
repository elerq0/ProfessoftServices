using System;
using System.ServiceProcess;
using System.Timers;

namespace ProfessoftServices
{
    public partial class ServiceTester : ServiceBase
    {
        protected Timer t;
        public ServiceTester()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            t = new Timer();
            t.Interval = 10000; // 10 sec
            t.Elapsed += new ElapsedEventHandler(this.OnTimer);
            t.Start();
        }

        protected void OnTimer(object sender, ElapsedEventArgs e)
        {
            t.Enabled = false;

            AppTester tester = new AppTester();
            tester.Run();

            t.Enabled = true;

        }

        protected override void OnStop()
        {
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);
            if(command == 128)
            {

            }
            else if(command == 129)
            {

            }
        }
    }
}
