using System;
using System.Data;
using System.Data.SqlClient;

namespace PROXL
{
    public class XL
    {
        protected static int session = 0;
        protected string path;

        public XL(string path)
        {
            this.path = path;
        }

        public void Login(string oper, string pass, string _base, string key)
        {
            try
            {
                RefreshEnvironmentPath();

                cdn_api.XLLoginInfo_20171 loginInfo = new cdn_api.XLLoginInfo_20171
                {
                    Wersja = 20171,
                    ProgramID = "SynchMag",
                    UtworzWlasnaSesje = 1,
                    Winieta = -1,
                    TrybWsadowy = 1,
                    TrybNaprawy = 1,
                    Baza = _base,
                    SerwerKlucza = key,
                    OpeIdent = oper,
                    OpeHaslo = pass
                };

                switch (cdn_api.cdn_api.XLLogin(loginInfo, ref session))
                {
                    case -8:
                        throw new Exception("Nie podano nazwy bazy.");
                    case -7:
                        throw new Exception("Baza niezarejestrowana w systemie.");
                    case -6:
                        throw new Exception("Nie podano hasła lub brak operatora.");
                    case -5:
                        throw new Exception("Nieprawidłowe hasło.");
                    case -4:
                        throw new Exception("Konto operatora zablokowane.");
                    case -3:
                        throw new Exception("Nie podano nazwy programu.");
                    case -2:
                        throw new Exception("Błąd otwarcia pliku tekstowego, do którego mają być zapisywane komunikaty.Nie znaleziono ścieżki lub nazwa pliku jest nieprawidłowa.");
                    case -1:
                        throw new Exception("Podano niepoprawną wersję API.");
                    case 0:
                        break;
                    case 1:
                        throw new Exception("Inicjalizacja nie powiodła się.");
                    case 2:
                        throw new Exception("Istnieje już jedna instancja programu i nastąpiło ponowne logowanie z tego samego komputera i na tego samego operatora.");
                    case 3:
                        throw new Exception("Istnieje już jedna instancja programu i nastąpiło ponowne logowanie z innego komputera i na tego samego operatora, ale operator nie posiada prawa do wielokrotnego logowania.");
                    case 5:
                        throw new Exception("Pracy terminalowej, operator nie ma prawa do wielokrotnego logowania");
                    case 61:
                        throw new Exception("Błąd zakładania nowej sesji");
                    default:
                        throw new Exception("Nieznany błąd!");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Wystąpił błąd przy logowaniu do ERP XL: " + e.Message);
            }
        }

        public void Logout()
        {
            try
            {
                RefreshEnvironmentPath();

                switch (cdn_api.cdn_api.XLLogout(session))
                {
                    case -2:
                        throw new Exception("Błąd otwarcia pliku tekstowego, do którego mają być zapisywane komunikaty.Nie znaleziono ścieżki lub nazwa pliku jest nieprawidłowa.");
                    case -1:
                        throw new Exception("Nie zalogowano");
                    case 0:
                        break;
                    case 2:
                        throw new Exception("Istnieje już jedna instancja programu i nastąpiło ponowne logowanie z tego samego komputera i na tego samego operatora.");
                    case 3:
                        throw new Exception("Istnieje już jedna instancja programu i nastąpiło ponowne logowanie z innego komputera i na tego samego operatora, ale operator nie posiada prawa do wielokrotnego logowania.");
                    case 5:
                        throw new Exception("Pracy terminalowej, operator nie ma prawa do wielokrotnego logowania");
                    default:
                        throw new Exception("Nieznany błąd!");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Wystąpił błąd przy wylogowywaniu z ERP XL: " + e.Message);
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
