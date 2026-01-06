namespace Nhom8
{
    public partial class UC_ContactItem : UserControl
    {
        public event EventHandler AddFriendClicked;
        public string GroupID { get; set; }

        public UC_ContactItem()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            AddFriendClicked?.Invoke(this, e);
        }

        public void SetData(string groupId, string groupName)
        {
            this.GroupID = groupId;
            if (lblGroupName != null)
            {
                lblGroupName.Text = groupName;
            }
        }
        public void ShowAddFriendButton(bool isShow)
        {
            if (btnAdd != null)
            {
                btnAdd.Visible = isShow;
            }
        }

    }
}