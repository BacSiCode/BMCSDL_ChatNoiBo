namespace Nhom8
{
    partial class SupAdmin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            tabControl1 = new TabControl();
            tpUserManagement = new TabPage();
            btn_GiamSat = new Button();
            btn_KhoiPhuc = new Button();
            dgvUsers = new Guna.UI2.WinForms.Guna2DataGridView();
            guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            btnViewLoginHistory = new Button();
            btnLock = new Button();
            btnUpdateUser = new Button();
            btnDeleteGroup = new Button();
            btnAddNew = new Button();
            cmbSortUser = new Guna.UI2.WinForms.Guna2ComboBox();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnSearch = new Button();
            txtSearchUser = new TextBox();
            tpGroupManagement = new TabPage();
            dgvGroups = new Guna.UI2.WinForms.Guna2DataGridView();
            btnGroupNotification = new Button();
            btnViewChatHistory = new Button();
            btnManageMembers = new Button();
            btnEditGroup = new Button();
            btnDeleteGroups = new Button();
            btnAddGroup = new Button();
            cmbSortGroup = new Guna.UI2.WinForms.Guna2ComboBox();
            guna2HtmlLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnSearchGroup = new Button();
            txtSearchGroup = new TextBox();
            tabControl1.SuspendLayout();
            tpUserManagement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
            tpGroupManagement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGroups).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tpUserManagement);
            tabControl1.Controls.Add(tpGroupManagement);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Font = new Font("Segoe UI", 10F);
            tabControl1.ItemSize = new Size(150, 40);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(900, 600);
            tabControl1.TabIndex = 0;
            // 
            // tpUserManagement
            // 
            tpUserManagement.Controls.Add(btn_GiamSat);
            tpUserManagement.Controls.Add(btn_KhoiPhuc);
            tpUserManagement.Controls.Add(dgvUsers);
            tpUserManagement.Controls.Add(guna2Button1);
            tpUserManagement.Controls.Add(btnViewLoginHistory);
            tpUserManagement.Controls.Add(btnLock);
            tpUserManagement.Controls.Add(btnUpdateUser);
            tpUserManagement.Controls.Add(btnDeleteGroup);
            tpUserManagement.Controls.Add(btnAddNew);
            tpUserManagement.Controls.Add(cmbSortUser);
            tpUserManagement.Controls.Add(guna2HtmlLabel1);
            tpUserManagement.Controls.Add(btnSearch);
            tpUserManagement.Controls.Add(txtSearchUser);
            tpUserManagement.Location = new Point(4, 44);
            tpUserManagement.Name = "tpUserManagement";
            tpUserManagement.Padding = new Padding(3);
            tpUserManagement.Size = new Size(892, 552);
            tpUserManagement.TabIndex = 0;
            tpUserManagement.Text = "Quản lý người dùng";
            tpUserManagement.UseVisualStyleBackColor = true;
            // 
            // btn_GiamSat
            // 
            btn_GiamSat.BackColor = Color.BurlyWood;
            btn_GiamSat.Location = new Point(732, 70);
            btn_GiamSat.Name = "btn_GiamSat";
            btn_GiamSat.Size = new Size(100, 40);
            btn_GiamSat.TabIndex = 12;
            btn_GiamSat.Text = "Giám sát";
            btn_GiamSat.UseVisualStyleBackColor = false;
            btn_GiamSat.Click += btn_GiamSat_Click;
            // 
            // btn_KhoiPhuc
            // 
            btn_KhoiPhuc.BackColor = Color.Salmon;
            btn_KhoiPhuc.Location = new Point(608, 70);
            btn_KhoiPhuc.Name = "btn_KhoiPhuc";
            btn_KhoiPhuc.Size = new Size(100, 40);
            btn_KhoiPhuc.TabIndex = 11;
            btn_KhoiPhuc.Text = "Khôi phục";
            btn_KhoiPhuc.UseVisualStyleBackColor = false;
            btn_KhoiPhuc.Click += btn_KhoiPhuc_Click;
            // 
            // dgvUsers
            // 
            dataGridViewCellStyle1.BackColor = Color.FromArgb(247, 248, 249);
            dgvUsers.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(232, 234, 237);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvUsers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvUsers.ColumnHeadersHeight = 29;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dataGridViewCellStyle3.SelectionForeColor = Color.Black;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvUsers.DefaultCellStyle = dataGridViewCellStyle3;
            dgvUsers.GridColor = Color.FromArgb(244, 245, 247);
            dgvUsers.Location = new Point(20, 130);
            dgvUsers.Name = "dgvUsers";
            dgvUsers.RowHeadersVisible = false;
            dgvUsers.RowHeadersWidth = 51;
            dgvUsers.Size = new Size(850, 350);
            dgvUsers.TabIndex = 0;
            dgvUsers.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Light;
            dgvUsers.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(247, 248, 249);
            dgvUsers.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvUsers.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvUsers.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvUsers.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvUsers.ThemeStyle.BackColor = Color.White;
            dgvUsers.ThemeStyle.GridColor = Color.FromArgb(244, 245, 247);
            dgvUsers.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(232, 234, 237);
            dgvUsers.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvUsers.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F);
            dgvUsers.ThemeStyle.HeaderStyle.ForeColor = Color.Black;
            dgvUsers.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvUsers.ThemeStyle.HeaderStyle.Height = 29;
            dgvUsers.ThemeStyle.ReadOnly = false;
            dgvUsers.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvUsers.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvUsers.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 10F);
            dgvUsers.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            dgvUsers.ThemeStyle.RowsStyle.Height = 29;
            dgvUsers.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dgvUsers.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            dgvUsers.CellContentClick += dgvUsers_CellContentClick_1;
            // 
            // guna2Button1
            // 
            guna2Button1.BorderRadius = 10;
            guna2Button1.CustomizableEdges = customizableEdges1;
            guna2Button1.FillColor = Color.Salmon;
            guna2Button1.Font = new Font("Segoe UI", 9F);
            guna2Button1.ForeColor = Color.White;
            guna2Button1.Location = new Point(750, 500);
            guna2Button1.Name = "guna2Button1";
            guna2Button1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2Button1.Size = new Size(120, 40);
            guna2Button1.TabIndex = 1;
            guna2Button1.Text = "Đăng xuất";
            guna2Button1.Click += guna2Button1_Click;
            // 
            // btnViewLoginHistory
            // 
            btnViewLoginHistory.BackColor = Color.RosyBrown;
            btnViewLoginHistory.Location = new Point(483, 70);
            btnViewLoginHistory.Name = "btnViewLoginHistory";
            btnViewLoginHistory.Size = new Size(100, 40);
            btnViewLoginHistory.TabIndex = 2;
            btnViewLoginHistory.Text = "Lịch sử";
            btnViewLoginHistory.UseVisualStyleBackColor = false;
            btnViewLoginHistory.Click += button2_Click;
            // 
            // btnLock
            // 
            btnLock.BackColor = Color.Orange;
            btnLock.Location = new Point(359, 70);
            btnLock.Name = "btnLock";
            btnLock.Size = new Size(100, 40);
            btnLock.TabIndex = 3;
            btnLock.Text = "Khóa/Mở";
            btnLock.UseVisualStyleBackColor = false;
            btnLock.Click += btnLock_Click;
            // 
            // btnUpdateUser
            // 
            btnUpdateUser.BackColor = Color.SteelBlue;
            btnUpdateUser.ForeColor = Color.White;
            btnUpdateUser.Location = new Point(130, 70);
            btnUpdateUser.Name = "btnUpdateUser";
            btnUpdateUser.Size = new Size(100, 40);
            btnUpdateUser.TabIndex = 4;
            btnUpdateUser.Text = "Cập nhật";
            btnUpdateUser.UseVisualStyleBackColor = false;
            btnUpdateUser.Click += btnUpdateUser_Click;
            // 
            // btnDeleteGroup
            // 
            btnDeleteGroup.BackColor = Color.Firebrick;
            btnDeleteGroup.ForeColor = Color.White;
            btnDeleteGroup.Location = new Point(240, 70);
            btnDeleteGroup.Name = "btnDeleteGroup";
            btnDeleteGroup.Size = new Size(100, 40);
            btnDeleteGroup.TabIndex = 5;
            btnDeleteGroup.Text = "Xóa User";
            btnDeleteGroup.UseVisualStyleBackColor = false;
            btnDeleteGroup.Click += btnDeleteGroup_Click;
            // 
            // btnAddNew
            // 
            btnAddNew.BackColor = Color.SeaGreen;
            btnAddNew.ForeColor = Color.White;
            btnAddNew.Location = new Point(20, 70);
            btnAddNew.Name = "btnAddNew";
            btnAddNew.Size = new Size(100, 40);
            btnAddNew.TabIndex = 6;
            btnAddNew.Text = "Thêm Mới";
            btnAddNew.UseVisualStyleBackColor = false;
            btnAddNew.Click += btnAddNew_Click;
            // 
            // cmbSortUser
            // 
            cmbSortUser.BackColor = Color.Transparent;
            cmbSortUser.CustomizableEdges = customizableEdges3;
            cmbSortUser.DrawMode = DrawMode.OwnerDrawFixed;
            cmbSortUser.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSortUser.FocusedColor = Color.Empty;
            cmbSortUser.Font = new Font("Segoe UI", 10F);
            cmbSortUser.ForeColor = Color.FromArgb(68, 88, 112);
            cmbSortUser.ItemHeight = 30;
            cmbSortUser.Location = new Point(470, 20);
            cmbSortUser.Name = "cmbSortUser";
            cmbSortUser.ShadowDecoration.CustomizableEdges = customizableEdges4;
            cmbSortUser.Size = new Size(150, 36);
            cmbSortUser.TabIndex = 7;
            cmbSortUser.SelectedIndexChanged += cmbSortUser_SelectedIndexChanged;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Location = new Point(400, 25);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(59, 22);
            guna2HtmlLabel1.TabIndex = 8;
            guna2HtmlLabel1.Text = "Sắp xếp:";
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(280, 20);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(80, 30);
            btnSearch.TabIndex = 9;
            btnSearch.Text = "Tìm";
            btnSearch.Click += button1_Click;
            // 
            // txtSearchUser
            // 
            txtSearchUser.Location = new Point(20, 20);
            txtSearchUser.Name = "txtSearchUser";
            txtSearchUser.PlaceholderText = "Tìm kiếm user...";
            txtSearchUser.Size = new Size(250, 30);
            txtSearchUser.TabIndex = 10;
            txtSearchUser.TextChanged += txtSearchUser_TextChanged_1;
            // 
            // tpGroupManagement
            // 
            tpGroupManagement.Controls.Add(dgvGroups);
            tpGroupManagement.Controls.Add(btnGroupNotification);
            tpGroupManagement.Controls.Add(btnViewChatHistory);
            tpGroupManagement.Controls.Add(btnManageMembers);
            tpGroupManagement.Controls.Add(btnEditGroup);
            tpGroupManagement.Controls.Add(btnDeleteGroups);
            tpGroupManagement.Controls.Add(btnAddGroup);
            tpGroupManagement.Controls.Add(cmbSortGroup);
            tpGroupManagement.Controls.Add(guna2HtmlLabel3);
            tpGroupManagement.Controls.Add(btnSearchGroup);
            tpGroupManagement.Controls.Add(txtSearchGroup);
            tpGroupManagement.Location = new Point(4, 44);
            tpGroupManagement.Name = "tpGroupManagement";
            tpGroupManagement.Padding = new Padding(3);
            tpGroupManagement.Size = new Size(892, 552);
            tpGroupManagement.TabIndex = 1;
            tpGroupManagement.Text = "Quản lý nhóm chat";
            tpGroupManagement.UseVisualStyleBackColor = true;
            // 
            // dgvGroups
            // 
            dataGridViewCellStyle4.BackColor = Color.FromArgb(247, 248, 249);
            dgvGroups.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.FromArgb(232, 234, 237);
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle5.ForeColor = Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            dgvGroups.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dgvGroups.ColumnHeadersHeight = 29;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.White;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle6.ForeColor = Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dataGridViewCellStyle6.SelectionForeColor = Color.Black;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dgvGroups.DefaultCellStyle = dataGridViewCellStyle6;
            dgvGroups.GridColor = Color.FromArgb(244, 245, 247);
            dgvGroups.Location = new Point(20, 130);
            dgvGroups.Name = "dgvGroups";
            dgvGroups.RowHeadersVisible = false;
            dgvGroups.RowHeadersWidth = 51;
            dgvGroups.Size = new Size(850, 350);
            dgvGroups.TabIndex = 0;
            dgvGroups.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Light;
            dgvGroups.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(247, 248, 249);
            dgvGroups.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvGroups.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvGroups.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvGroups.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvGroups.ThemeStyle.BackColor = Color.White;
            dgvGroups.ThemeStyle.GridColor = Color.FromArgb(244, 245, 247);
            dgvGroups.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(232, 234, 237);
            dgvGroups.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvGroups.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F);
            dgvGroups.ThemeStyle.HeaderStyle.ForeColor = Color.Black;
            dgvGroups.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvGroups.ThemeStyle.HeaderStyle.Height = 29;
            dgvGroups.ThemeStyle.ReadOnly = false;
            dgvGroups.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvGroups.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvGroups.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 10F);
            dgvGroups.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            dgvGroups.ThemeStyle.RowsStyle.Height = 29;
            dgvGroups.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(239, 241, 243);
            dgvGroups.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            // 
            // btnGroupNotification
            // 
            btnGroupNotification.Location = new Point(694, 70);
            btnGroupNotification.Name = "btnGroupNotification";
            btnGroupNotification.Size = new Size(134, 40);
            btnGroupNotification.TabIndex = 1;
            btnGroupNotification.Text = "Thông báo";
            // 
            // btnViewChatHistory
            // 
            btnViewChatHistory.Location = new Point(568, 70);
            btnViewChatHistory.Name = "btnViewChatHistory";
            btnViewChatHistory.Size = new Size(100, 40);
            btnViewChatHistory.TabIndex = 2;
            btnViewChatHistory.Text = "Lịch sử";
            btnViewChatHistory.Click += btnViewChatHistory_Click_1;
            // 
            // btnManageMembers
            // 
            btnManageMembers.Location = new Point(432, 70);
            btnManageMembers.Name = "btnManageMembers";
            btnManageMembers.Size = new Size(109, 40);
            btnManageMembers.TabIndex = 3;
            btnManageMembers.Text = "Thành viên";
            btnManageMembers.Click += btnManageMembers_Click_1;
            // 
            // btnEditGroup
            // 
            btnEditGroup.Location = new Point(170, 70);
            btnEditGroup.Name = "btnEditGroup";
            btnEditGroup.Size = new Size(100, 40);
            btnEditGroup.TabIndex = 4;
            btnEditGroup.Text = "Sửa Nhóm";
            btnEditGroup.Click += btnEditGroup_Click_1;
            // 
            // btnDeleteGroups
            // 
            btnDeleteGroups.Location = new Point(306, 70);
            btnDeleteGroups.Name = "btnDeleteGroups";
            btnDeleteGroups.Size = new Size(100, 40);
            btnDeleteGroups.TabIndex = 5;
            btnDeleteGroups.Text = "Xóa Nhóm";
            btnDeleteGroups.Click += btnDeleteGroups_Click;
            // 
            // btnAddGroup
            // 
            btnAddGroup.Location = new Point(20, 70);
            btnAddGroup.Name = "btnAddGroup";
            btnAddGroup.Size = new Size(119, 40);
            btnAddGroup.TabIndex = 6;
            btnAddGroup.Text = "Thêm Nhóm";
            btnAddGroup.Click += btnAddGroup_Click;
            // 
            // cmbSortGroup
            // 
            cmbSortGroup.BackColor = Color.Transparent;
            cmbSortGroup.CustomizableEdges = customizableEdges5;
            cmbSortGroup.DrawMode = DrawMode.OwnerDrawFixed;
            cmbSortGroup.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSortGroup.FocusedColor = Color.Empty;
            cmbSortGroup.Font = new Font("Segoe UI", 10F);
            cmbSortGroup.ForeColor = Color.FromArgb(68, 88, 112);
            cmbSortGroup.ItemHeight = 30;
            cmbSortGroup.Location = new Point(470, 20);
            cmbSortGroup.Name = "cmbSortGroup";
            cmbSortGroup.ShadowDecoration.CustomizableEdges = customizableEdges6;
            cmbSortGroup.Size = new Size(150, 36);
            cmbSortGroup.TabIndex = 7;
            // 
            // guna2HtmlLabel3
            // 
            guna2HtmlLabel3.BackColor = Color.Transparent;
            guna2HtmlLabel3.Location = new Point(400, 25);
            guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            guna2HtmlLabel3.Size = new Size(59, 22);
            guna2HtmlLabel3.TabIndex = 8;
            guna2HtmlLabel3.Text = "Sắp xếp:";
            // 
            // btnSearchGroup
            // 
            btnSearchGroup.Location = new Point(280, 20);
            btnSearchGroup.Name = "btnSearchGroup";
            btnSearchGroup.Size = new Size(80, 30);
            btnSearchGroup.TabIndex = 9;
            btnSearchGroup.Text = "Tìm";
            btnSearchGroup.Click += btnSearchGroup_Click;
            // 
            // txtSearchGroup
            // 
            txtSearchGroup.Location = new Point(20, 20);
            txtSearchGroup.Name = "txtSearchGroup";
            txtSearchGroup.Size = new Size(250, 30);
            txtSearchGroup.TabIndex = 10;
            // 
            // SupAdmin
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 600);
            Controls.Add(tabControl1);
            Name = "SupAdmin";
            Text = "Super Admin";
            Load += Admin_Load;
            tabControl1.ResumeLayout(false);
            tpUserManagement.ResumeLayout(false);
            tpUserManagement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
            tpGroupManagement.ResumeLayout(false);
            tpGroupManagement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGroups).EndInit();
            ResumeLayout(false);
        }

        #endregion

        // Giữ nguyên tên biến cũ của bạn
        private TabControl tabControl1;
        private TabPage tpUserManagement, tpGroupManagement;

        private Guna.UI2.WinForms.Guna2DataGridView dgvUsers, dgvGroups;

        private TextBox txtSearchUser, txtSearchGroup;
        private Button btnSearch, btnSearchGroup;

        private Guna.UI2.WinForms.Guna2ComboBox cmbSortUser, cmbSortGroup;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1, guna2HtmlLabel3;
        private Guna.UI2.WinForms.Guna2Button guna2Button1; // Logout

        private Button btnAddNew, btnDeleteGroup, btnUpdateUser, btnLock, btnViewLoginHistory; // User Buttons
        private Button btnAddGroup, btnDeleteGroups, btnEditGroup, btnManageMembers, btnViewChatHistory, btnGroupNotification; // Group Buttons
        private Button btn_GiamSat;
        private Button btn_KhoiPhuc;
    }
}