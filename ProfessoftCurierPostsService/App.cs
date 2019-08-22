using System;
using System.Data;
using System.Threading;

namespace ProfessoftCurierPostsService
{
    class App
    {
        protected ProfessoftApps.LogEvent logEvent;
        protected ProfessoftApps.LogFile logFile;

        public App()
        {
            try
            {
                logFile = new ProfessoftApps.LogFile(Properties.Settings.Default.LogFilePath);
                logFile.Write("Stworzono obiekt logFile");
                logEvent = new ProfessoftApps.LogEvent(Properties.Settings.Default.SourceEventName, Properties.Settings.Default.LogEventName);
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
                DataModule module = new DataModule(logFile);
                try
                {
                    DataTable deliveriesIds = module.GetDeliveriesDataTable(); //@@@@@@@@@@@@@@@@@@@@@ przy instalacji wywalić człon 'Mock'
                    logFile.Write("Otrzymano: " + deliveriesIds.Rows.Count + " przesyłek do przetworzenia");

                    string deliveryId;
                    DataTable documentIds;
                    foreach (DataRow row in deliveriesIds.Rows)
                    {
                        deliveryId = row.Field<int>(Extensions.ColumnNameDelivery).ToString();
                        try
                        {
                            try
                            {
                                documentIds = module.GetWZDocumentsDataTable(deliveryId); //@@@@@@@@@@@@@@@@@@@@@@ przy instalacji wywalić człon 'Mock'
                            }
                            catch(Exception)
                            {
                                module.UpdateSenditExtTable(deliveryId, States.Do_Sprawdzenia);
                                continue;
                            }
                            Delivery delivery = new Delivery(deliveryId, logFile);
                            delivery.SetPackages(module.GetPackagesDataTable(deliveryId));  //@@@@@@@@@@@@@@@@@ przy instalacji wywalić człon 'Mock'
                            delivery.Valid();
                            delivery.WaitForValidation();

                            foreach(DataRow r in documentIds.Rows)
                            {
                                module.UpdateDocumentDateAtribute(r.Field<int>(Extensions.ColumnNameDocument).ToString(), 
                                                                                delivery.date, 
                                                                                delivery.number, 
                                                                                delivery.state);
                            }

                             module.UpdateSenditExtTable(deliveryId, delivery.state); //@@@@@@@@@@@@@@@@@@@@@ przy instalacji odkomentować
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Wystąpił błąd dla przesyłki o ID: " + deliveryId + " " + e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    module.Dispose();
                    throw new Exception(e.Message);
                }
                module.Dispose();
            }
            catch (Exception e)
            {
                logFile.Write(e.Message);
                logEvent.Write(e.Message);
            }
            logFile.Write(" ");
        }
    }
}