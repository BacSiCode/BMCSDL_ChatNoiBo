using Oracle.ManagedDataAccess.Client;
using System.Data;
namespace Nhom8
{
    public partial class DangNhap : Form
    {
        public DangNhap()
        {
            InitializeComponent();
            CenterToScreen();
        }

        bool check_textBox()
        {
            if (guna2TextBox_username.Text == "" || guna2TextBox_password.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void DangNhap_Load(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel_forgotpassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            QuenMatKhau quenMatKhau = new QuenMatKhau();
            quenMatKhau.ShowDialog();
        }

        private void linkLabel_createAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DangKy dangKy = new DangKy();
            dangKy.ShowDialog();
        }

        private void guna2TextBox_username_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox_password_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button_login_Click(object sender, EventArgs e)
        {
            if (!check_textBox()) return;
            string user = guna2TextBox_username.Text.Trim();
            string pass = guna2TextBox_password.Text.Trim();
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string sql = @"SELECT USERID, PASSWORD, HOTEN, MAPHONGBAN, TRANGTHAI, CHUC_VU 
                                   FROM TAIKHOAN 
                                   WHERE USERNAME = :p_user 
                                   AND PASSWORD = PKG_SECURITY.Encrypt_AES(:p_pass)";

                    OracleCommand cmd = new OracleCommand(sql, conn);
                    cmd.Parameters.Add(new OracleParameter("p_user", user));
                    cmd.Parameters.Add(new OracleParameter("p_pass", pass));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string trangThai = reader["TRANGTHAI"].ToString();
                            if (trangThai == "LOCKED")
                            {
                                MessageBox.Show("Tài khoản của bạn đã bị KHÓA!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            string userId = reader["USERID"].ToString();
                            string hoTen = reader["HOTEN"].ToString();
                            string chucVu = reader["CHUC_VU"].ToString();

                            // --- BẮT ĐẦU ĐOẠN MÃ HÓA LAI IP ---
                            try
                            {
                                string ip = new System.Net.WebClient().DownloadString("https://api.ipify.org");
                                using (OracleConnection logConn = BaoCao.DataBase.GetConnection())
                                {
                                    if (logConn.State != ConnectionState.Open) logConn.Open();

                                    // Gọi Procedure Mã Hóa Lai thay vì Insert thường
                                    string sqlLog = "BEGIN NHOM8.PKG_LOG_SECURITY.INSERT_LOG_HYBRID(:u, :ip); END;";

                                    using (OracleCommand logCmd = new OracleCommand(sqlLog, logConn))
                                    {
                                        logCmd.Parameters.Add(new OracleParameter("u", userId));
                                        logCmd.Parameters.Add(new OracleParameter("ip", ip));
                                        logCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            catch (Exception exLog)
                            {
                                // Có thể log lỗi ra file hoặc bỏ qua để không chặn đăng nhập
                                Console.WriteLine("Lỗi log IP: " + exLog.Message);
                            }
                            // --- KẾT THÚC ĐOẠN MÃ HÓA LAI IP ---

                            UserSession.UserID = userId;
                            UserSession.HoTen = hoTen;
                            MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (chucVu == "ADMIN")
                            {
                                SupAdmin adminDashboard = new SupAdmin();
                                adminDashboard.Show();
                            }
                            else if (chucVu == "LEAD")
                            {
                                HomeADMIN adminChatForm = new HomeADMIN();
                                adminChatForm.Show();
                            }
                            else
                            {
                                Home userChatForm = new Home();
                                userChatForm.Show();
                            }

                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void guna2TextBox_password_IconRightClick(object sender, EventArgs e)
        {
            if (guna2TextBox_password.UseSystemPasswordChar == true)
            {
                guna2TextBox_password.UseSystemPasswordChar = false;
                guna2TextBox_password.PasswordChar = '\0';
                guna2TextBox_password.IconRight = Properties.Resources.eye_open;
            }
            else
            {
                guna2TextBox_password.UseSystemPasswordChar = true;
                guna2TextBox_password.IconRight = Properties.Resources.eye_close;
            }
        }
    }
}