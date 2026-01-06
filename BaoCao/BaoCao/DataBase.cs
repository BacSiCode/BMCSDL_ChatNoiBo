using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BaoCao
{
    public class DataBase
    {
        public static OracleConnection Conn;
        public static string Host;
        public static string Port;
        public static string User;
        public static string Password;
        public static string Sid;

        public static void Set_Database(string host, string port, string sid, string user, string password)
        {
            DataBase.Host = host;
            DataBase.Port = port;
            DataBase.Sid = sid;
            DataBase.User = user;
            DataBase.Password = password;

        }

        public static bool Connect()
        {
            try
            {
                string connsys = "";
                if (User.ToUpper().Equals("SYS"))
                {
                    connsys = ";DBA Privilege=SYSDBA";
                }

                string connString =
                    "User Id=" + User +
                    ";Password=" + Password +
                    ";Data Source=localhost:1521/orcl" + connsys;

                Conn = new OracleConnection(connString);
                Conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi kết nối với Oracle: " + ex.Message);
                return false;
            }
        }

        public static OracleConnection GetConnection()
        {
            if (string.IsNullOrEmpty(Host)) Host = "localhost";
            if (string.IsNullOrEmpty(Port)) Port = "1521";
            if (string.IsNullOrEmpty(Sid)) Sid = "orcl";
            if (string.IsNullOrEmpty(User)) User = "nhom8";
            if (string.IsNullOrEmpty(Password)) Password = "123";

            string connString = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={Host})(PORT={Port}))"
                              + $"(CONNECT_DATA=(SERVICE_NAME={Sid})));"
                              + $"User Id={User};Password={Password};";

            OracleConnection conn = new OracleConnection(connString);
            conn.Open();
            return conn;
        }

        public static void SetOracleContext(OracleConnection conn, string userId)
        {
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                string sql = "BEGIN PKG_SESSION_CTX.SET_USER_ID(:uid); END;";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("uid", userId));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi set context VPD: " + ex.Message);
            }
        }

    }
}
