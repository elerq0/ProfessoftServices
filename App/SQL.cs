using System;
using System.Data;
using System.Data.SqlClient;

namespace ProfessoftApps
{
    public class SQL
    {
        protected SqlConnection cnn;
        public SQL(string servername, string database, string username, string password, Boolean NT)
        {
            if (!NT)
                cnn = new SqlConnection("Data Source = " + servername + "; Initial Catalog = " + database + "; User ID = " + username + "; Password = " + password);
            else
                cnn = new SqlConnection("Data Source = " + servername + "; Initial Catalog = " + database + "; Integrated Security=true;");
        }

        public Boolean Connect()
        {

            try
            {
                if ((int)cnn.State == 0)
                {
                    cnn.Open();
                    return true;
                }
                return false;
            }
            catch (SqlException)
            {
                throw new Exception("Nie można nawiązać połączenia z SQL: nie można odnaleźć serwera lub jest on niedostępny.");
            }

        }

        public void Disconnect()
        {
            try
            {
                if ((int)cnn.State == 1)
                {
                    cnn.Close();
                }
            }
            catch (SqlException)
            {
                throw new Exception("Wystąpił błąd podczas zamykania połączenia z serwerem SQL");
            }
        }

        public DataTable Execute(string sql)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(sql, cnn);
                SqlDataReader dataReader;
                try
                {
                    dataReader = cmd.ExecuteReader();
                }
                catch (SqlException)
                {
                    throw new Exception("Błędna treść zapytania.");
                }

                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);

                dataReader.Close();
                cmd.Dispose();

                return dataTable;
            }
            catch (Exception e)
            {
                throw new Exception("Błąd w wykonaniu polecenia sql: '" + sql + "', " + e.Message);
            }
        }
    }
}
