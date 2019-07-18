using System;
using System.Diagnostics;

namespace ProfessoftApps
{
    public class LogEvent
    {
        protected EventLog eventLog;
        public LogEvent(string name)
        {
            try
            {
                if (!System.Diagnostics.EventLog.SourceExists(name + "Source"))
                {
                    System.Diagnostics.EventLog.CreateEventSource(name + "Source", name + "Log");
                }
                eventLog = new System.Diagnostics.EventLog
                {
                    Source = name + "Source",
                    Log = name + "Log"
                };
            }
            catch (Exception e)
            {
                throw new Exception("Wystąpił błąd przy tworzeniu dziennika zdarzeń: " + name + ", " + e.Message);
            }
        }

        public void Write(string msg)
        {
            try
            {
                eventLog.WriteEntry(msg);
            }
            catch (Exception e)
            {
                throw new Exception("Wystąpił błąd przy tworzeniu zapisu do dziennika: " + e.Message);
            }
        }
    }
}
