using System.Data;

namespace Nhom8
{
    public partial class FormDataView : Form
    {
        public FormDataView()
        {
            InitializeComponent();
            CenterToScreen();
        }

        private void FormDataView_Load(object sender, EventArgs e)
        {

        }
        public void HienThiDuLieu(string title, DataTable dt)
        {
            this.Text = title;
            if (dgvData != null)
            {
                dgvData.DataSource = dt;
                if (dt == null || dt.Rows.Count == 0)
                {
                    this.Text += " (Trống)";
                }
                else
                {
                    this.Text += $" ({dt.Rows.Count} dòng)";
                }
            }
        }
    }
}
