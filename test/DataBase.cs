using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace test
{
    public class DataBase
    {
        public DataBase()
        {
            sql = new SqlConnection(@"Data Source=HP-PAVILION;Initial catalog=point;Integrated Security=true");
        }
        private SqlConnection sql;
        public SqlConnection Sql
        {
            get { return sql; }
           // set { sql = value; }
        }
        public void OpenConnection()
        {
            if (sql.State == System.Data.ConnectionState.Closed)
            {
                sql.Open();
            }
        }
        
        public void CloseConnection()
        {
            if (sql.State == System.Data.ConnectionState.Open)
            {
                sql.Close();
            }
        }

    }
}
