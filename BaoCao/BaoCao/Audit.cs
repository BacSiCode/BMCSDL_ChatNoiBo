using BaoCao;
using Oracle.ManagedDataAccess.Client;
using System.Data;
// Đảm bảo bạn đã cài gói NuGet Microsoft.Office.Interop.Excel
using Excel = Microsoft.Office.Interop.Excel;

namespace Nhom8
{
    public partial class Audit : Form
    {
        public Audit()
        {
            InitializeComponent();
            CenterToScreen();

            // 1. Tải chế độ Audit vào cbo_User
            LoadComboBoxData();

            // 2. Thiết lập giá trị mặc định cho DatePicker
            dtp_TuNgay.Value = DateTime.Now.AddDays(-30);
            dtp_DenNgay.Value = DateTime.Now;

            // 3. Khởi tạo trạng thái hiển thị ban đầu (phải chạy sau LoadComboBoxData)
            UpdateFilterVisibility(cbo_User.SelectedItem?.ToString());

            // 4. Tải dữ liệu ban đầu
            LoadData();
        }

        // HÀM 1: Tải các chế độ Audit vào cbo_User (control chọn mode)
        private void LoadComboBoxData()
        {
            cbo_User.Items.Clear();
            cbo_User.Items.Add("Trigger");      // Custom Audit Log (log_audit)
            cbo_User.Items.Add("Standard");     // Standard Table (TAIKHOAN)
            cbo_User.SelectedIndex = 0;
        }

        // HÀM 2: Cập nhật trạng thái hiển thị của các trường lọc
        private void UpdateFilterVisibility(string selectedMode)
        {
            bool isLogMode = selectedMode?.Contains("Trigger") ?? true;
            dtp_TuNgay.Visible = isLogMode;
            dtp_DenNgay.Visible = isLogMode;
            txtAction.Visible = isLogMode;
        }

        // HÀM 3: Tải và hiển thị dữ liệu
        private void LoadData()
        {
            string selectedMode = cbo_User.SelectedItem?.ToString() ?? "Trigger";
            string filterUser = txtUser.Text.Trim();
            string filterAction = txtAction.Text.Trim();

            DateTime tuNgay = dtp_TuNgay.Value.Date;
            DateTime denNgay = dtp_DenNgay.Value.Date.AddDays(1);

            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    string sql = "";
                    string searchSql = "";

                    if (selectedMode.Contains("Trigger"))
                    {
                        sql = @"
                            SELECT
                                USER_NAME AS ""Người thực hiện"",
                                ACTION_TIME AS ""Thời gian"",
                                ACTION_TYPE AS ""Hành động"",
                                TABLE_NAME AS ""Bảng"",
                                OLD_DATA AS ""Dữ liệu cũ"",
                                NEW_DATA AS ""Dữ liệu mới""
                            FROM log_audit
                            WHERE ACTION_TIME >= :p_tu_ngay AND ACTION_TIME < :p_den_ngay";

                        if (!string.IsNullOrEmpty(filterUser))
                            searchSql += " AND LOWER(USER_NAME) LIKE LOWER(:p_user_name)";

                        if (!string.IsNullOrEmpty(filterAction))
                            searchSql += " AND LOWER(ACTION_TYPE) LIKE LOWER(:p_action_type)";

                        sql += searchSql + " ORDER BY ACTION_TIME DESC";
                    }

                    else if (selectedMode.Contains("Standard"))
                    {
                        sql = @"
                            SELECT USERID AS ""Mã NV"", 
                                   USERNAME AS ""Tên đăng nhập"", 
                                   HOTEN AS ""Họ và tên"", 
                                   CHUC_VU AS ""Chức vụ"", 
                                   TRANGTHAI AS ""Trạng thái""
                            FROM TAIKHOAN";
                        if (!string.IsNullOrEmpty(filterUser))
                            searchSql += @" WHERE LOWER(USERID) LIKE LOWER(:p_user_name) 
                                           OR LOWER(USERNAME) LIKE LOWER(:p_user_name) 
                                           OR LOWER(HOTEN) LIKE LOWER(:p_user_name)";

                        sql += searchSql + " ORDER BY USERID ASC";
                    }

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        if (selectedMode.Contains("Trigger"))
                        {
                            cmd.Parameters.Add("p_tu_ngay", OracleDbType.Date).Value = tuNgay;
                            cmd.Parameters.Add("p_den_ngay", OracleDbType.Date).Value = denNgay;

                            if (!string.IsNullOrEmpty(filterUser))
                                cmd.Parameters.Add("p_user_name", OracleDbType.Varchar2).Value = "%" + filterUser + "%";

                            if (!string.IsNullOrEmpty(filterAction))
                                cmd.Parameters.Add("p_action_type", OracleDbType.Varchar2).Value = "%" + filterAction + "%";
                        }
                        else if (selectedMode.Contains("Standard") && !string.IsNullOrEmpty(filterUser))
                        {
                            cmd.Parameters.Add("p_user_name", OracleDbType.Varchar2).Value = "%" + filterUser + "%";
                        }

                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dgvAudit.DataSource = dt;
                        dgvAudit.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi");
            }
        }

        // --- XUẤT EXCEL (HÀM BẠN ĐÃ CUNG CẤP VÀ ĐÃ TỐI ƯU) ---
        private void btnExcel_Click(object sender, EventArgs e)
        {
            if (dgvAudit.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                excelApp = new Excel.Application();
                workbook = excelApp.Workbooks.Add(Type.Missing);
                worksheet = (Excel.Worksheet)workbook.ActiveSheet;
                worksheet.Name = "DuLieuLog";

                for (int i = 0; i < dgvAudit.Columns.Count; i++)
                {
                    Excel.Range headerCell = (Excel.Range)worksheet.Cells[1, i + 1];
                    headerCell.Value = dgvAudit.Columns[i].HeaderText;
                    headerCell.Font.Bold = true;
                } 

                for (int i = 0; i < dgvAudit.Rows.Count; i++)
                {
                    for (int j = 0; j < dgvAudit.Columns.Count; j++)
                    {
                        if (dgvAudit.Rows[i].Cells[j].Value != null)
                        {
                            worksheet.Cells[i + 2, j + 1] = dgvAudit.Rows[i].Cells[j].Value.ToString();
                        }
                    }
                }
                worksheet.Columns.AutoFit();
                excelApp.Visible = true;

                MessageBox.Show("Xuất dữ liệu Audit Log sang Excel thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (workbook != null) workbook.Close(false);
                if (excelApp != null) excelApp.Quit();

                MessageBox.Show("Lỗi xuất Excel: " + ex.Message + "\n(Vui lòng đảm bảo đã cài đặt Microsoft Excel và gói NuGet)", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                if (workbook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            }
        }


        private void txtUser_TextChanged(object sender, EventArgs e) { }
        private void txtAction_TextChanged(object sender, EventArgs e) { }
        private void dgvAudit_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void cbo_User_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFilterVisibility(cbo_User.SelectedItem?.ToString());
            LoadData();
        }

        private void dtp_TuNgay_ValueChanged(object sender, EventArgs e)// Loc date tu ngay
        {
            LoadData();
        }

        private void dtp_DenNgay_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnLoad_Click(object sender, EventArgs e)// Button de load du lieu theo tieu chi tim kiem
        {
            LoadData();
        }

        private void btnClose_Click(object sender, EventArgs e)// Button de quay ve lai form truoc do la Supadmin
        {
            this.Close();
        }
    }
}