using System;


namespace XLTester
{
    class Tester
    {
        static int Main(string[] args)
        {
            string xlPath = @"C:\Program Files (x86)\Comarch ERP XL 2017.1";
            string logFilePath = @"C:\temp";

            ProfessoftApps.LogEvent logEvent;
            ProfessoftApps.LogFile logFile;

            try
            {
                logFile = new ProfessoftApps.LogFile(logFilePath, "ProffLog");
                logFile.Write("Stworzono obiekt logFile");
                logEvent = new ProfessoftApps.LogEvent("ProfessoftServices");
                logFile.Write("Stworzono obiekt logEvent");
            }
            catch (Exception)
            {
                return -2;
            }

            try
            {

                ProfessoftApps.XL xl = new ProfessoftApps.XL(xlPath);
                logFile.Write("Stworzono obiekt AppXL");

                string operXL = "OPERATOR";
                string passXL = "PASSWORD";
                string databaseXL = "DATABASE";
                string keyXL = @"KEY";
                xl.Login(operXL, passXL, databaseXL, keyXL);
                logFile.Write("Zalogowano do ERP XL");

                xl.Logout();
                logFile.Write("Wylogowano z ERP XL");
                return 0;
            }
            catch (Exception e)
            {
                logFile.Write(e.Message);
                logEvent.Write(e.Message);
                return -1;
            }

        }
    }
}
