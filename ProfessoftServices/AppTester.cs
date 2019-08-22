using System;
using System.Data;
using System.Diagnostics;

namespace ProfessoftServices
{
    class AppTester
    {
        
        protected readonly string xlPath = @"C:\Program Files (x86)\Comarch ERP XL 2017.1";
        protected readonly string optimaPath = @"C:\Program Files (x86)\Comarch ERP Optima";
        protected readonly string logFilePath = @"C:\temp";

        protected ProfessoftApps.LogEvent logEvent;
        protected ProfessoftApps.LogFile logFile;
        
        public AppTester()
        {
            try
            {
                logFile = new ProfessoftApps.LogFile(logFilePath, "ProffLog");
                logFile.Write("Stworzono obiekt logFile");
                logEvent = new ProfessoftApps.LogEvent("ProfessoftServices");
                logFile.Write("Stworzono obiekt logEvent");
            }
            catch(Exception)
            {
                throw new Exception("Krytyczny błąd!");
            }
        }

        public void Run()
        {
            try
            {
                logEvent.Write("Rozpoczęto sekwencję testową");
                logFile.Write("Rozpoczęto sekwencję testową");

                //=======================================================================================================================//


                ProfessoftApps.Optima optima = new ProfessoftApps.Optima(optimaPath);
                logFile.Write("Stworzono obiekt AppOptima");

                string operOptima = "PROADMIN";
                string passOptima = "Profes45";
                string companyOptima = "Dotykačka Polska sp.z o.o.";

                Console.WriteLine(optima.GetState());

                optima.Login(operOptima, passOptima, companyOptima);
                logFile.Write("Zalogowano do ERP Optima");

                Console.WriteLine(optima.GetState());

                /*

                string contractorName = "Testowy trzeci";
                CDNHeal.Kontrahent kontrahent;
                try
                {
                    kontrahent = optima.GetContractorCollection().AddNew();
                    kontrahent.Akronim = contractorName;
                    logFile.Write("Stworzono kontrahenta: " + contractorName);
                }
                catch (Exception)
                {
                    throw new Exception("Użytkownik o akronimie " + contractorName + "juz istnieje!");
                }

                try
                {
                    optima.Save();
                    logFile.Write("Zapisano zmiany w ERP Optima");
                }
                catch(Exception e)
                {
                    optima.LogOut();
                    logFile.Write("Wylogowano z ERP Optima");
                    throw new Exception(e.Message);
                }

                kontrahent.Email = "test.mail.com";
                logFile.Write("Ustawiono mail " + kontrahent.Akronim + " na " + kontrahent.Email);

                try
                {
                    optima.Save();
                    logFile.Write("Zapisano zmiany w ERP Optima");
                }
                catch (Exception e)
                {
                    optima.LogOut();
                    logFile.Write("Wylogowano z ERP Optima");
                    throw new Exception(e.Message);
                }

                CDNHeal.Kontrahent kontrahent2;
                try
                {
                    kontrahent2 = optima.GetContractorByName("Testowy trzeci");
                    logFile.Write("Znaleziono kontrahenta " + kontrahent2.Akronim + " po nazwie");
                }
                catch(Exception e)
                {
                    throw new Exception(e.Message);
                }


                optima.GetContractorCollection().Delete(kontrahent);
                logFile.Write("Usunięto kontrahenta: " + contractorName);

                */

                try
                {
                    optima.Save();
                    logFile.Write("Zapisano zmiany w ERP Optima");
                }
                catch (Exception e)
                {
                    optima.LogOut();
                    logFile.Write("Wylogowano z ERP Optima");
                    throw new Exception(e.Message);
                }

                


                optima.LogOut();
                logFile.Write("Wylogowano z ERP Optima");

                Console.WriteLine(optima.GetState());

                //=======================================================================================================================//

                /*

                using (Process exe = new Process())
                {
                    exe.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    exe.StartInfo.FileName = @"C:\Users\elerq\source\repos\ProfessoftServices\XLTester\bin\Debug\XLTester.exe";
                    exe.Start();
                    exe.WaitForExit();
                    if (exe.ExitCode == -2)
                        throw new Exception("Krytyczny błąd!");
                    else if (exe.ExitCode == -1)
                        throw new Exception("Błąd XL");
                }

                */

                //=======================================================================================================================//

                /*
                
                string serverNameSQL = @"SERVERNAME";
                string databaseSQL = @"DATABASE";
                string operSQL = "OPERATOR";
                string passSQL = "PASSWORD";

                ProfessoftApps.SQL sql = new ProfessoftApps.SQL(serverNameSQL, databaseSQL, operSQL, passSQL);
                logFile.Write("Stworzono obiekt AppSQL");

                sql.Connect();
                logFile.Write("Nawiązano połączenie z serwerem SQL");

                string sqlCmd = "select top 1 Knt_Kod from CDN.Kontrahenci where Knt_KntId > FLOOR(Rand()*300 + 100)";
                DataTable dt = sql.Execute(sqlCmd);
                logFile.Write("Wykonano zapytanie: " + sqlCmd + " Wynik: " + dt.Rows[0][0]);

                sql.Disconnect();
                logFile.Write("Zamknięto połączenie z serwerem SQL");

                */

                //=======================================================================================================================//

                logEvent.Write("Zakończono sekwencję testową");
                logFile.Write("Zakończono sekwencję testową");

            }
            catch (Exception e)
            {
                logFile.Write(e.Message);
                logEvent.Write(e.Message);
            }
        }

        public void LogFileWrite(string msg)
        {
            logFile.Write(msg);
        }
    }
}
