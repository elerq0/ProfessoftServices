using System;
using System.Data;

namespace ProfessoftCurierPostsService
{
    class App
    {
        protected readonly string atrName = "DATADOSTAWY";
        protected readonly string descriptionPrefix = "DPD";

        protected readonly string sqlCmd = "";

        protected ProfessoftApps.LogEvent logEvent;
        protected ProfessoftApps.LogFile logFile;
        protected ProfessoftApps.Optima optima;
        protected ProfessoftApps.SQL sql;

        public App()
        {
            try
            {
                logFile = new ProfessoftApps.LogFile(Properties.Settings.Default.LogFilePath, "ProCurierPosts");
                logFile.Write("Stworzono obiekt logFile");
                logEvent = new ProfessoftApps.LogEvent("ProCurierPostsService");
                logFile.Write("Stworzono obiekt logEvent");
            }
            catch (Exception e)
            {
                throw new Exception("Krytyczny błąd!" + e.Message);
            }

        }

        public void Run()
        {
            try
            {
                optima = new ProfessoftApps.Optima(Properties.Settings.Default.OptimaPath);
                logFile.Write("Stworzono obiekt AppOptima");

                sql = new ProfessoftApps.SQL(Properties.Settings.Default.SQLServername,
                                                Properties.Settings.Default.SQLDatabase,
                                                Properties.Settings.Default.SQLUsername,
                                                Properties.Settings.Default.SQLPassword,
                                                Properties.Settings.Default.SQLNT,
                                                Properties.Settings.Default.SQLKey);
                logFile.Write("Stworzono obiekt AppSQL");

                try
                {
                    optima.Login(Properties.Settings.Default.OptimaUsername,
                                Properties.Settings.Default.OptimaPassword,
                                Properties.Settings.Default.OptimaCompany);
                    logFile.Write("Zalogowano do ERP Optima");

                    try
                    {
                        sql.Connect();
                        logFile.Write("Nawiązano połączenie z serwerem SQL");

                        try
                        {
                            //DataTable dt = sql.Execute(sqlCmd);
                            DataTable dt = GetMockData();
                            logFile.Write("Otrzymano: " + dt.Rows.Count + " przesyłek do przetworzenia");

                            foreach (DataRow row in dt.Rows)
                            {
                                try
                                {
                                    Parcel parcel = new Parcel(row.Field<string>("SZLID"), row.Field<string>("TrNID"), optima, sql, logFile);
                                    parcel.Valid();
                                    parcel.WaitForValidation();
                                    parcel.UpdateDocumentDateAtribute(descriptionPrefix, atrName);

                                    optima.Save();
                                    logFile.Write("Zapisano zmiany w ERP Optima");
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Wystąpił błąd dla przesyłki o ID: " + row.Field<string>("SZLID") + " " + e.Message);
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            sql.Disconnect();
                            logFile.Write("Zamknięto połączenie z serwerem SQL");
                            throw new Exception(e.Message);
                        }
                        sql.Disconnect();
                        logFile.Write("Zamknięto połączenie z serwerem SQL");
                    }
                    catch(Exception e)
                    {
                        optima.LogOut();
                        logFile.Write("Wylogowano z ERP Optima");
                        throw new Exception(e.Message);
                    }
                    optima.LogOut();
                    logFile.Write("Wylogowano z ERP Optima");
                }
                catch(Exception e)
                {
                    optima = null;
                    sql = null;
                    throw new Exception(e.Message);
                }
            }
            catch(Exception e)
            {
                logFile.Write(e.Message);
                logEvent.Write(e.Message);
            }
            logFile.Write(" ");
        }

        private DataTable GetMockData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SZLID");
            dt.Columns.Add("TrNID");

            DataRow r1 = dt.NewRow();
            r1["SZLID"] = 1;
            r1["TrNID"] = 148;
            dt.Rows.Add(r1);

            DataRow r2 = dt.NewRow();
            r2["SZLID"] = 2;
            r2["TrNID"] = 160;
            dt.Rows.Add(r2);

            DataRow r3 = dt.NewRow();
            r3["SZLID"] = 3;
            r3["TrNID"] = 161;
            dt.Rows.Add(r3);

            return dt;
        }
    }
}
