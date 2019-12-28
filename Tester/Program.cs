using System;
using System.Data;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            PROSql.SQL sql = new PROSql.SQL(Properties.Settings.Default.SQLServerName,
                                               Properties.Settings.Default.SQLDatabase,
                                               Properties.Settings.Default.SQLUsername,
                                               Properties.Settings.Default.SQLPassword,
                                               Properties.Settings.Default.SQLNT);
            sql.Connect();
            PROSql.PROSQLParam[] sqlParams = { };
            DataTable dataTable = sql.Execute2("CDN.PROKntWeryfStatHistErr", sqlParams);

            foreach (DataRow row in dataTable.Rows)
            {
                Console.WriteLine(row);
            }
            sql.Disconnect();

            // [CDN].[PROKntWeryfStatHistErr]
        }
    }
}
