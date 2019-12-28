using System;
using System.Data;

namespace ProfessoftCurierPostsService
{

    class DataModule
    {
        protected PROLog.LogFile logFile;
        protected PROOptima.Optima optima;
        protected PROSql.SQL sql;

        private readonly Boolean debug = false;

        public DataModule(PROLog.LogFile logFile)
        {
            this.logFile = logFile;
            try
            {
                optima = new PROOptima.Optima(Properties.Settings.Default.OptimaPath, true);
                logFile.Write("Stworzono obiekt AppOptima");

                sql = new PROSql.SQL(Properties.Settings.Default.SQLServerName,
                                                Properties.Settings.Default.SQLDatabase,
                                                Properties.Settings.Default.SQLUsername,
                                                Properties.Settings.Default.SQLPassword,
                                                Properties.Settings.Default.SQLNT);
                logFile.Write("Stworzono obiekt AppSQL");
            }
            catch (Exception e)
            {
                optima = null;
                sql = null;
                throw new Exception(e.Message);
            }
        }

        private void OptimaConnect()
        {
            if (optima.Login(Properties.Settings.Default.OptimaUsername,
                                Properties.Settings.Default.OptimaPassword,
                                Properties.Settings.Default.OptimaCompany))
                logFile.Write("Zalogowano do ERP Optima");
        }

        private void OptimaDisconnect()
        {
            optima.LogOut();
            logFile.Write("Wylogowano z ERP Optima");
        }

        private void OptimaSave()
        {
            optima.Save();
            logFile.Write("Zapisano zmiany w ERP Optima");
        }

        private void SqlConnect()
        {
            if (sql.Connect())
                logFile.Write("Nawiązano połączenie z serwerem SQL");
        }

        private void SqlDisconnect()
        {
            sql.Disconnect();
            logFile.Write("Zamknięto połączenie z serwerem SQL");
        }

        public void Dispose()
        {
            sql.Disconnect();
            logFile.Write("Zamknięto połączenie z serwerem SQL");

            optima.LogOut();
            logFile.Write("Wylogowano z ERP Optima");
        }

        public DataTable GetDeliveriesDataTable()
        {
            if (debug)
                return GetDeliveriesDataTableMock();

            DataTable dt;
            try
            {
                SqlConnect();
                dt = sql.Execute("exec " + Extensions.ProcDeliveries);
                logFile.Write("Stworzono listę przesyłek");
            }
            catch (Exception e)
            {
                throw new Exception("Błąd przy pobieraniu listy przesyłek: " + e.Message);
            }
            return dt;
        }

        public DataTable GetDeliveriesDataTableMock()
        {
            int t = 1;
            DataTable dt = new DataTable();
            dt.Columns.Add(Extensions.ColumnNameDelivery, t.GetType());
            dt.Rows.Add(dt.NewRow()[Extensions.ColumnNameDelivery] = 1);
            dt.Rows.Add(dt.NewRow()[Extensions.ColumnNameDelivery] = 2);
            dt.Rows.Add(dt.NewRow()[Extensions.ColumnNameDelivery] = 3);

            logFile.Write("Stworzono listę przesyłek");
            return dt;
        }

        public DataTable GetPackagesDataTable(string deliveryId)
        {
            if (debug)
                return GetPackagesDataTableMock(deliveryId);

            DataTable dt;
            try
            {
                SqlConnect();
                dt = sql.Execute("exec " + Extensions.ProcPackages + " " + deliveryId);
                logFile.Write("Stworzono listę paczek dla przesyłki o id: [" + deliveryId + "]");
            }
            catch (Exception e)
            {
                throw new Exception("Błąd przy pobieraniu listy paczek dla przesyłki o id: [" + deliveryId + "], " + e.Message);
            }
            return dt;
        }

        public DataTable GetPackagesDataTableMock(string deliveryId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(Extensions.ColumnNamePackage);
            switch (deliveryId)
            {
                case "1":
                    dt.Rows.Add(dt.NewRow()[Extensions.ColumnNamePackage] = @"0000160561270U");
                    dt.Rows.Add(dt.NewRow()[Extensions.ColumnNamePackage] = @"0000160561271U");
                    dt.Rows.Add(dt.NewRow()[Extensions.ColumnNamePackage] = @"0000160561272U");
                    dt.Rows.Add(dt.NewRow()[Extensions.ColumnNamePackage] = @"0000160561273U");
                    break;
                case "2":
                    dt.Rows.Add(dt.NewRow()[Extensions.ColumnNamePackage] = @"0000169596774U");
                    dt.Rows.Add(dt.NewRow()[Extensions.ColumnNamePackage] = @"0000169596775U");
                    break;
                case "3":
                    dt.Rows.Add(dt.NewRow()[Extensions.ColumnNamePackage] = @"0000169835189U");
                    break;
            }


            logFile.Write("Stworzono listę paczek dla przesyłki o id: " + deliveryId);
            return dt;
        }

        public DataTable GetWZDocumentsDataTable(string deliveryId)
        {
            if (debug)
                return GetWZDocumentsDataTableMock(deliveryId);

            DataTable dt;
            try
            {
                SqlConnect();
                dt = sql.Execute("exec " + Extensions.ProcDocuments + " " + deliveryId);
                if (dt.Rows.Count == 0)
                    throw new Exception("Przesyłka nie jest powiązana z dokumentem WZ");

                logFile.Write("Stworzono listę dokumentów WZ dla przesyłki o id: [" + deliveryId + "]");
            }
            catch (Exception e)
            {
                throw new Exception("Błąd przy pobieraniu listy dokumentów WZ dla przesyłki o id: [" + deliveryId + "], " + e.Message);
            }
            return dt;
        }

        public DataTable GetWZDocumentsDataTableMock(string deliveryId)
        {
            int t = 1;
            DataTable dt = new DataTable();
            dt.Columns.Add(Extensions.ColumnNameDocument, t.GetType());
            dt.Rows.Add(dt.NewRow()[Extensions.ColumnNameDocument] = 3239);

            return dt;
        }

        public void UpdateDocumentDateAtribute(string documentId, DateTime date, string number, States state)
        {
            OptimaConnect();
            string description = Properties.Settings.Default.DescriptionPrefix + " " + number;
            string atrName = Properties.Settings.Default.AtributeName;
            try
            {
                if (state == States.Przetworzony)
                {
                    CDNHlmn.IDokumentHaMag doc = optima.GetDocumentHaMagByID(documentId);
                    string atrValue = date.ToShortDateString() + " " + description + " {" + DateTime.Now.ToShortDateString() + ' ' + DateTime.Now.ToLongTimeString() + " " + optima.operatorKod + "[" + optima.operatorID + "]}";
                    optima.AddOrEditAtributeDocumentHaMag(doc, atrName, atrValue);
                    logFile.Write("Do dokumentu o ID: [" + doc.ID + "] dodano atrybut " + atrName + " o wartości: [ " + atrValue + " ]");
                    OptimaSave();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Błąd podczas dodawania/aktualizowania atrybutu dla TrNId = [" + documentId + "] " + e.Message);
            }
        }

        public void UpdateSenditExtTable(string deliveryId, States state)
        {
            if (debug)
                return;

            try
            {
                SqlConnect();
                sql.Execute("exec " + Extensions.ProcUpdateSenditExt + " " + deliveryId + ", " + (int)state);
                logFile.Write("Zaktualizowano tabele SenditExt dla przesyłki o id: [" + deliveryId + "], stan = " + state);
            }
            catch (Exception e)
            {
                throw new Exception("Błąd przy aktualizowaniu tabeli SenditExt dla przesyłki o id: [" + deliveryId + "], " + e.Message);
            }
        }
    }
}
