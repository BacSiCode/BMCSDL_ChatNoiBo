using BaoCao;
namespace Nhom8
{
    public partial class Dangnhaporcl : Form
    {
        public Dangnhaporcl()
        {
            InitializeComponent();
            this.CenterToScreen();
        }
        bool Check_Textbox(string host, string port, string sid, string user, string password)
        {
            if (string.IsNullOrEmpty(host))
            {
                MessageBox.Show("Chưa nhập Host IP");
                txt_host.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(port))
            {
                MessageBox.Show("Chưa nhập Port");
                txt_port.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(sid))
            {
                MessageBox.Show("Chưa nhập SID");
                txt_sid.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(user))
            {
                MessageBox.Show("Chưa nhập Username");
                txt_user.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Chưa nhập Password");
                txt_pass.Focus();
                return false;
            }
            return true;
        }
        private void PerformLogin()
        {
            string host = txt_host.Text.Trim();
            string port = txt_port.Text.Trim();
            string sid = txt_sid.Text.Trim();
            string user = txt_user.Text.Trim();
            string password = txt_pass.Text;
            if (Check_Textbox(host, port, sid, user, password))
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    button1.Enabled = false;

                    DataBase.Set_Database(host, port, sid, user, password);

                    if (DataBase.Connect())
                    {
                        Cursor.Current = Cursors.Default;
                        button1.Enabled = true;

                        MessageBox.Show("Kết nối thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        new DangNhap().Show();
                        this.Hide();
                    }
                    else
                    {
                        Cursor.Current = Cursors.Default;
                        button1.Enabled = true;
                        MessageBox.Show("Kết nối thất bại. Vui lòng kiểm tra lại thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    button1.Enabled = true;
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            PerformLogin();
        }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void Dangnhaporcl_Load(object sender, EventArgs e) { }
        private void guna2PictureBox1_Click(object sender, EventArgs e) { }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }
    }
}