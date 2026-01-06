using BaoCao;
using Microsoft.AspNetCore.SignalR.Client;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Nhom8
{
    public partial class Home : Form
    {
        private HubConnection _hubConnection;
        private string _selectedFilePath = null;
        private string _currentUserId = "";
        private string _currentGroupId = "";
        private string _currentGroupName = "";
        private System.Windows.Forms.Timer _statusTimer;

        public Home()
        {
            InitializeComponent();
            CenterToScreen();
            _currentUserId = UserSession.UserID;
            UpdateUserStatus(true);

            _statusTimer = new System.Windows.Forms.Timer();
            _statusTimer.Interval = 30000;
            _statusTimer.Tick += (s, e) => LoadGroupMembersStatus();
            _statusTimer.Start();

            txtSearch.KeyDown += txtSearch_KeyDown;
        }

        private void Home_Load(object sender, EventArgs e)
        {
            ShowUserInfo();
            LoadMyGroups();
            JoinGeneralGroupAutomatic();
            InitializeSignalR();
        }


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
                    rtxbUser.AppendText($"[{timeNow}] {userId}: {message}\n");
                    rtxbUser.ScrollToCaret();
                });
            });

            try
            {
                await _hubConnection.StartAsync();
                await _hubConnection.InvokeAsync("JoinGroup", "GENERAL");
                if (!string.IsNullOrEmpty(_currentGroupId))
                {
                    await _hubConnection.InvokeAsync("JoinGroup", _currentGroupId);
                }
            }
            catch { }
        }
        private void JoinGeneralGroupAutomatic()// Nhan chung cho tat ca nguoi dung
        {
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    try { DataBase.SetOracleContext(conn, "U000"); } catch { } // Quyền Admin để check

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

        private async void btnSendUser_Click(object sender, EventArgs e)
        {
            var btn = (Guna.UI2.WinForms.Guna2Button)sender;
            if (!btn.Enabled) return;
            btn.Enabled = false;

            string message = txtUser.Text.Trim();
            if (string.IsNullOrEmpty(message) && string.IsNullOrEmpty(_selectedFilePath))
            {
                btn.Enabled = true; return;
            }

            if (!string.IsNullOrEmpty(_selectedFilePath))
                message = $"[File: {Path.GetFileName(_selectedFilePath)}] {message}";

            if (string.IsNullOrEmpty(_currentGroupId))
            {
                MessageBox.Show("Chưa chọn nhóm để gửi!");
                btn.Enabled = true; return;
            }

            try
            {
                await _hubConnection.InvokeAsync("SendMessageToGroup", _currentGroupId, _currentUserId, message);

                txtUser.Clear();
                _selectedFilePath = null;
                txtUser.Focus();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi gửi tin: " + ex.Message); }
            finally
            {
                await Task.Delay(500);
                btn.Enabled = true;
            }
        }

        // --- 2. QUẢN LÝ NHÓM & LỊCH SỬ CHAT ---
        private void LoadMyGroups()
        {
            flowGroups.Controls.Clear();

            //Sắp xếp để nhóm GENERAL lên đầu
            string sql = @"SELECT nc.MANHOM, nc.TENNHOM 
                           FROM NHOMCHAT nc 
                           JOIN THANHVIENNHOM tv ON nc.MANHOM = tv.MANHOM 
                           WHERE tv.USERID = :u AND (nc.LOAI_NHOM = 0 OR nc.LOAI_NHOM IS NULL OR nc.MANHOM = 'GENERAL')
                           ORDER BY CASE WHEN nc.MANHOM = 'GENERAL' THEN 0 ELSE 1 END, nc.TENNHOM ASC";

            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    DataBase.SetOracleContext(conn, _currentUserId);
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("u", _currentUserId));
                        new OracleDataAdapter(cmd).Fill(dt);
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        UC_ContactItem item = new UC_ContactItem();
                        string gID = row["MANHOM"].ToString();
                        string gName = row["TENNHOM"].ToString();
                        if (gID == "GENERAL")
                        {
                            gName = "Kênh Chung";
                            item.BackColor = Color.FromArgb(40, 60, 80);
                            item.ForeColor = Color.White;
                        }
                        item.SetData(gID, gName);
                        item.Dock = DockStyle.Top;
                        item.Click += Item_Click;
                        flowGroups.Controls.Add(item);
                    }

                    // Nếu chưa chọn nhóm, mặc định vào nhóm đầu tiên
                    if (string.IsNullOrEmpty(_currentGroupId))
                    {
                        _currentGroupId = dt.Rows[0]["MANHOM"].ToString();
                        _currentGroupName = dt.Rows[0]["TENNHOM"].ToString();
                        if (_currentGroupId == "GENERAL") _currentGroupName = "Kênh Chung";

                        rtxbUser.AppendText($"--- Đã vào: {_currentGroupName} ---\n");
                        LoadChatHistory(_currentGroupId);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi load nhóm: " + ex.Message); }
        }

        private void Item_Click(object sender, EventArgs e)
        {
            UC_ContactItem clickedItem = (UC_ContactItem)sender;
            _currentGroupId = clickedItem.GroupID;
            rtxbUser.Clear();
            string displayName = clickedItem.GroupID == "GENERAL" ? "Kênh Chung" : clickedItem.GroupID;
            rtxbUser.AppendText($"--- Chat với: {displayName} ---\n");
            if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                _hubConnection.InvokeAsync("JoinGroup", _currentGroupId);
            LoadChatHistory(_currentGroupId);
        }

        private void LoadChatHistory(string groupId)
        {
            rtxbUser.Clear();       
            string sql = "SELECT GUIBOI, NHOM8.GIAIMARSA(NOIDUNG) AS NOIDUNG, THOIGIAN FROM TINNHAN WHERE MANHOM = :g ORDER BY THOIGIAN ASC";
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    // Đảm bảo mở kết nối
                    if (conn.State != ConnectionState.Open) conn.Open();
                    try { DataBase.SetOracleContext(conn, _currentUserId); } catch { }

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("g", groupId));
                        new OracleDataAdapter(cmd).Fill(dt);
                    }
                }
                foreach (DataRow row in dt.Rows)
                {
                    string sender = row["GUIBOI"].ToString();
                    string content = row["NOIDUNG"] != DBNull.Value ? row["NOIDUNG"].ToString() : ""; // Fix lỗi null
                    DateTime time = Convert.ToDateTime(row["THOIGIAN"]);
                    rtxbUser.AppendText($"[{time:HH:mm}] {sender}: {content}\n");
                }
                rtxbUser.ScrollToCaret();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải lịch sử: " + ex.Message); }
        }

        // --- 3. FIX LỖI KẾT BẠN & ONLINE ---
        private void btnTabFriend_Click(object sender, EventArgs e)
        {
            flowGroups.Visible = false; flowContacts.Visible = true; LoadMyFriends();
        }
        private void btnTabGroup_Click(object sender, EventArgs e)
        {
            flowGroups.Visible = true; flowContacts.Visible = false; LoadMyGroups();
        }
        private void btnRequests_Click(object sender, EventArgs e) { LoadFriendRequests(); }

        private void LoadFriendRequests()
        {
            flowGroups.Visible = false; flowContacts.Visible = true; flowContacts.Controls.Clear();
            string sql = @"SELECT t.USERID, t.HOTEN FROM BANBE b JOIN TAIKHOAN t ON b.USER_ID_1 = t.USERID
                           WHERE b.USER_ID_2 = :me AND b.TRANGTHAI = 0";
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("me", _currentUserId));
                        new OracleDataAdapter(cmd).Fill(dt);
                    }
                }
                if (dt.Rows.Count == 0)
                {
                    Label lbl = new Label { Text = "Không có lời mời nào.", AutoSize = true, ForeColor = Color.Gray };
                    flowContacts.Controls.Add(lbl); return;
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
                    flowContacts.Controls.Add(item);
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải lời mời: " + ex.Message); }
        }

        private void AcceptFriendRequest(string friendID, string friendName)
        {
            if (MessageBox.Show($"Chấp nhận kết bạn với {friendName}?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No) return;
            string sql = "UPDATE BANBE SET TRANGTHAI = 1 WHERE USER_ID_1 = :friend AND USER_ID_2 = :me";
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("friend", friendID));
                        cmd.Parameters.Add(new OracleParameter("me", _currentUserId));
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Đã trở thành bạn bè!");
                LoadFriendRequests();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi chấp nhận: " + ex.Message); }
        }

        private void LoadMyFriends()
        {
            flowContacts.Controls.Clear();
            string sql = @"SELECT t.USERID, t.HOTEN, t.TRUC_TUYEN 
                           FROM BANBE b
                           JOIN TAIKHOAN t ON (b.USER_ID_1 = t.USERID OR b.USER_ID_2 = t.USERID)
                           WHERE (b.USER_ID_1 = :me OR b.USER_ID_2 = :me) 
                             AND t.USERID != :me 
                             AND b.TRANGTHAI = 1";
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("me", _currentUserId));
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
                    item.ShowAddFriendButton(false);
                    item.Click += (s, ev) => OpenPrivateChat(fID, fName);
                    flowContacts.Controls.Add(item);
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
                _currentGroupId = privateGroupId;
                _currentGroupName = friendName;
                rtxbUser.Clear();
                rtxbUser.AppendText($"--- Chat với: {friendName} ---\n");

                if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                    _hubConnection.InvokeAsync("JoinGroup", _currentGroupId);

                LoadChatHistory(_currentGroupId);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi mở chat riêng: " + ex.Message); }
        }

        private void UpdateUserStatus(bool isOnline)
        {
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    string sql = isOnline ? "UPDATE TAIKHOAN SET TRUC_TUYEN = 1, HOAT_DONG_CUOI = SYSDATE WHERE USERID = :u" : "UPDATE TAIKHOAN SET TRUC_TUYEN = 0 WHERE USERID = :u";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("u", _currentUserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string keyword = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(keyword)) { if (flowGroups.Visible) LoadMyGroups(); else LoadMyFriends(); return; }
                SearchUserInline(keyword);
                e.SuppressKeyPress = true;
            }
        }

        private void SearchUserInline(string keyword)
        {
            flowGroups.Visible = false; flowContacts.Visible = true; flowContacts.Controls.Clear();
            if (keyword == _currentUserId || keyword == UserSession.Email) return;
            try
            {
                string sql = "SELECT USERID, HOTEN FROM TAIKHOAN WHERE EMAIL = :k OR DIENTHOAI = :k OR USERID = :k";
                DataTable dt = new DataTable();
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("k", keyword);
                        new OracleDataAdapter(cmd).Fill(dt);
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    string foundID = dt.Rows[0]["USERID"].ToString();
                    string foundName = dt.Rows[0]["HOTEN"].ToString();
                    UC_ContactItem item = new UC_ContactItem();
                    item.SetData(foundID, foundName); item.Dock = DockStyle.Top;
                    if (CheckIfFriend(foundID)) { item.ShowAddFriendButton(false); }
                    else
                    {
                        item.ShowAddFriendButton(true);
                        item.AddFriendClicked += (s, e) => { SendFriendRequest(foundID); item.ShowAddFriendButton(false); };
                    }
                    flowContacts.Controls.Add(item);
                }
                else { MessageBox.Show("Không tìm thấy!"); if (flowGroups.Visible) LoadMyGroups(); else LoadMyFriends(); }
            }
            catch { }
        }

        private bool CheckIfFriend(string targetID)
        {
            string sql = "SELECT COUNT(*) FROM BANBE WHERE ((USER_ID_1=:me AND USER_ID_2=:u) OR (USER_ID_1=:u AND USER_ID_2=:me))";
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("me", _currentUserId); cmd.Parameters.Add("u", targetID); return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch { return false; }
        }

        private void SendFriendRequest(string targetID)
        {
            string sql = "INSERT INTO BANBE (USER_ID_1, USER_ID_2, TRANGTHAI, NGAYGUI) VALUES (:me, :u, 0, SYSDATE)";
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("me", _currentUserId); cmd.Parameters.Add("u", targetID); cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { MessageBox.Show("Đã có lời mời!"); }
        }

        private void btnAddFriend_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword)) { MessageBox.Show("Nhập ID/Email/SĐT!", "Hướng dẫn"); txtSearch.Focus(); return; }
            SearchUserInline(keyword);
        }

        private void ShowUserInfo() { guna2HtmlLabel1.Text = UserSession.HoTen; }
        private void LoadGroupMembersStatus() { }
        private void guna2Button2_upload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()) { if (ofd.ShowDialog() == DialogResult.OK) { _selectedFilePath = ofd.FileName; txtUser.Text = $"[File: {ofd.SafeFileName}] "; } }
        }
        private void guna2Button1_Click(object sender, EventArgs e) { UpdateUserStatus(false); new DangNhap().Show(); this.Close(); }
        protected override void OnFormClosing(FormClosingEventArgs e) { UpdateUserStatus(false); base.OnFormClosing(e); }
        private void guna2CirclePictureBox1_Click(object sender, EventArgs e) { }
        private void guna2TextBox1_user_TextChanged(object sender, EventArgs e) { }
        private void richTextBox1_TextChanged(object sender, EventArgs e) { }
        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
        private void pnlSidebar_Paint(object sender, PaintEventArgs e) { }
        private void txtSearch_TextChanged(object sender, EventArgs e) { }
        private void flowContacts_Paint(object sender, PaintEventArgs e) { }
    }
}