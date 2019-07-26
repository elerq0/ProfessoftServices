using System;

namespace ProfessoftApps
{
    public class Optima
    {
        protected CDNBase.Application application = null;
        protected CDNBase.ILogin login = null;
        protected CDNBase.AdoSession session = null;
        protected string path;

        public int operatorID;
        public string operatorKod;
        public Boolean connected;

        public Optima(string path)
        {
            this.path = path;
            connected = false;
        }

        public void Login(string oper, string pass, string company)
        {
            object[] hPar = new object[] {1, // Księga_podatkowa
                                          0, // Księga_handlowa
                                          0, // Księga_handlowa_plus
                                          0, // Środki_trwale
                                          1, // Faktury
                                          1, // MAG?? Magazyny
                                          0, // Płace_i_kadry
                                          0, // Płace_i_kadry_xl?????
                                          0, // CRM
                                          0, // Analizy
                                          0, // DET????
                                          0, // BIU????
                                          0, // Serwis
                                          0, // Obieg_dokumentow
                                          0, // Kasa_bank
                                          1, // Kasa_bank_plus
                                          1, // Handel_plus
                                          0};// CRM_plus

            try
            {
                RefreshEnvironmentPath();
                application = new CDNBase.Application();                
                application.LockApp(513, 5000, null, null, null, null);
                login = application.Login(oper, pass, company, hPar[0], hPar[1], hPar[2], hPar[3], hPar[4], hPar[5], hPar[6], hPar[7], hPar[8], hPar[9], hPar[10], hPar[11], hPar[12], hPar[13], hPar[14], hPar[15], hPar[16], hPar[17]);
                session = login.CreateSession();
                operatorID = login.OperatorParam.ID;
                operatorKod = login.OperatorParam.Akronim;
                connected = true;
            }
            catch (Exception e)
            {
                application.UnlockApp();
                connected = false;
                throw new Exception("Wystąpił błąd przy logowaniu do ERP Optima: " + e.Message);
            }
        }

        public void LogOut()
        {
            try
            {
                RefreshEnvironmentPath();
                login = null;
                application.UnlockApp();
                application = null;
                session = null;
                connected = false;
            }
            catch (Exception e)
            {
                throw new Exception("Wystąpił błąd przy wylogowywaniu z ERP Optima: " + e.Message);
            }
        }

        public void Save()
        {
            session.Save();
        }

        public CDNBase.ICollection GetContractorCollection()
        {
            try
            {
                return session.CreateObject("CDN.Kontrahenci", null);
            }
            catch (Exception)
            {
                throw new Exception("Nie można stworzyć kolekcji kontrahentów");
            }
        }

        public CDNHeal.Kontrahent GetContractorByName(string code)
        {
            try
            {
                return session.CreateObject("CDN.Kontrahenci").Item("Knt_Kod = " + code);
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono kontrahenta o akronimie: " + code);
            }

        }

        public CDNHlmn.IDokumentHaMag GetDocumentHaMagByID(string trNID)
        {
            try
            {
                return session.CreateObject("CDN.DokumentyHaMag").Item("TrN_TrNId = " + trNID);
            }
            catch(Exception)
            {
                throw new Exception("Nie znaleziono dokkumentu HaMag o id: " + trNID);
            }
        }

        public void AddOrEditAtributeDocumentHaMag(CDNHlmn.IDokumentHaMag doc, string code, string value)
        {
            Boolean found = false;
            foreach(CDNTwrb1.IDokAtrybut atr in doc.Atrybuty)
            {
                if(atr.Kod.Equals(code))
                {
                    atr.Wartosc = value;
                    found = true;
                    break;
                }
            }

            if(!found)
            {
                CDNTwrb1.IDefAtrybut defAtrybut = session.CreateObject("CDN.DefAtrybuty").Item("DeA_Kod = '" + code + "' and DeA_Typ = 4");
                CDNTwrb1.IDokAtrybut atrybut = doc.Atrybuty.AddNew();
                atrybut.DeAID = defAtrybut.ID;
                atrybut.Wartosc = value;
            }
        }

        public void CreateDefAtribute(string name, int format, int type)
        {
            try
            {
                CDNTwrb1.DefAtrybut defAtrybut = session.CreateObject("CDN.DefAtrybuty", null).AddNew();
                defAtrybut.Kod = name;
                defAtrybut.Nazwa = name;
                defAtrybut.Format = format;
                defAtrybut.Typ = type;
            }
            catch(Exception e)
            {
                throw new Exception("Błąd przy tworzeniu definicji atrybutu " + e.Message);
            }
        }

        private void RefreshEnvironmentPath()
        {
            try
            {
                System.Environment.CurrentDirectory = path;
            }
            catch (Exception)
            {
                throw new Exception("Brak dostępu do środowiska systemu");
            }
        }
    }
}
