using System;

namespace PROOptima
{
    public class Optima
    {
        protected CDNBase.Application application = null;
        protected CDNBase.ILogin login = null;
        private OptimaSession optimaSession;
        protected string path;

        public int operatorID;
        public string operatorKod;
        public Boolean connected;
        private Boolean sessionRefresh;

        public Optima(string path, Boolean sessionRefresh)
        {
            this.path = path;
            connected = false;
            this.sessionRefresh = sessionRefresh;
        }

        public Boolean Login(string oper, string pass, string company)
        {
            object[] hPar = new object[] {0, // Księga_podatkowa
                                          0, // Księga_handlowa
                                          0, // Księga_handlowa_plus
                                          0, // Środki_trwale
                                          0, // Faktury
                                          1, // MAG?? Magazyny
                                          0, // Płace_i_kadry
                                          0, // Płace_i_kadry_xl?????
                                          0, // CRM
                                          0, // Analizy
                                          0, // DET????
                                          0, // BIU????
                                          0, // Serwis
                                          0, // Obieg_dokumentow
                                          1, // Kasa_bank
                                          0, // Kasa_bank_plus
                                          1, // Handel_plus
                                          0};// CRM_plus

            if (GetState() == 0)
            {
                try
                {
                    RefreshEnvironmentPath();
                    application = new CDNBase.Application();
                    application.LockApp(513, 5000, null, null, null, null);
                    login = application.Login(oper, pass, company, hPar[0], hPar[1], hPar[2], hPar[3], hPar[4], hPar[5], hPar[6], hPar[7], hPar[8], hPar[9], hPar[10], hPar[11], hPar[12], hPar[13], hPar[14], hPar[15], hPar[16], hPar[17]);
                    optimaSession = new OptimaSession(login, sessionRefresh);
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
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LogOut()
        {
            if (GetState() != 0)
            {
                try
                {
                    RefreshEnvironmentPath();
                    application.UnlockApp();
                    connected = false;
                }
                catch (Exception e)
                {
                    throw new Exception("Wystąpił błąd przy wylogowywaniu z ERP Optima: " + e.Message);
                }
            }
        }

        public int GetState()
        {
            if (application == null || application.HASPConnection == null)
                return 0;
            else
                return application.HASPConnection.State;
        }

        public void Save()
        {
            optimaSession.Save(true);
        }

        public void SaveWithoutSessionRefresh()
        {
            optimaSession.Save(false);
        }

        public void ForceSessionRenew()
        {
            optimaSession.ForceSessionRenew();
        }

        public CDNBase.ICollection GetContractorCollection()
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.Kontrahenci", null);
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
                return optimaSession.session.CreateObject("CDN.Kontrahenci").Item("Knt_Kod = '" + code.Replace("'", "''") + "'");
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono kontrahenta o akronimie: " + code);
            }
        }

        public CDNHeal.Kontrahent GetContractorByID(int KntId)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.Kontrahenci").Item("Knt_KntId = " + KntId);
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono kontrahenta o ID: " + KntId);
            }

        }

        public CDNHlmn.IDokumentHaMag CreateNewDocumentHaMag()
        {
            return optimaSession.session.CreateObject("CDN.DokumentyHaMag").AddNew();
        }

        public CDNHlmn.Magazyn GetWarehouseBySymbol(string mag_symbol)
        {
            try
            { 
                return optimaSession.session.CreateObject("CDN.Magazyny").Item("Mag_Symbol = '" + mag_symbol + "'");
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono magazynu o symbolu: " + mag_symbol);
            }
        }

        public CDNHeal.DefinicjaDokumentu GetNumeratorBySymbol(string num_symbol)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.DefinicjeDokumentow").Item("DDf_Symbol = '" + num_symbol + "'");
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono numeratora o symbolu: " + num_symbol);
            }
        }

        public CDNHlmn.IDokumentHaMag GetDocumentHaMagByID(string trNID)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.DokumentyHaMag").Item("TrN_TrNId = " + trNID);
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono dokkumentu HaMag o id: " + trNID);
            }
        }

        public CDNBase.ICollection GetGoodCollection()
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.Towary", null);
            }
            catch (Exception)
            {
                throw new Exception("Nie można stworzyc kolekcji towarów");
            }
        }

        public CDNHeal.Kategoria GetCategoryByCode(string kat_code)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.Kategorie").Item("Kat_KodSzczegol = '" + kat_code + "'");
            }
            catch (Exception)
            {
                throw new Exception("Nie można stworzyc kolekcji towarów");
            }
        }

        public CDNTwrb1.Towar GetGoodByCode(string twr_code)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.Towary").Item("Twr_Kod = '" + twr_code.Replace("'", "''") + "'");
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono towaru o kodzie: " + twr_code);
            }
        }

        public CDNTwrb1.Towar GetGoodByID(int TwrId)
        {
            try
            {
                
                return optimaSession.session.CreateObject("CDN.Towary").Item("Twr_TwrId = " + TwrId);
                
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono towaru o ID: " + TwrId);
            }
        }

        public CDNBase.ICollection GetCNCodeCollection()
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.KodyCN", null);
            }
            catch (Exception)
            {
                throw new Exception("Nie można stworzyc kolekcji kodów CN");
            }
        }

        public CDNTwrb1.KodCN GetCNCodeByCode(string KCNKod)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.KodyCN").Item("KCN_Kod = '" + KCNKod + "'");
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono kodu cn [" + KCNKod + "]");
            }
        }

        public OP_KASBOLib.FormaPlatnosci GetPaymentMethodbyName(string FPl_Name)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.FormyPlatnosci").Item("FPl_Nazwa = '" + FPl_Name + "'");
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono formy płatności o nazwie [" + FPl_Name + "]");
            }
        }

        public CDNTwrb1.TwrEAN GetGoodEAN(string ean)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.TwrEan").Item("TwE_EAN = '" + ean + "'");
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono ean [" + ean + "]");
            }
        }

        public CDNBase.ICollection GetGoodGroupCollection()
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.TwrGrupy", null);
            }
            catch (Exception)
            {
                throw new Exception("Nie można stworzyc kolekcji grup towarów");
            }
        }

        public CDNTwrb1.TwrGrupa GetGoodGroupByCode(string TwGKod)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.TwrGrupy").Item("TwG_Kod = '" + TwGKod + "'");
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono grupy towarowej o kodzie [" + TwGKod + "]");
            }
        }

        public CDNHeal.Waluta GetCurrencyBySymbol(string symbol)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.Waluty").Item("WNa_Symbol ='" + symbol + "'");
            }
            catch (Exception)
            {
                throw new Exception("Wystąpił błąd wyboru waluty ");
            }
        }

        public CDNBase.ICollection GetBankCollection()
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.Banki", null);
            }
            catch (Exception)
            {
                throw new Exception("Nie można stworzyć kolekcji banków");
            }
        }

        public CDNHeal.IBank GetBankByNumber(string number)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.Banki").Item("BNa_Numer ='" + number + "'");
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono banku o numerze [" + number + "]");
            }
        }

        public CDNTwrb1.IDefAtrybut GetDefAtribute(string code, int type)
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.DefAtrybuty").Item("DeA_Kod = '" + code + "' and DeA_Typ = " + type);
            }
            catch (Exception)
            {
                throw new Exception("Nie znaleziono definicji atrybutu o kodzie: " + code);
            }
        }

        public void AddOrEditAtributeDocumentHaMag(CDNHlmn.IDokumentHaMag doc, string code, string value)
        {
            Boolean found = false;
            try
            {
                foreach (CDNTwrb1.IDokAtrybut atr in doc.Atrybuty)
                {
                    if (atr.Kod.Equals(code))
                    {
                        atr.Wartosc = value;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    CDNTwrb1.IDefAtrybut defAtrybut = GetDefAtribute(code, 4);
                    CDNTwrb1.IDokAtrybut atrybut = doc.Atrybuty.AddNew();
                    atrybut.DeAID = defAtrybut.ID;
                    atrybut.Wartosc = value;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Błąd przy ustawianiu atrybutu: " + code + " = '" + value + "', " + e.Message);
            }
        }

        public CDNTwrb1.Rabat CreateRabat()
        {
            try
            {
                return optimaSession.session.CreateObject("CDN.Rabaty", null).AddNew();
            }
            catch(Exception e)
            {
                throw new Exception("Błąd przy tworzeniu rabatu " + e.Message);
            }
        }

        public void CreateDefAtribute(string name, int format, int type)
        {
            try
            {
                GetDefAtribute(name, type);
            }
            catch (Exception)
            {
                try
                {
                    CDNTwrb1.DefAtrybut defAtrybut = optimaSession.session.CreateObject("CDN.DefAtrybuty", null).AddNew();
                    defAtrybut.Kod = name;
                    defAtrybut.Nazwa = name;
                    defAtrybut.Format = format;
                    defAtrybut.Typ = type;
                }
                catch (Exception e)
                {
                    throw new Exception("Błąd przy tworzeniu definicji atrybutu " + e.Message);
                }
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
