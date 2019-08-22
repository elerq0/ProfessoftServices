using System.ServiceProcess;
using System.Timers;

namespace ProfessoftCurierPostsService
{
    public partial class ProCurierPostsService : ServiceBase
    {

        protected Timer t;
        public ProCurierPostsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            t = new Timer();
            if (Properties.Settings.Default.FirstRunImmediately)
                t.Interval = 10000;
            else
                t.Interval = Properties.Settings.Default.RefreshTimeInMin * 1000;

            t.Elapsed += new ElapsedEventHandler(this.OnTimer);
            t.Start();
        }

        protected void OnTimer(object sender, ElapsedEventArgs e)
        {
            t.Enabled = false;

            App app = new App();
            app.Run();

            t.Enabled = true;
            t.Interval = Properties.Settings.Default.RefreshTimeInMin * 60000;
        }

        protected override void OnStop()
        {
        }
    }
}
