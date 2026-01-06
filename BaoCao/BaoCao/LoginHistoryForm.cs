using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Nhom8
{
    public partial class LoginHistoryForm : Form
    {

        private string _userId;
        public LoginHistoryForm(string userId, string username)
        {
            InitializeComponent();
            CenterToScreen();
            _userId = userId;
            this.Text = $"Lịch sử Đăng nhập - {username}";
            this.Load += new System.EventHandler(this.LoginHistoryForm_Load);
        }
        private void LoginHistoryForm_Load(object sender, EventArgs e)
        {
            LoadHistoryData();
        }

        private void LoadHistoryData()
        {
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    //GIAI MA HOA LAI
                    string sql = @"SELECT LOGIN_TIME, 
                                          NHOM8.PKG_LOG_SECURITY.DECRYPT_IP_HYBRID(IP_ENCRYPTED, SESSION_KEY_ENC) AS IP_ADDRESS 
                                   FROM LOGIN_HISTORY 
                                   WHERE USERID = :userId 
                                   ORDER BY LOGIN_TIME DESC";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("userId", _userId));

                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvHistory.DataSource = dt;
                        if (dgvHistory.Columns["LOGIN_TIME"] != null)
                            dgvHistory.Columns["LOGIN_TIME"].HeaderText = "Thời gian Đăng nhập";
                        if (dgvHistory.Columns["IP_ADDRESS"] != null)
                            dgvHistory.Columns["IP_ADDRESS"].HeaderText = "Địa chỉ IP (Giải mã Lai)";
                        dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lịch sử đăng nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LoginHistoryForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}