using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Nhom8
{
    public partial class Form_CapNhatTaiKhoan : Form
    {
        private string _userId;
        public Form_CapNhatTaiKhoan(string userId, string username)
        {
            InitializeComponent();
            CenterToScreen();
            _userId = userId;
            txtUsernameReadOnly.Text = $"CẬP NHẬT TÀI KHOẢN: [{username}]";
            this.Load += new System.EventHandler(this.Form_CapNhatTaiKhoan_Load);
            this.btnSaveInfo.Click += new System.EventHandler(this.btnLuuThongTin_Click);
            this.btnResetPassword.Click += new System.EventHandler(this.btnDoiMatKhau_Click);
        }

        private void Form_CapNhatTaiKhoan_Load(object sender, EventArgs e)
        {
            try
            {
                LoadPhongBanList();
                cmbGioiTinh.Items.Clear();
                cmbGioiTinh.Items.Add("Nam");
                cmbGioiTinh.Items.Add("Nữ");
                cmbGioiTinh.Items.Add("Khác");
                LoadUserInfo();
            }
            catch { }
        }

        private void LoadUserInfo()
        {
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string sql = "SELECT USERNAME, HOTEN, EMAIL, DIENTHOAI, MAPHONGBAN, GIOITINH FROM TAIKHOAN WHERE USERID = :id";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("id", _userId));

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                username.Text = reader["USERNAME"].ToString();
                                txtHoTen.Text = reader["HOTEN"].ToString();
                                txtEmail.Text = reader["EMAIL"].ToString();
                                txtDienThoai.Text = reader["DIENTHOAI"].ToString();
                                cmbPhongBan.SelectedValue = reader["MAPHONGBAN"];
                                cmbGioiTinh.Text = reader["GIOITINH"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin User: " + ex.Message, "Lỗi");
            }
        }

        private void LoadPhongBanList()
        {
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    string sql = "SELECT MAPHONGBAN, TENPHONGBAN FROM PHONGBAN ORDER BY TENPHONGBAN";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        cmbPhongBan.DataSource = dt;
                        cmbPhongBan.ValueMember = "MAPHONGBAN";
                        cmbPhongBan.DisplayMember = "TENPHONGBAN";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách phòng ban: " + ex.Message, "Lỗi");
            }
        }

        private void btnLuuThongTin_Click(object sender, EventArgs e)
        {
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    string sql = @"UPDATE TAIKHOAN SET 
                                    HOTEN = :hoten, 
                                    EMAIL = :email, 
                                    DIENTHOAI = :dienthoai, 
                                    MAPHONGBAN = :mapb,
                                    GIOITINH = :gioitinh
                                WHERE USERID = :id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("hoten", txtHoTen.Text.Trim()));
                        cmd.Parameters.Add(new OracleParameter("email", txtEmail.Text.Trim()));
                        cmd.Parameters.Add(new OracleParameter("dienthoai", txtDienThoai.Text.Trim()));
                        cmd.Parameters.Add(new OracleParameter("mapb", cmbPhongBan.SelectedValue));
                        cmd.Parameters.Add(new OracleParameter("gioitinh", cmbGioiTinh.Text));
                        cmd.Parameters.Add(new OracleParameter("id", _userId));

                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Cập nhật thông tin tài khoản thành công!", "Thành công");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi");
            }
        }

        // --- PHẦN SỬA ĐỔI QUAN TRỌNG: LOGIC ĐỔI MẬT KHẨU ---
        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            string newPass = txtNewPassword.Text;
            string confirmPass = txtConfirmPassword.Text;
            if (string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu mới và xác nhận.", "Thiếu thông tin");
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Lỗi");
                return;
            }
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string sql = "UPDATE TAIKHOAN SET PASSWORD = PKG_SECURITY.Encrypt_AES(:rawPass) WHERE USERID = :id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("rawPass", newPass));
                        cmd.Parameters.Add(new OracleParameter("id", _userId));

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Đặt lại mật khẩu thành công!", "Thành công");
                txtNewPassword.Clear();
                txtConfirmPassword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật mật khẩu: " + ex.Message, "Lỗi");
            }
        }

        private void guna2GroupBox1_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel7_Click(object sender, EventArgs e) { }
        private void btnCancel_Click(object sender, EventArgs e) { this.Close(); }
        private void grpResetPassword_Click(object sender, EventArgs e) { }
        private void txtDienThoai_TextChanged(object sender, EventArgs e) { }
    }
}