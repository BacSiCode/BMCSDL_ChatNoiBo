namespace Nhom8
{
    partial class UC_ContactItem
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            lblGroupName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnAdd = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // lblGroupName
            // 
            lblGroupName.AutoSize = false;
            lblGroupName.BackColor = Color.Transparent;
            lblGroupName.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblGroupName.ForeColor = Color.White;
            lblGroupName.Location = new Point(0, 20);
            lblGroupName.Name = "lblGroupName";
            lblGroupName.Size = new Size(194, 25);
            lblGroupName.TabIndex = 1;
            lblGroupName.Text = "User Name";
            lblGroupName.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // btnAdd
            // 
            btnAdd.BorderRadius = 5;
            btnAdd.CustomizableEdges = customizableEdges1;
            btnAdd.DisabledState.BorderColor = Color.DarkGray;
            btnAdd.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAdd.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAdd.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAdd.FillColor = Color.SeaGreen;
            btnAdd.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(200, 20);
            btnAdd.Name = "btnAdd";
            btnAdd.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnAdd.Size = new Size(35, 30);
            btnAdd.TabIndex = 3;
            btnAdd.Text = "+";
            btnAdd.Visible = false;
            btnAdd.Click += btnAdd_Click;
            // 
            // UC_ContactItem
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(btnAdd);
            Controls.Add(lblGroupName);
            Name = "UC_ContactItem";
            Size = new Size(240, 70);
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel lblGroupName;
        private Guna.UI2.WinForms.Guna2Button btnAdd;
    }
}