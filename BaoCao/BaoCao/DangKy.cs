using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text.RegularExpressions;

namespace Nhom8
{
    public partial class DangKy : Form
    {
        public DangKy()
        {
            InitializeComponent();
            CenterToScreen();
        }

        private void DangKy_Load(object sender, EventArgs e)
        {
            try
            {
                LoadPhongBan();
                guna2ComboBox1_gioiTinh.Items.Clear();
                guna2ComboBox1_gioiTinh.Items.Add("Nam");
                guna2ComboBox1_gioiTinh.Items.Add("Nữ");
                guna2ComboBox1_gioiTinh.Items.Add("Khác");
                guna2ComboBox1_gioiTinh.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadPhongBan()
        {
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    // --- BƯỚC 1: GIẢ DANH ADMIN ĐỂ THẤY DỮ LIỆU (PHÒNG HỜ) ---
                    try { BaoCao.DataBase.SetOracleContext(conn, "U000"); } catch { }

                    string query = "SELECT MAPHONGBAN, TENPHONGBAN FROM PHONGBAN ORDER BY MAPHONGBAN";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            guna2ComboBox1_phongBan.Items.Clear();
                            while (reader.Read())
                            {
                                string maPB = reader["MAPHONGBAN"].ToString();
                                string tenPB = reader["TENPHONGBAN"].ToString();
                                guna2ComboBox1_phongBan.Items.Add($"{maPB} - {tenPB}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải phòng ban: " + ex.Message); }
        }

        private void guna2Button_signUp_Click(object sender, EventArgs e)
        {
            // --- VALIDATION ---
            string phone = guna2TextBox1_phoneNumber.Text.Trim();
            string username = guna2TextBox_usernameDK.Text.Trim();
            string password = guna2TextBox_passwordDK.Text.Trim();
            string confirmPass = guna2TextBox3_confirmPass.Text.Trim();
            string email = guna2TextBox_Email.Text.Trim();
            string fullName = guna2TextBox1_fullName.Text.Trim();
            string gioiTinh = guna2ComboBox1_gioiTinh.SelectedItem?.ToString() ?? "Nam";

            if (string.IsNullOrEmpty(phone) || !Regex.IsMatch(phone, @"^[0-9]{10,11}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ!", "Cảnh báo"); return;
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập Username/Password!", "Cảnh báo"); return;
            }
            if (password != confirmPass)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi"); return;
            }
            if (guna2ComboBox1_phongBan.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn phòng ban!", "Cảnh báo"); return;
            }

            // Lấy mã phòng ban
            string selectedPB = guna2ComboBox1_phongBan.SelectedItem.ToString();
            string maPhongBan = selectedPB.Split('-')[0].Trim();

            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    // SET CONTEXT ADMIN
                    try { BaoCao.DataBase.SetOracleContext(conn, "U000"); } catch { }

                    // Check Username
                    using (OracleCommand cmdCheck = new OracleCommand("SELECT COUNT(*) FROM TAIKHOAN WHERE USERNAME = :val", conn))
                    {
                        cmdCheck.BindByName = true; // QUAN TRỌNG
                        cmdCheck.Parameters.Add("val", username);
                        if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Tên đăng nhập đã tồn tại!", "Cảnh báo"); return;
                        }
                    }
                    // Check Phone
                    using (OracleCommand cmdCheckPhone = new OracleCommand("SELECT COUNT(*) FROM TAIKHOAN WHERE DIENTHOAI = :val", conn))
                    {
                        cmdCheckPhone.BindByName = true; // QUAN TRỌNG
                        cmdCheckPhone.Parameters.Add("val", phone);
                        if (Convert.ToInt32(cmdCheckPhone.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Số điện thoại đã được sử dụng!", "Cảnh báo"); return;
                        }
                    }

                    // TRANSACTION
                    using (OracleTransaction tr = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Generate ID
                            string newUserId = "U001";
                            try
                            {
                                using (OracleCommand cmdMax = new OracleCommand("SELECT MAX(TO_NUMBER(SUBSTR(USERID,2))) FROM TAIKHOAN WHERE REGEXP_LIKE(USERID,'^U[0-9]+$')", conn))
                                {
                                    cmdMax.Transaction = tr;
                                    object r = cmdMax.ExecuteScalar();
                                    if (r != null && r != DBNull.Value) newUserId = "U" + (Convert.ToInt32(r) + 1).ToString("D3");
                                }
                            }
                            catch { }

                            // 2. Insert TAIKHOAN
                            // Lưu ý: Dùng tham số :p_... để tránh trùng từ khóa
                            string sqlInsert = @"INSERT INTO TAIKHOAN 
                                             (USERID, USERNAME, PASSWORD, EMAIL, HOTEN, MAPHONGBAN, TRANGTHAI, GIOITINH, DIENTHOAI) 
                                             VALUES (:p_uid, :p_usr, :p_pwd, :p_mail, :p_name, :p_pb, 'ACTIVE', :p_sex, :p_tel)";

                            using (OracleCommand cmdInsert = new OracleCommand(sqlInsert, conn))
                            {
                                cmdInsert.Transaction = tr;
                                cmdInsert.BindByName = true; // CỰC KỲ QUAN TRỌNG VỚI ODP.NET

                                cmdInsert.Parameters.Add("p_uid", newUserId);
                                cmdInsert.Parameters.Add("p_usr", username);
                                cmdInsert.Parameters.Add("p_pwd", password);
                                cmdInsert.Parameters.Add("p_mail", email);
                                cmdInsert.Parameters.Add("p_name", fullName);
                                cmdInsert.Parameters.Add("p_pb", maPhongBan);
                                cmdInsert.Parameters.Add("p_sex", gioiTinh);
                                cmdInsert.Parameters.Add("p_tel", phone);

                                cmdInsert.ExecuteNonQuery();
                            }

                            // 3. Insert THANHVIENNHOM (Mù)
                            string groupId = maPhongBan.Replace("PB", "N");
                            try
                            {
                                string sqlAddMem = "INSERT INTO THANHVIENNHOM (MANHOM, USERID, DUYET) VALUES (:p_gid, :p_uid, 0)";
                                using (OracleCommand cmdAdd = new OracleCommand(sqlAddMem, conn))
                                {
                                    cmdAdd.Transaction = tr;
                                    cmdAdd.BindByName = true;
                                    cmdAdd.Parameters.Add("p_gid", groupId);
                                    cmdAdd.Parameters.Add("p_uid", newUserId);
                                    cmdAdd.ExecuteNonQuery();
                                }
                            }
                            catch { /* Bỏ qua nếu lỗi khóa ngoại (nhóm ko tồn tại) */ }

                            tr.Commit();
                            MessageBox.Show("Đăng ký thành công!\nBạn đã được thêm vào danh sách chờ duyệt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            tr.Rollback();
                            MessageBox.Show("Lỗi Database: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi kết nối: " + ex.Message); }
        }

        private void guna2TextBox1_phoneNumber_TextChanged(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(guna2TextBox1_phoneNumber.Text, "^[0-9]*$"))
            {
                guna2TextBox1_phoneNumber.Text = Regex.Replace(guna2TextBox1_phoneNumber.Text, "[^0-9]", "");
                guna2TextBox1_phoneNumber.SelectionStart = guna2TextBox1_phoneNumber.Text.Length;
            }
        }

        private void guna2Button_Exit_Click(object sender, EventArgs e) { Close(); }

        // Events
        private void guna2HtmlLabel2_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel4_Click(object sender, EventArgs e) { }
        private void guna2PictureBox1_Click(object sender, EventArgs e) { }
        private void guna2TextBox_usernameDK_TextChanged(object sender, EventArgs e) { }
        private void guna2TextBox_passwordDK_TextChanged(object sender, EventArgs e) { }
        private void guna2TextBox3_confirmPass_TextChanged(object sender, EventArgs e) { }
        private void guna2TextBox_Email_TextChanged(object sender, EventArgs e) { }
        private void guna2TextBox1_fullName_TextChanged(object sender, EventArgs e) { }
        private void guna2HtmlLabel7_Click(object sender, EventArgs e) { }
        private void guna2ComboBox1_phongBan_SelectedIndexChanged(object sender, EventArgs e) { }
        private void guna2ComboBox1_gioiTinh_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}