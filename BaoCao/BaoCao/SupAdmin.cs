using Microsoft.VisualBasic;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
namespace Nhom8
{
    public partial class SupAdmin : Form
    {
        public SupAdmin()
        {
            InitializeComponent();
            CenterToScreen();
            this.Load += new System.EventHandler(this.Admin_Load);
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            // --- 1. Load Combobox User ---
            if (cmbSortUser.Items.Count == 0)
            {
                cmbSortUser.Items.Add("Tên đăng nhập (A-Z)");
                cmbSortUser.Items.Add("ID (Tăng dần)");
                cmbSortUser.Items.Add("Họ tên (A-Z)");
                cmbSortUser.SelectedIndex = 0;
            }
            cmbSortUser.SelectedIndexChanged -= cmbSortUser_SelectedIndexChanged;
            cmbSortUser.SelectedIndexChanged += cmbSortUser_SelectedIndexChanged;

            // --- 2. Load Combobox Group ---
            if (cmbSortGroup.Items.Count == 0)
            {
                cmbSortGroup.Items.Add("Mới nhất");
                cmbSortGroup.Items.Add("Tên nhóm (A-Z)");
                cmbSortGroup.Items.Add("Đông thành viên nhất");
                cmbSortGroup.SelectedIndex = 0;
            }
            cmbSortGroup.SelectedIndexChanged -= (s, ev) => LoadGroupData();
            cmbSortGroup.SelectedIndexChanged += (s, ev) => LoadGroupData();

            // Load dữ liệu ban đầu
            LoadUserData();
            LoadGroupData();
        }

        // ====================================================================================
        // PHẦN 1: QUẢN LÝ NGƯỜI DÙNG (USER)
        // ====================================================================================
        private void LoadUserData()
        {
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    string sql = @"SELECT T.USERID, T.USERNAME, T.HOTEN, T.EMAIL, 
                           P.TENPHONGBAN, T.TRANGTHAI, T.CHUC_VU
                    FROM TAIKHOAN T 
                    LEFT JOIN PHONGBAN P ON T.MAPHONGBAN = P.MAPHONGBAN
                    WHERE T.TRANGTHAI <> 'DELETED'";

                    string orderByClause = " ORDER BY T.USERNAME ASC";
                    if (cmbSortUser.SelectedItem != null)
                    {
                        switch (cmbSortUser.SelectedItem.ToString())
                        {
                            case "ID (Tăng dần)": orderByClause = " ORDER BY T.USERID ASC"; break;
                            case "Họ tên (A-Z)": orderByClause = " ORDER BY T.HOTEN ASC"; break;
                            default: orderByClause = " ORDER BY T.USERNAME ASC"; break;
                        }
                    }
                    sql += orderByClause;

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvUsers.DataSource = dt;

                        // Đặt tên cột tiếng Việt
                        if (dgvUsers.Columns.Contains("USERID")) dgvUsers.Columns["USERID"].HeaderText = "Mã NV";
                        if (dgvUsers.Columns.Contains("USERNAME")) dgvUsers.Columns["USERNAME"].HeaderText = "Tài khoản";
                        if (dgvUsers.Columns.Contains("HOTEN")) dgvUsers.Columns["HOTEN"].HeaderText = "Họ tên";
                        if (dgvUsers.Columns.Contains("EMAIL")) dgvUsers.Columns["EMAIL"].HeaderText = "Email";
                        if (dgvUsers.Columns.Contains("TENPHONGBAN")) dgvUsers.Columns["TENPHONGBAN"].HeaderText = "Phòng ban";
                        if (dgvUsers.Columns.Contains("TRANGTHAI")) dgvUsers.Columns["TRANGTHAI"].HeaderText = "Trạng thái";
                        if (dgvUsers.Columns.Contains("CHUC_VU")) dgvUsers.Columns["CHUC_VU"].HeaderText = "Chức vụ";
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải User: " + ex.Message); }
        }

        // --- Các nút chức năng User ---
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            DangKy dk = new DangKy(); dk.ShowDialog(); LoadUserData();
        }

        private void btnUpdateUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;
            string uid = dgvUsers.SelectedRows[0].Cells["USERID"].Value.ToString();
            string uname = dgvUsers.SelectedRows[0].Cells["USERNAME"].Value.ToString();
            Form_CapNhatTaiKhoan f = new Form_CapNhatTaiKhoan(uid, uname);
            f.ShowDialog();
            LoadUserData();
        }

        private void btnDeleteGroup_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;
            string uid = dgvUsers.SelectedRows[0].Cells["USERID"].Value.ToString();
            string uname = dgvUsers.SelectedRows[0].Cells["USERNAME"].Value.ToString();
            string currentAdminId = "U000";

            if (MessageBox.Show($"Xác nhận xóa tài khoản '{uname}'",
                "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                    {
                        if (conn.State != ConnectionState.Open) conn.Open();
                        using (OracleCommand cmd = new OracleCommand("NHOM8.SP_DELETE_USER_SOFT", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("p_userid_xoa", OracleDbType.Varchar2).Value = uid;
                            cmd.Parameters.Add("p_user_thuc_hien", OracleDbType.Varchar2).Value = currentAdminId;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show($"Đã xóa tài khoản '{uname}'.", "Thông báo");
                    LoadUserData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi thực hiện xóa: " + ex.Message); }
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;
            string uid = dgvUsers.SelectedRows[0].Cells["USERID"].Value.ToString();
            string status = dgvUsers.SelectedRows[0].Cells["TRANGTHAI"].Value.ToString();
            string newStatus = (status == "ACTIVE") ? "LOCKED" : "ACTIVE";

            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    new OracleCommand($"UPDATE TAIKHOAN SET TRANGTHAI='{newStatus}' WHERE USERID='{uid}'", conn).ExecuteNonQuery();
                }
                MessageBox.Show("Đã cập nhật trạng thái: " + newStatus); LoadUserData();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;
            string uid = dgvUsers.SelectedRows[0].Cells["USERID"].Value.ToString();
            string uname = dgvUsers.SelectedRows[0].Cells["USERNAME"].Value.ToString();
            LoginHistoryForm f = new LoginHistoryForm(uid, uname);
            f.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadUserData();
            string key = txtSearchUser.Text.ToLower();
            (dgvUsers.DataSource as DataTable).DefaultView.RowFilter = $"USERNAME LIKE '%{key}%' OR HOTEN LIKE '%{key}%'";
        }

        private void cmbSortUser_SelectedIndexChanged(object sender, EventArgs e) { LoadUserData(); }

        private void LoadGroupData()
        {
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string sql = @"SELECT N.MANHOM, N.TENNHOM, N.NGAYTAO, 
                                  T.HOTEN AS NGUOI_TAO,
                                  (SELECT COUNT(*) FROM THANHVIENNHOM TV WHERE TV.MANHOM = N.MANHOM) AS SO_THANH_VIEN
                           FROM NHOMCHAT N
                           LEFT JOIN TAIKHOAN T ON N.TAOBOI = T.USERID";

                    string sort = "";
                    if (cmbSortGroup.SelectedItem != null)
                    {
                        switch (cmbSortGroup.SelectedItem.ToString())
                        {
                            case "Tên nhóm (A-Z)": sort = " ORDER BY N.TENNHOM ASC"; break;
                            case "Đông thành viên nhất": sort = " ORDER BY SO_THANH_VIEN DESC"; break;
                            default: sort = " ORDER BY N.NGAYTAO DESC"; break;
                        }
                    }
                    sql += sort;
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvGroups.DataSource = dt;
                        if (dgvGroups.Columns.Contains("MANHOM")) dgvGroups.Columns["MANHOM"].HeaderText = "Mã Nhóm";
                        if (dgvGroups.Columns.Contains("TENNHOM")) dgvGroups.Columns["TENNHOM"].HeaderText = "Tên Nhóm";
                        if (dgvGroups.Columns.Contains("NGAYTAO")) dgvGroups.Columns["NGAYTAO"].HeaderText = "Ngày Tạo";
                        if (dgvGroups.Columns.Contains("NGUOI_TAO")) dgvGroups.Columns["NGUOI_TAO"].HeaderText = "Người tạo";
                        if (dgvGroups.Columns.Contains("SO_THANH_VIEN")) dgvGroups.Columns["SO_THANH_VIEN"].HeaderText = "Thành viên";
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải nhóm: " + ex.Message); }
        }
        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            string groupName = Interaction.InputBox("Nhập tên nhóm mới:", "Tạo Nhóm", "Nhóm Mới");
            if (string.IsNullOrWhiteSpace(groupName)) return;

            string groupId = "N" + DateTime.Now.Ticks.ToString().Substring(10);
            string creatorId = UserSession.UserID;

            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string sql = "INSERT INTO NHOMCHAT (MANHOM, TENNHOM, NGAYTAO, TAOBOI) VALUES (:p_manhom, :p_tennhom, SYSDATE, :p_taoboi)";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("p_manhom", groupId));
                        cmd.Parameters.Add(new OracleParameter("p_tennhom", groupName));
                        cmd.Parameters.Add(new OracleParameter("p_taoboi", creatorId));
                        cmd.ExecuteNonQuery();
                    }
                    string sqlMem = "INSERT INTO THANHVIENNHOM (MANHOM, USERID, DUYET) VALUES (:p_manhom, :p_user, 1)";
                    using (OracleCommand cmd = new OracleCommand(sqlMem, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("p_manhom", groupId));
                        cmd.Parameters.Add(new OracleParameter("p_user", creatorId));
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Tạo nhóm thành công!", "Thông báo");
                LoadGroupData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo nhóm: " + ex.Message);
            }
        }
        private void btnDeleteGroups_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;
            string uid = dgvUsers.SelectedRows[0].Cells["USERID"].Value.ToString();

            if (MessageBox.Show("Xóa tài khoản này?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                    {
                        if (conn.State != ConnectionState.Open) conn.Open();
                        new OracleCommand($"DELETE FROM THANHVIENNHOM WHERE USERID='{uid}'", conn).ExecuteNonQuery();
                        new OracleCommand($"DELETE FROM TINNHAN WHERE GUIBOI='{uid}'", conn).ExecuteNonQuery();
                        new OracleCommand($"DELETE FROM TAIKHOAN WHERE USERID='{uid}'", conn).ExecuteNonQuery();
                    }
                    MessageBox.Show("Đã xóa!"); LoadUserData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
            }
        }
        private void btnSearchGroup_Click(object sender, EventArgs e)
        {
            string key = txtSearchGroup.Text.Trim();
            if (dgvGroups.DataSource is DataTable dt)
            {
                try
                {
                    dt.DefaultView.RowFilter = string.Format("TENNHOM LIKE '%{0}%' OR " + "MANHOM LIKE '%{0}%' OR " + "NGUOI_TAO LIKE '%{0}%'",
 key);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            new DangNhap().Show(); this.Close();
        }

        private void tpUserManagement_Click(object sender, EventArgs e) { }
        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void txtSearchUser_TextChanged(object sender, EventArgs e) { }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void dgvGroups_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void txtSearchUser_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void btnManageMembers_Click_1(object sender, EventArgs e)
        {
            if (dgvGroups.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một nhóm để xem thành viên!", "Thông báo");
                return;
            }

            string gid = dgvGroups.SelectedRows[0].Cells["MANHOM"].Value.ToString();
            string gName = dgvGroups.SelectedRows[0].Cells["TENNHOM"].Value.ToString();

            string sql = @"
        SELECT T.USERID AS ""Mã NV"", 
               T.HOTEN AS ""Họ và Tên"", 
               T.CHUC_VU AS ""Chức Vụ"", 
               P.TENPHONGBAN AS ""Phòng Ban""
        FROM THANHVIENNHOM TV
        JOIN TAIKHOAN T ON TV.USERID = T.USERID
        LEFT JOIN PHONGBAN P ON T.MAPHONGBAN = P.MAPHONGBAN
        WHERE TV.MANHOM = :gid  -- Đã đổi thành :gid
        ORDER BY CASE WHEN T.CHUC_VU = 'LEAD' THEN 1 ELSE 2 END, T.HOTEN ASC";

            ShowDataPopup($"Thành viên nhóm: {gName}", sql, gid);
        }


        private void btnViewChatHistory_Click_1(object sender, EventArgs e)
        {
            if (dgvGroups.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một nhóm để xem lịch sử chat!", "Thông báo");
                return;
            }

            string gid = dgvGroups.SelectedRows[0].Cells["MANHOM"].Value.ToString();
            string gName = dgvGroups.SelectedRows[0].Cells["TENNHOM"].Value.ToString();

            string sql = @"
        SELECT T.HOTEN AS ""Người Gửi"", 
               NHOM8.GIAIMARSA(TN.NOIDUNG) AS ""Nội Dung"", 
               TO_CHAR(TN.THOIGIAN, 'DD/MM/YYYY HH24:MI:SS') AS ""Thời Gian""
        FROM TINNHAN TN
        JOIN TAIKHOAN T ON TN.GUIBOI = T.USERID
        WHERE TN.MANHOM = :gid -- Đã đổi thành :gid
        ORDER BY TN.THOIGIAN DESC";

            ShowDataPopup($"Lịch sử chat nhóm: {gName}", sql, gid);
        }

        private void btnEditGroup_Click_1(object sender, EventArgs e)
        {
            if (dgvGroups.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhóm cần sửa!", "Thông báo");
                return;
            }
            string gid = dgvGroups.SelectedRows[0].Cells["MANHOM"].Value.ToString();
            string oldName = dgvGroups.SelectedRows[0].Cells["TENNHOM"].Value.ToString();
            string newName = Interaction.InputBox("Nhập tên mới:", "Sửa Tên Nhóm", oldName);
            if (string.IsNullOrWhiteSpace(newName) || newName == oldName) return;

            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string sql = "UPDATE NHOMCHAT SET TENNHOM = :p_tennhom WHERE MANHOM = :p_manhom";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("p_tennhom", newName));
                        cmd.Parameters.Add(new OracleParameter("p_manhom", gid));

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Cập nhật thành công!", "Thông báo");
                            LoadGroupData();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhóm để cập nhật.", "Lỗi");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
        }
        private void txtSearchGroup_TextChanged(object sender, EventArgs e)
        {

        }


        //MAHOATN 
        private void btnGroupNotification_Click_1(object sender, EventArgs e)
        {
            if (dgvGroups.SelectedRows.Count == 0) return;
            string gid = dgvGroups.SelectedRows[0].Cells["MANHOM"].Value.ToString();
            string msg = Interaction.InputBox("Nhập nội dung thông báo:", "Gửi thông báo nhóm");
            if (string.IsNullOrWhiteSpace(msg)) return;

            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string msgId = "TB" + DateTime.Now.Ticks.ToString().Substring(12);
                    string sql = "INSERT INTO TINNHAN (MATN, MANHOM, GUIBOI, NOIDUNG, THOIGIAN) VALUES (:id, :gid, :uid, NHOM8.MAHOARSA(:content), SYSDATE)";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("id", msgId);
                        cmd.Parameters.Add("gid", gid);
                        cmd.Parameters.Add("uid", UserSession.UserID);
                        cmd.Parameters.Add("content", "[THÔNG BÁO ADMIN]: " + msg);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Đã gửi thông báo vào nhóm!");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
        private void ShowDataPopup(string title, string query, string paramValue)
        {
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // --- SỬA LẠI ĐOẠN NÀY ---
                        cmd.BindByName = true; // Bắt buộc có dòng này để tránh lỗi nhầm lẫn tham số
                        cmd.Parameters.Add(new OracleParameter("gid", paramValue)); // Chỉ add 1 tham số duy nhất

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                FormDataView frm = new FormDataView();
                frm.HienThiDuLieu(title, dt);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị dữ liệu: " + ex.Message);
            }
        }
        private DataTable LoadDeletedUserData()
        {
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    string sql = @"
                SELECT 
                    USERID, USERNAME, HOTEN, 
                    THOIGIAN_XOA, XOA_BOI
                FROM LOG_DELETE_TAIKHOAN
                ORDER BY THOIGIAN_XOA DESC";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải Log xóa User: " + ex.Message); return null; }
        }

        private void PerformRestore(string userId)
        {
            try
            {
                using (OracleConnection conn = BaoCao.DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    using (OracleCommand cmd = new OracleCommand("NHOM8.SP_RESTORE_USER", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_userid_khoi_phuc", OracleDbType.Varchar2).Value = userId;
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show($"Khôi phục tài khoản '{userId}' thành công! Vui lòng kiểm tra lại danh sách user.", "Hoàn tất");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khôi phục: " + ex.Message);
            }
        }
        private void btn_KhoiPhuc_Click(object sender, EventArgs e)
        {
            KhoiPhucTaiKhoan frmRestore = new KhoiPhucTaiKhoan();
            frmRestore.ShowDialog();
            LoadUserData();
        }

        private void btn_GiamSat_Click(object sender, EventArgs e)
        {
            Audit auditForm = new Audit();
            auditForm.ShowDialog();
        }

        private void dgvUsers_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}