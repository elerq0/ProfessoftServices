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
    class Parcel
    {
        private Boolean isCompleted;
        private readonly int timeoutLimit = 100;
        private DataTable parcesIds = new DataTable();
        private ProfessoftApps.LogFile logFile;
        private ProfessoftApps.Optima optima;
        private ProfessoftApps.SQL sql;
        protected string sqlCmd = "select SPA_NumerListu as numer from CDN.SenditPaczki " +
                                    "join CDN.SenditZleceniePrzesylki on SZL_SZLID = SPA_SZLID " +
                                    "where SPA_SZLID = ";

        public DateTime date;
        public string state;
        public string number;

        public string parcelID;
        public string documentID;
        

        public Parcel(string parcelID, string documentID, ProfessoftApps.Optima optima, ProfessoftApps.SQL sql, ProfessoftApps.LogFile logFile)
        {
            this.optima = optima;
            this.sql = sql;
            this.logFile = logFile;
            this.parcelID = parcelID;
            this.documentID = documentID;

            isCompleted = false;
            state = string.Empty;

            parcesIds = GetUrlsMock();
            //parcesIds = sql.Execute(sqlCmd + parcelID);
            logFile.Write("Stworzono datatable dla id: " + parcelID);
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
                logFile.Write("Zakończono sprawdzenie przesyłki o id: " + parcelID);
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

            foreach (DataRow row in parcesIds.Rows)
            {
                try
                {
                    response = await GetResponse(row.Field<string>("numer"));
                    dateString = RegexGetDateString(row.Field<string>("numer"), response, "Przesyłka doręczona");

                    if (firstPackageDateString.Equals(string.Empty) && !dateString.Equals(string.Empty))
                    { // pierwsze użycie i pierwsze przesyłka dostarczona
                        firstPackageDateString = dateString;

                    }
                    else if (!firstPackageDateString.Equals(dateString) || firstPackageDateString.Equals(string.Empty))
                    { // któraś przesyłka jest róźna od pierwszej lub pierwsza przesyłka nie była dostarczna
                        dateString = RegexGetDateString(row.Field<string>("numer"), response, "Przesyłka anulowana");
                        if (!dateString.Equals(string.Empty))
                        {
                            state = "Nieaktywny";
                            date = DateTime.Parse(dateString);
                        }
                        else
                        {
                            state = "Nieprzetworzony";
                        }
                        number = string.Empty;
                        break;
                    }
                }
                catch(Exception e)
                {
                    throw new Exception("Błąd dla numeru: " + row.Field<string>("numer") + " " + e.Message);
                }
            }

            if (state.Equals(string.Empty))
            {
                state = "Przetworzony";
                number = parcesIds.Rows[0].Field<string>("numer");
                date = DateTime.Parse(firstPackageDateString);
            }

            logFile.Write("Dla przesyłki o id: " + parcelID + " data: " + date + ", stan: " + state);

            isCompleted = true;
        }

        public void UpdateDocumentDateAtribute(string descriptionPrefix, string atrName)
        {
            string description = descriptionPrefix + " " + number;
            try
            {
                if (state.Equals("Przetworzony"))
                {
                    CDNHlmn.IDokumentHaMag doc = optima.GetDocumentHaMagByID(documentID);
                    string atrValue = date.ToShortDateString() + " " + description + " {" + DateTime.Now.ToShortDateString() + ' ' + DateTime.Now.ToLongTimeString() + " " + optima.operatorKod + "[" + optima.operatorID + "]}";
                    optima.AddOrEditAtributeDocumentHaMag(doc, atrName, atrValue);
                    logFile.Write("Do dokumentu o ID: " + doc.ID + " dodano atrybut " + atrName + " o wartości: [ " + atrValue + " ]");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Błąd podczas dodawania/aktualizowania atrybutu dla TrNId = " + documentID + " " + e.Message);
            }
        }



        private DataTable GetUrlsMock()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("numer");
            dt.Rows.Add(dt.NewRow()["numer"] = @"0000160561270U");
            dt.Rows.Add(dt.NewRow()["numer"] = @"0000160561271U");
            dt.Rows.Add(dt.NewRow()["numer"] = @"0000160561272U");
            dt.Rows.Add(dt.NewRow()["numer"] = @"0000160561273U");

            return dt;
        }

        private async Task<string> GetResponse(string id)
        {
            try
            {
                string url = @"https://tracktrace.dpd.com.pl/findPackage";
                string formData = "?q=" + id + "&typ=1";

                HttpClient client = new HttpClient();
                HttpResponseMessage responseMessage;
                string responseString;

                responseMessage = await client.PostAsync(url + formData, null);
                logFile.Write("Http post request '" + url + formData + "' ");
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
