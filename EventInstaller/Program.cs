using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.EventLog.CreateEventSource(Properties.Settings.Default.SourceEventName, Properties.Settings.Default.LogEventName);
        }
    }
}
