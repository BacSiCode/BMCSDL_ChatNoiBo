using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Nhom8
{
    public partial class QuenMatKhau : Form
    {
        public QuenMatKhau()
        {
            InitializeComponent();
            CenterToScreen();
            // Ẩn các control lúc đầu
            labelOTP.Visible = false;
            txtOTP.Visible = false;
            txtNewPass.Visible = false;
            txtConfirmPass.Visible = false;
            btnDoiMK.Visible = false;
            labelNewpassword.Visible = false;
            labelConfirmPass.Visible = false;

            txtNewPass.DefaultText = "";
            txtConfirmPass.DefaultText = "";
        }

        public static class OTPStore
        {
            public static string OTP = "";
            public static string UserID = "";
        }

        private void guna2Button_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        // --- BƯỚC 1: TÌM USER VÀ GỬI OTP ---
        private void guna2Button_resetPassword_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Vui lòng nhập Username và Số điện thoại");
                return;
            }

            string userId = "";
            string dbPhone = "";

            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    // SỬA 1: Không mã hóa Username ở C# nữa. Gửi username thô để tìm.
                    // XÓA: string encryptedUsernameToSearch = BaoCao.SecurityHelper.MaHoaUsername.MaHoaUsernameAES(username);

                    string sql = "SELECT USERID, DIENTHOAI FROM TAIKHOAN WHERE USERNAME = :u";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("u", OracleDbType.Varchar2).Value = username; // Gửi thô

                        using (OracleDataReader rd = cmd.ExecuteReader())
                        {
                            if (!rd.Read())
                            {
                                MessageBox.Show("Không tìm thấy Username!");
                                return;
                            }
                            userId = rd["USERID"].ToString();
                            dbPhone = rd["DIENTHOAI"].ToString();
                        }
                    }
                }

                // Kiểm tra số điện thoại (Giả sử SĐT trong DB đang lưu thô. 
                // Nếu SĐT trong DB đã mã hóa AES, bạn cần Decrypt nó ra mới so sánh được)
                if (dbPhone != phone)
                {
                    MessageBox.Show("Số điện thoại không đúng với tài khoản này!");
                    return;
                }

                // Sinh OTP và giả lập gửi
                string otp = new Random().Next(100000, 999999).ToString();
                OTPStore.OTP = otp;
                OTPStore.UserID = userId;

                MessageBox.Show($"Mã OTP của bạn là: {otp}.", "Mã OTP", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Ẩn phần nhập User/Phone, Hiện phần nhập Pass mới
                guna2HtmlLabel3.Visible = false;
                txtUsername.Visible = false;
                guna2HtmlLabel2.Visible = false;
                txtPhone.Visible = false;
                btnXacNhan.Visible = false;

                labelOTP.Visible = true;
                txtOTP.Visible = true;
                txtNewPass.Visible = true;
                txtConfirmPass.Visible = true;
                btnDoiMK.Visible = true;
                labelNewpassword.Visible = true;
                labelConfirmPass.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        // --- BƯỚC 2: ĐỔI MẬT KHẨU ---
        private void btnDoiMK_Click(object sender, EventArgs e)
        {
            string otp = txtOTP.Text.Trim();
            string newPass = txtNewPass.Text.Trim();
            string confirm = txtConfirmPass.Text.Trim();

            if (string.IsNullOrEmpty(otp) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirm))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (otp != OTPStore.OTP)
            {
                MessageBox.Show("OTP không đúng!");
                return;
            }

            if (newPass != confirm)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                return;
            }

            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string sql = "UPDATE TAIKHOAN SET PASSWORD = PKG_SECURITY.Encrypt_AES(:p) WHERE USERID = :id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("p", OracleDbType.Varchar2).Value = newPass; // Gửi Pass thô
                        cmd.Parameters.Add("id", OracleDbType.Varchar2).Value = OTPStore.UserID;

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Đổi mật khẩu thành công!");
                OTPStore.OTP = "";
                OTPStore.UserID = "";

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật mật khẩu: " + ex.Message);
            }
        }

        private void txtUsername_TextChanged(object sender, EventArgs e) { }
        private void txtPhone_TextChanged(object sender, EventArgs e) { }
        private void txtNewPass_TextChanged(object sender, EventArgs e) { }
    }
}