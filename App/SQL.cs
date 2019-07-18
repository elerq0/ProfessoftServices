using System;
using System.Data;
using System.Data.SqlClient;

namespace ProfessoftApps
{
    public class SQL
    {
        protected SqlConnection cnn;
        public SQL(string serverName, string database, string userName, string pass)
        {
            cnn = new SqlConnection("Data Source = " + serverName + "; Initial Catalog = " + database + "; User ID = " + userName + "; Password = " + pass);
        }

        public void Connect()
        {
            try
            {
                cnn.Open();
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
                cnn.Close();
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
