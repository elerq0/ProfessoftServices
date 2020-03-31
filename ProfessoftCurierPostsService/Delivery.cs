using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;

namespace ProfessoftCurierPostsService
{
    class Delivery
    {
        private readonly string ColumnNamePackage = "Paczka_ID";

        private Boolean isCompleted;
        private readonly int timeoutLimit = 100;
        private PROLog.LogFile logFile;

        private string deliveryId;
        private DataTable packagesIds;

        public DateTime date;
        public States state;
        public string number;
        

        public Delivery(string deliveryId, PROLog.LogFile logFile)
        {
            this.deliveryId = deliveryId;
            this.logFile = logFile;

            isCompleted = false;
            state = States.Niezdefiniowany;
        }

        public void SetPackages(DataTable packagesIds)
        {
            this.packagesIds = packagesIds;
        }

        public void WaitForValidation()
        {
            try
            {
                int counter = 0;
                while (!isCompleted)
                {
                    Thread.Sleep(100);
                    counter += 1;
                    if (counter >= timeoutLimit)
                        throw new Exception("Timeout");
                }
                logFile.Write("Zakończono sprawdzenie przesyłki o id: " + deliveryId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async void Valid()
        {
            string response, dateString;
            string firstPackageDateString = string.Empty;

            foreach (DataRow row in packagesIds.Rows)
            {
                try
                {
                    response = await GetResponse(row.Field<string>(ColumnNamePackage));
                    dateString = RegexGetDateString(row.Field<string>(ColumnNamePackage), response, "Przesyłka doręczona");

                    if(firstPackageDateString.Equals(string.Empty) && !dateString.Equals(string.Empty))
                    { // pierwsze użycie, data nie jest pusta
                        firstPackageDateString = dateString;
                        continue;
                    }
                    else if (!firstPackageDateString.Equals(string.Empty) && !dateString.Equals(string.Empty))
                    { // kolejne użycie, data nie jest pusta
                        continue;
                    }
                    else if (dateString.Equals(string.Empty))
                    { // data pusta
                        dateString = RegexGetDateString(row.Field<string>(ColumnNamePackage), response, "Przesyłka anulowana");
                        if (!dateString.Equals(string.Empty))
                        {
                            state = States.Nieaktywny;
                            date = DateTime.Parse(dateString);
                        }
                        else
                        {
                            state = States.Nieprzetworzony;
                        }
                        number = string.Empty;
                        break;
                    }
                }
                catch(Exception e)
                {
                    throw new Exception("Błąd dla numeru: " + row.Field<string>(ColumnNamePackage) + " " + e.Message);
                }
            }

            if(packagesIds.Rows.Count == 0)
            {
                state = States.Nieprzetworzony;
                number = string.Empty;
            }
            else if (state == States.Niezdefiniowany)
            {
                state = States.Przetworzony;
                number = packagesIds.Rows[0].Field<string>(ColumnNamePackage);
                date = DateTime.Parse(firstPackageDateString);
            }

            logFile.Write("Dla przesyłki o id: " + deliveryId + " data: " + date + ", stan: " + state);

            isCompleted = true;
        }
        
        private async Task<string> GetResponse(string id)
        {
            try
            {
                string formData = "?q=" + id + "&typ=1";

                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage;
                string responseString;

                responseMessage = await client.PostAsync(Properties.Settings.Default.DPDFindPackageUrl + formData, null);
                logFile.Write("Http post request '" + Properties.Settings.Default.DPDFindPackageUrl + formData + "' ");
                responseString = await responseMessage.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception e)
            {
                throw new Exception("Http exception dla '" + id + "' " + e.Message);
            }
        }
        
        private string RegexGetDateString(string numer, string text, string lookingFor)
        {
            try
            {
                Regex rx = new Regex(@"<td>([0-9-]+)</td>(?:[\s\r\n]*)<td>(?:[\d:]*)</td>(?:[\s\r\n]*)<td>" + lookingFor);
                MatchCollection matches = rx.Matches(text);
                if (matches.Count == 0)
                    return String.Empty;
                else
                    return matches[0].Groups[1].Value;
            }
            catch (Exception e)
            {
                throw new Exception("Regex exception dla '" + numer + "' " + e.Message);
            }
        }
    }
}
