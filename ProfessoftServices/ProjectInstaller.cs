using System.ComponentModel;
using System.Configuration.Install;

namespace ProfessoftServices
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.User;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;

        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

    }
}
