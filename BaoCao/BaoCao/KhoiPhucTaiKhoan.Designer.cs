namespace Nhom8
{
    partial class KhoiPhucTaiKhoan
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvDeletedUsers = new System.Windows.Forms.DataGridView();
            this.btnRestore = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeletedUsers)).BeginInit();
            this.SuspendLayout();

            // 
            // dgvDeletedUsers
            // 
            this.dgvDeletedUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDeletedUsers.Location = new System.Drawing.Point(12, 50);
            this.dgvDeletedUsers.Name = "dgvDeletedUsers";
            this.dgvDeletedUsers.Size = new System.Drawing.Size(776, 330);
            this.dgvDeletedUsers.TabIndex = 0;
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(688, 390);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(100, 35);
            this.btnRestore.TabIndex = 1;
            this.btnRestore.Text = "Khôi phục tài khoản";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(12, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(262, 25);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "DANH SÁCH TÀI KHOẢN ĐÃ XÓA";
            // 
            // KhoiPhucTaiKhoan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.dgvDeletedUsers);
            this.Text = "KhoiPhucTaiKhoan";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeletedUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.DataGridView dgvDeletedUsers;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Label lblTitle;
        #endregion
    }
}