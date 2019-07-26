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
            t.Interval = 10000; // 86400000; // 1 day
            t.Elapsed += new ElapsedEventHandler(this.OnTimer);
            t.Start();
        }

        protected void OnTimer(object sender, ElapsedEventArgs e)
        {
            t.Enabled = false;

            App app = new App();
            app.Run();

            t.Enabled = true;
        }

        protected override void OnStop()
        {
        }
    }
}
