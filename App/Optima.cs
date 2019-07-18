using System;

namespace ProfessoftApps
{
    public class Optima
    {
        protected static CDNBase.Application application = null;
        protected static CDNBase.ILogin login = null;
        protected static CDNBase.AdoSession session = null;
        protected string path;

        public Optima(string path)
        {
            this.path = path;
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
            }
            catch (Exception e)
            {
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
            }
            catch (Exception e)
            {
                throw new Exception("Wystąpił błąd przy wylogowywaniu z ERP Optima: " + e.Message);
            }
        }

        /*
        public void Login(string oper, string pass, string company)
        {
            try
            {
                RefreshEnvironmentPath();
                login = new LoginService();
                ModuleCollection mc = new ModuleCollection() { Module.KasaBankPlus, Module.Handel, Module.CRMPlus, Module.KsiegaHandlowaPlus, Module.KadryPlace };
                login.Login(oper, pass, company, mc);
                session = login.LoginInfo.CreateSession();

            }
            catch (Exception e)
            {
                throw new Exception("Wystąpił błąd przy logowaniu do ERP Optima: " + e.Message);
            }
        }

        public void LogOut()
        {
            try
            {
                RefreshEnvironmentPath();
                login.Logout();
                session = null;
            }
            catch (Exception e)
            {
                throw new Exception("Wystąpił błąd przy wylogowywaniu z ERP Optima: " + e.Message);
            }
        }
        */

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

        public CDNHeal.Kontrahent GetContractorByName(string name)
        {
            CDNBase.ICollection kontrahenci = session.CreateObject("CDN.Kontrahenci", null);

            foreach (CDNHeal.Kontrahent kontrahent in kontrahenci)
            {
                if (kontrahent.Akronim.Equals(name.ToUpper()))
                    return kontrahent;
            }
            throw new Exception("Nie znaleziono kontrahenta o akronimie: " + name);
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
