using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BaoCao
{
    public static class SecurityHelper
    {

        public static string Encrypt_Oracle(string rawData)
        { 
            if (string.IsNullOrEmpty(rawData)) return null;

            string result = "";
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string sql = "SELECT PKG_SECURITY.Encrypt_AES(:input) FROM DUAL";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("input", rawData));

                        object val = cmd.ExecuteScalar();
                        if (val != null && val != DBNull.Value)
                        {
                            result = val.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mã hóa từ Oracle: " + ex.Message);
            }
            return result;
        }

        public static string Decrypt_Oracle(string encryptedData)
        {
            if (string.IsNullOrEmpty(encryptedData)) return null;

            string result = "";
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    string sql = "SELECT PKG_SECURITY.Decrypt_AES(:input) FROM DUAL";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("input", encryptedData));

                        object val = cmd.ExecuteScalar();
                        if (val != null && val != DBNull.Value)
                        {
                            result = val.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return encryptedData;
            }
            return result;
        }


    }
}