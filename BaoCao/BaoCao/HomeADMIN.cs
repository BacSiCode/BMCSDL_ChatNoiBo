using BaoCao;
using Microsoft.AspNetCore.SignalR.Client;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Nhom8
{
    public partial class HomeADMIN : Form
    {
        private HubConnection _hubConnection;
        private string _selectedFilePath = null;
        private string _currentUserId = UserSession.UserID;
        private string _currentGroup = "";
        private string _currentGroupName = "";

        public HomeADMIN()
        {
            InitializeComponent();
            CenterToScreen();

            // Tự động tham gia nhóm Chung (GENERAL)
            JoinGeneralGroupAutomatic();

            // Lấy nhóm mặc định (GENERAL hoặc nhóm quản lý)
            _currentGroup = GetFirstGroupId(_currentUserId);
            if (string.IsNullOrEmpty(_currentGroup)) _currentGroup = "GENERAL";

            InitializeSignalR();
        }

        private void HomeADMIN_Load(object sender, EventArgs e)
        {
            label1.Text = "LEAD: " + (UserSession.HoTen ?? "Admin");
            if (_currentGroup != "GENERAL" && !_currentGroup.StartsWith("PRI_"))
            {
                pnlMembers.Visible = true;
                LoadGroupMembers();
            }
            else
            {
                pnlMembers.Visible = false;
            }
            LoadMyGroups();
            LoadGroupMembers();
            LoadChatHistory(_currentGroup);
        }

        private void JoinGeneralGroupAutomatic()
        {
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    try { DataBase.SetOracleContext(conn, "U000"); } catch { }
                    string check = "SELECT COUNT(*) FROM THANHVIENNHOM WHERE MANHOM='GENERAL' AND USERID=:u";
                    using (OracleCommand cmd = new OracleCommand(check, conn))
                    {
                        cmd.Parameters.Add("u", _currentUserId);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                        {
                            string insert = "INSERT INTO THANHVIENNHOM (MANHOM, USERID, DUYET) VALUES ('GENERAL', :u, 1)";
                            using (OracleCommand cmdIn = new OracleCommand(insert, conn))
                            {
                                cmdIn.Parameters.Add("u", _currentUserId);
                                cmdIn.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch { }
        }

        // --- 1. QUẢN LÝ NHÓM & SIDEBAR ---
        private void LoadMyGroups()
        {
            flowGroups.Controls.Clear();
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    DataBase.SetOracleContext(conn, _currentUserId); // VPD

                    string sql = @"SELECT nc.MANHOM, nc.TENNHOM, nc.LOAI_NHOM
                                   FROM NHOMCHAT nc 
                                   JOIN THANHVIENNHOM tv ON nc.MANHOM = tv.MANHOM 
                                   WHERE tv.USERID = :u AND (nc.LOAI_NHOM = 0 OR nc.LOAI_NHOM IS NULL OR nc.MANHOM = 'GENERAL')
                                   ORDER BY CASE WHEN nc.MANHOM = 'GENERAL' THEN 0 ELSE 1 END, nc.TENNHOM ASC";

                    DataTable dt = new DataTable();
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("u", _currentUserId);
                        new OracleDataAdapter(cmd).Fill(dt);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        UC_ContactItem item = new UC_ContactItem();
                        string gID = row["MANHOM"].ToString();
                        string gName = row["TENNHOM"].ToString();

                        // --- FIX THIẾT KẾ MÀU SẮC ---
                        if (gID == "GENERAL")
                        {
                            gName = "Kênh Chung";
                            item.BackColor = Color.FromArgb(40, 60, 80);
                            item.ForeColor = Color.White;
                        }

                        item.SetData(gID, gName);
                        item.Dock = DockStyle.Top;

                        item.Click += (s, e) =>
                        {
                            _currentGroup = gID;
                            _currentGroupName = gName;

                            // Nếu là nhóm thường (Admin quản lý) -> Hiện Panel Duyệt TV
                            if (gID != "GENERAL" && !gID.StartsWith("PRI_"))
                            {
                                pnlMembers.Visible = true;
                                LoadGroupMembers();
                            }
                            else
                            {
                                pnlMembers.Visible = false; // Ẩn panel nếu là chat chung/riêng
                            }

                            LoadChatHistory(gID);
                            if (_hubConnection.State == HubConnectionState.Connected)
                                _hubConnection.InvokeAsync("JoinGroup", gID);
                        };

                        flowGroups.Controls.Add(item);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải danh sách nhóm: " + ex.Message); }
        }

        private string GetFirstGroupId(string userId) { return "GENERAL"; }

        // --- 2. TAB CONTROLS (Groups / Friends / Search) ---
        private void btnTabGroup_Click(object sender, EventArgs e)
        {
            LoadMyGroups(); // Load lại nhóm
        }

        private void btnTabFriend_Click(object sender, EventArgs e)
        {
            LoadMyFriends(); // Load bạn bè (để chat 1-1)
        }

        private void btnRequests_Click(object sender, EventArgs e)
        {
            LoadFriendRequests(); // Load lời mời
        }

        private void btnAddFriend_Click(object sender, EventArgs e)
        {
            string key = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(key)) SearchUserInline(key);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnAddFriend_Click(sender, e);
        }

        // --- 3. KẾT BẠN & CHAT 1-1 ---
        private void LoadMyFriends()
        {
            flowGroups.Controls.Clear();
            string sql = @"SELECT t.USERID, t.HOTEN, t.TRUC_TUYEN 
                           FROM BANBE b
                           JOIN TAIKHOAN t ON (b.USER_ID_1 = t.USERID OR b.USER_ID_2 = t.USERID)
                           WHERE (b.USER_ID_1 = :me OR b.USER_ID_2 = :me) 
                             AND t.USERID != :me AND b.TRANGTHAI = 1";
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("me", _currentUserId);
                        new OracleDataAdapter(cmd).Fill(dt);
                    }
                }
                foreach (DataRow row in dt.Rows)
                {
                    UC_ContactItem item = new UC_ContactItem();
                    string fID = row["USERID"].ToString();
                    string fName = row["HOTEN"].ToString();
                    string onlineVal = row["TRUC_TUYEN"] != DBNull.Value ? row["TRUC_TUYEN"].ToString() : "0";
                    string statusIcon = (onlineVal == "1") ? "🟢" : "⚫";

                    item.SetData(fID, $"{statusIcon} {fName}");
                    item.Dock = DockStyle.Top;
                    item.Click += (s, ev) => OpenPrivateChat(fID, fName);
                    flowGroups.Controls.Add(item);
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải bạn bè: " + ex.Message); }
        }

        private void OpenPrivateChat(string friendId, string friendName)
        {
            try
            {
                string id1 = String.Compare(_currentUserId, friendId) < 0 ? _currentUserId : friendId;
                string id2 = String.Compare(_currentUserId, friendId) < 0 ? friendId : _currentUserId;
                string privateGroupId = $"PRI_{id1}_{id2}";

                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    try { DataBase.SetOracleContext(conn, "U000"); } catch { }

                    string checkSql = "SELECT COUNT(*) FROM NHOMCHAT WHERE MANHOM = :g";
                    using (OracleCommand cmd = new OracleCommand(checkSql, conn))
                    {
                        cmd.Parameters.Add("g", privateGroupId);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                        {
                            string insertGroup = "INSERT INTO NHOMCHAT (MANHOM, TENNHOM, TAOBOI) VALUES (:g, :name, :u)";
                            using (OracleCommand cmdG = new OracleCommand(insertGroup, conn))
                            {
                                cmdG.Parameters.Add("g", privateGroupId);
                                cmdG.Parameters.Add("name", $"Chat: {_currentUserId} - {friendId}");
                                cmdG.Parameters.Add("u", _currentUserId);
                                cmdG.ExecuteNonQuery();
                            }
                            string insertMem = "INSERT INTO THANHVIENNHOM (MANHOM, USERID, DUYET) VALUES (:g, :u, 1)";
                            using (OracleCommand cmdM1 = new OracleCommand(insertMem, conn))
                            {
                                cmdM1.Parameters.Add("g", privateGroupId);
                                cmdM1.Parameters.Add("u", _currentUserId);
                                cmdM1.ExecuteNonQuery();
                            }
                            using (OracleCommand cmdM2 = new OracleCommand(insertMem, conn))
                            {
                                cmdM2.Parameters.Add("g", privateGroupId);
                                cmdM2.Parameters.Add("u", friendId);
                                cmdM2.ExecuteNonQuery();
                            }
                        }
                    }
                }
                _currentGroup = privateGroupId;
                _currentGroupName = friendName;
                pnlMembers.Visible = false; // Ẩn panel quản lý

                rtxbChat.Clear();
                rtxbChat.AppendText($"--- Chat riêng với: {friendName} ---\n");

                if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                    _hubConnection.InvokeAsync("JoinGroup", _currentGroup);

                LoadChatHistory(_currentGroup);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi mở chat riêng: " + ex.Message); }
        }

        private void LoadFriendRequests()
        { /* Logic giống Home.cs */
            flowGroups.Controls.Clear();
            string sql = @"SELECT t.USERID, t.HOTEN FROM BANBE b JOIN TAIKHOAN t ON b.USER_ID_1 = t.USERID
                           WHERE b.USER_ID_2 = :me AND b.TRANGTHAI = 0";
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("me", _currentUserId);
                        new OracleDataAdapter(cmd).Fill(dt);
                    }
                }
                if (dt.Rows.Count == 0)
                {
                    Label lbl = new Label { Text = "Không có lời mời nào.", AutoSize = true, ForeColor = Color.White };
                    flowGroups.Controls.Add(lbl); return;
                }
                foreach (DataRow row in dt.Rows)
                {
                    UC_ContactItem item = new UC_ContactItem();
                    string reqID = row["USERID"].ToString();
                    string reqName = row["HOTEN"].ToString();
                    item.SetData(reqID, $"📩 {reqName}");
                    item.Dock = DockStyle.Top;
                    item.ShowAddFriendButton(true);
                    item.AddFriendClicked += (s, ev) => AcceptFriendRequest(reqID, reqName);
                    flowGroups.Controls.Add(item);
                }
            }
            catch { }
        }

        private void AcceptFriendRequest(string fId, string fName)
        {
            if (MessageBox.Show("Chấp nhận?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No) return;
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string sql = "UPDATE BANBE SET TRANGTHAI=1 WHERE USER_ID_1=:fr AND USER_ID_2=:me";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("fr", fId); cmd.Parameters.Add("me", _currentUserId);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Đã kết bạn!"); LoadFriendRequests();
            }
            catch { }
        }

        private void SearchUserInline(string keyword)
        {
            flowGroups.Controls.Clear();
            if (keyword == _currentUserId) return;
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string sql = "SELECT USERID, HOTEN FROM TAIKHOAN WHERE EMAIL=:k OR DIENTHOAI=:k OR USERID=:k";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("k", keyword);
                        using (OracleDataReader rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                string fID = rd["USERID"].ToString();
                                string fName = rd["HOTEN"].ToString();
                                UC_ContactItem item = new UC_ContactItem();
                                item.SetData(fID, fName); item.Dock = DockStyle.Top;
                                item.ShowAddFriendButton(true);
                                item.AddFriendClicked += (s, e) => SendFriendRequest(fID);
                                flowGroups.Controls.Add(item);
                            }
                            else MessageBox.Show("Không tìm thấy!");
                        }
                    }
                }
            }
            catch { }
        }

        private void SendFriendRequest(string targetID)
        {
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    string sql = "INSERT INTO BANBE (USER_ID_1, USER_ID_2, TRANGTHAI, NGAYGUI) VALUES (:me, :u, 0, SYSDATE)";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("me", _currentUserId); cmd.Parameters.Add("u", targetID);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Đã gửi lời mời!");
            }
            catch { MessageBox.Show("Đã gửi rồi!"); }
        }

        // --- 4. CHAT LOGIC (FIX DOUBLE TIN NHẮN) ---
        private async void InitializeSignalR()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chathub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string, string>("ReceiveMessage", (userId, message) =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string timeNow = DateTime.Now.ToString("HH:mm");
                    if (userId == "SYSTEM")
                    {
                        rtxbChat.SelectionColor = Color.Red;
                        rtxbChat.SelectionFont = new Font(rtxbChat.Font, FontStyle.Bold);
                        rtxbChat.AppendText($"[HỆ THỐNG]: {message}\n");
                    }
                    else
                    {
                        rtxbChat.SelectionColor = (userId == _currentUserId) ? Color.Blue : Color.Black;
                        rtxbChat.SelectionFont = new Font(rtxbChat.Font, FontStyle.Regular);
                        rtxbChat.AppendText($"[{timeNow}] {userId}: {message}\n");
                    }
                    rtxbChat.ScrollToCaret();
                });
            });

            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync("JoinGroup", "GENERAL");
                if (!string.IsNullOrEmpty(_currentGroup) && _currentGroup != "GENERAL")
                    await _hubConnection.InvokeAsync("JoinGroup", _currentGroup);
            }
            catch { }
        }

        private async void guna2Button2_Click(object sender, EventArgs e)
        {
            var btn = (Guna.UI2.WinForms.Guna2Button)sender;
            if (!btn.Enabled) return; btn.Enabled = false;

            string message = guna2TextBox1_chat.Text.Trim();
            if (string.IsNullOrEmpty(message) && string.IsNullOrEmpty(_selectedFilePath)) { btn.Enabled = true; return; }
            if (!string.IsNullOrEmpty(_selectedFilePath)) message = $"[File: {Path.GetFileName(_selectedFilePath)}] {message}";

            try
            {
                // CHỈ GỬI SIGNALR - KHÔNG TỰ LƯU DB (Fix Double)
                await _hubConnection.InvokeAsync("SendMessageToGroup", _currentGroup, _currentUserId, message);

                guna2TextBox1_chat.Clear();
                _selectedFilePath = null;
                guna2TextBox1_chat.Focus();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi gửi tin: " + ex.Message); }
            finally { await Task.Delay(500); btn.Enabled = true; }
        }

        private void LoadChatHistory(string groupId)
        {
            if (string.IsNullOrEmpty(groupId)) return;
            rtxbChat.Clear();
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    // --- BỔ SUNG QUAN TRỌNG: Thiết lập Context cho User hiện tại ---
                    // Giúp Admin/Lead có thể thấy dữ liệu nếu có chính sách VPD
                    try { DataBase.SetOracleContext(conn, _currentUserId); } catch { }

                    // --- SỬA ĐỔI: Sử dụng hàm GIAIMARSA để lấy tin nhắn giải mã ---
                    string sql = "SELECT GUIBOI, NHOM8.GIAIMARSA(NOIDUNG) AS NOIDUNG, THOIGIAN FROM TINNHAN WHERE MANHOM = :g ORDER BY THOIGIAN ASC";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("g", groupId);
                        new OracleDataAdapter(cmd).Fill(dt);
                    }
                }
                foreach (DataRow row in dt.Rows)
                {
                    string senderId = row["GUIBOI"].ToString();
                    string content = row["NOIDUNG"] != DBNull.Value ? row["NOIDUNG"].ToString() : ""; // Fix Null
                    DateTime time = Convert.ToDateTime(row["THOIGIAN"]);
                    rtxbChat.SelectionColor = (senderId == _currentUserId) ? Color.Blue : Color.Black;
                    rtxbChat.AppendText($"[{time:HH:mm}] {senderId}: {content}\n");
                }
                rtxbChat.ScrollToCaret();
            }
            catch { }
        }

        // --- 5. QUẢN LÝ THÀNH VIÊN (GIỮ NGUYÊN) ---
        private void LoadGroupMembers()
        {
            if (string.IsNullOrEmpty(_currentGroup) || _currentGroup == "GENERAL" || _currentGroup.StartsWith("PRI_")) return;
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    DataBase.SetOracleContext(conn, _currentUserId);

                    string sql = @"
                        SELECT T.USERID, T.HOTEN, TV.DUYET, 
                               CASE WHEN TV.DUYET = 1 THEN 'Đã vào' ELSE 'Chờ duyệt' END AS TRANG_THAI
                        FROM TAIKHOAN T
                        JOIN THANHVIENNHOM TV ON T.USERID = TV.USERID
                        WHERE TV.MANHOM = :gid
                        ORDER BY TV.DUYET ASC, T.HOTEN ASC";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("gid", _currentGroup);
                        DataTable dt = new DataTable();
                        new OracleDataAdapter(cmd).Fill(dt);
                        dgvMembers.DataSource = dt;

                        if (dgvMembers.Columns.Contains("USERID")) dgvMembers.Columns["USERID"].Visible = false;
                        if (dgvMembers.Columns.Contains("DUYET")) dgvMembers.Columns["DUYET"].Visible = false;
                        if (dgvMembers.Columns.Contains("HOTEN")) dgvMembers.Columns["HOTEN"].HeaderText = "Thành Viên";
                        if (dgvMembers.Columns.Contains("TRANG_THAI")) dgvMembers.Columns["TRANG_THAI"].HeaderText = "Trạng Thái";
                        dgvMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        foreach (DataGridViewRow row in dgvMembers.Rows)
                        {
                            if (row.Cells["TRANG_THAI"].Value != null && row.Cells["TRANG_THAI"].Value.ToString() == "Chờ duyệt")
                            {
                                row.DefaultCellStyle.BackColor = Color.LightYellow;
                                row.DefaultCellStyle.ForeColor = Color.Red;
                                row.DefaultCellStyle.Font = new Font(dgvMembers.Font, FontStyle.Bold);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải thành viên: " + ex.Message); }
        }

        private void BtnRefresh_Click(object sender, EventArgs e) { LoadGroupMembers(); }
        private async void BtnDuyet_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0) return;
            string userId = dgvMembers.SelectedRows[0].Cells["USERID"].Value.ToString();
            string status = dgvMembers.SelectedRows[0].Cells["DUYET"].Value.ToString();
            string userName = dgvMembers.SelectedRows[0].Cells["HOTEN"].Value.ToString();

            if (status == "1") { MessageBox.Show("Thành viên này đã duyệt rồi!"); return; }

            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    DataBase.SetOracleContext(conn, _currentUserId);
                    string sql = "UPDATE THANHVIENNHOM SET DUYET = 1 WHERE USERID = :u AND MANHOM = :g";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("u", userId);
                        cmd.Parameters.Add("g", _currentGroup);
                        cmd.ExecuteNonQuery();
                    }
                }
                if (_hubConnection.State == HubConnectionState.Connected)
                    await _hubConnection.InvokeAsync("SendMessageToGroup", _currentGroup, "SYSTEM", $"🎉 Chào mừng {userName} gia nhập nhóm!");

                MessageBox.Show("Đã duyệt thành công!");
                LoadGroupMembers();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }

        }
        private void BtnTuChoi_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0) return;
            string userId = dgvMembers.SelectedRows[0].Cells["USERID"].Value.ToString();
            string status = dgvMembers.SelectedRows[0].Cells["DUYET"].Value.ToString();
            if (status == "1") { MessageBox.Show("Không thể từ chối thành viên chính thức!"); return; }
            if (MessageBox.Show("Từ chối yêu cầu này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                ExecuteDeleteMember(userId);

        }
        private async void BtnKick_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0) return;
            string userId = dgvMembers.SelectedRows[0].Cells["USERID"].Value.ToString();
            string userName = dgvMembers.SelectedRows[0].Cells["HOTEN"].Value.ToString();
            if (userId == _currentUserId) { MessageBox.Show("Không thể tự xóa mình!"); return; }
            if (MessageBox.Show($"Mời {userName} ra khỏi nhóm?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ExecuteDeleteMember(userId);
                if (_hubConnection.State == HubConnectionState.Connected)
                    await _hubConnection.InvokeAsync("SendMessageToGroup", _currentGroup, "SYSTEM", $"{userName} đã bị mời ra khỏi nhóm.");
            }
        }
        private void ExecuteDeleteMember(string userId)
        {
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    DataBase.SetOracleContext(conn, _currentUserId);
                    string sql = "DELETE FROM THANHVIENNHOM WHERE USERID = :u AND MANHOM = :g";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("u", userId);
                        cmd.Parameters.Add("g", _currentGroup);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadGroupMembers();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
        private void guna2Button2_upload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()) { if (ofd.ShowDialog() == DialogResult.OK) { _selectedFilePath = ofd.FileName; guna2TextBox1_chat.Text += $" [File]"; } }
        }
        private void guna2Button1_Click(object sender, EventArgs e) { new DangNhap().Show(); this.Close(); }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void guna2TextBox1_chat_TextChanged(object sender, EventArgs e) { }
        private void lblMemberTitle_Click(object sender, EventArgs e) { }

        private void dgvMembers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}