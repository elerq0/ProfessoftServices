using System;
using System.Data;
using System.Data.SqlClient;

namespace PROSql
{
    public class SQL
    {
        private SqlConnection cnn;
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
            catch (SqlException e)
            {
                throw new Exception("Nie można nawiązać połączenia z SQL: nie można odnaleźć serwera lub jest on niedostępny." + e.Message);
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

        public Object ExecuteFunction(string sql, PROSQLParam[] args)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    foreach (PROSQLParam arg in args)
                    {
                        SqlParameter param = new SqlParameter(arg.name, arg.type, arg.size)
                        {
                            Value = arg.value,
                            Direction = arg.direction
                        };
                        cmd.Parameters.Add(param);
                    }

                    return cmd.ExecuteScalar();
                }
            }
            catch(Exception e)
            {
                throw new Exception("sql error: " + e.Message);
            }
        }

        public DataTable Execute2(string commandName, PROSQLParam[] argsInput)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = cnn,
                    CommandText = commandName,
                    CommandType = CommandType.StoredProcedure
                })
                {
                    foreach (PROSQLParam arg in argsInput)
                    {
                        SqlParameter sqlParameter = new SqlParameter(arg.name, arg.type, 1)
                        {
                            Value = arg.value
                        };
                        sqlCommand.Parameters.Add(sqlParameter);
                    }

                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(dataReader);

                        dataReader.Close();
                        sqlCommand.Dispose();
                        return dataTable;
                    }

                }
            }
            catch(Exception e)
            {
                throw new Exception("Sql error: " + e.Message);
            }
        }

        public string ExecuteFunction(string commandName, PROSQLParam[] argsInput, PROSQLParam[] argsOutput)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand
                {
                    Connection = cnn,
                    CommandText = commandName,
                })
                {
                    foreach (PROSQLParam arg in argsInput)
                    {
                        SqlParameter sqlParameter = new SqlParameter(arg.name, arg.type, arg.size)
                        {
                            Value = arg.value,
                            Direction = ParameterDirection.Input
                        };
                        sqlCommand.Parameters.Add(sqlParameter);
                    }

                    foreach (PROSQLParam arg in argsOutput)
                    {
                        SqlParameter sqlParameter = new SqlParameter(arg.name, arg.type, arg.size)
                        {
                            Value = arg.value,
                            Direction = ParameterDirection.Output
                        };
                        sqlCommand.Parameters.Add(sqlParameter);
                    }

                    return sqlCommand.ExecuteScalar().ToString();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Sql error: " + e.Message);
            }
        }

    }






    public struct PROSQLParam
    {
        public string name;
        public SqlDbType type;
        public string value;
        public int size;
        public ParameterDirection direction;
    }
}
