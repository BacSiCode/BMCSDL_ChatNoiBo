// Trong file KhoiPhucTaiKhoan.cs

using BaoCao; // Đảm bảo bạn có thể truy cập lớp DataBase
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Nhom8
{
    public partial class KhoiPhucTaiKhoan : Form
    {
        public KhoiPhucTaiKhoan()
        {
            InitializeComponent();
            this.Load += KhoiPhucTaiKhoan_Load;
            CenterToScreen();
        }

        private void KhoiPhucTaiKhoan_Load(object sender, EventArgs e)
        {
            LoadDeletedUserData();
        }

        private void LoadDeletedUserData()
        {
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    string sql = @"
                        SELECT 
                            USERID_DELETED AS ""Mã NV"", 
                            USERNAME_DELETED AS ""Tài khoản"", 
                            HOTEN_DELETED AS ""Họ tên"", 
                            THOIGIAN_XOA AS ""Thời gian xóa"", 
                            XOA_BOI AS ""Xóa bởi""
                        FROM LOG_DELETE_TAIKHOAN
                        ORDER BY THOIGIAN_XOA DESC";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvDeletedUsers.DataSource = dt;
                        dgvDeletedUsers.Columns[0].Width = 70;
                        dgvDeletedUsers.Columns[3].Width = 150;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách xóa: " + ex.Message, "Lỗi");
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (dgvDeletedUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản để khôi phục.", "Thông báo");
                return;
            }
            string userIdToRestore = dgvDeletedUsers.SelectedRows[0].Cells["Mã NV"].Value.ToString();
            string uname = dgvDeletedUsers.SelectedRows[0].Cells["Tài khoản"].Value.ToString();

            if (MessageBox.Show($"Xác nhận khôi phục tài khoản '{uname}'?", "Xác nhận khôi phục",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                PerformRestore(userIdToRestore);
            }
        }

        private void PerformRestore(string userId)
        {
            try
            {
                using (OracleConnection conn = DataBase.GetConnection())
                {
                    using (OracleCommand cmd = new OracleCommand("NHOM8.SP_RESTORE_USER", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_userid_khoi_phuc", OracleDbType.Varchar2).Value = userId;
                        cmd.ExecuteNonQuery();
                    }


                    MessageBox.Show($"Khôi phục tài khoản '{userId}' thành công!", "Hoàn tất");
                    LoadDeletedUserData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khôi phục: " + ex.Message, "Lỗi");
            }
        }
    }
}