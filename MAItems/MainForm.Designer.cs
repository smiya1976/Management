namespace MAItems
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDataSync;

        private System.Windows.Forms.Button btnToggleNumeric;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnFirstPage;
        private System.Windows.Forms.Button btnPrevPage;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Button btnLastPage;
        private System.Windows.Forms.Label lblPageInfo;
        private System.Windows.Forms.ComboBox cmbPageSize;
        private System.Windows.Forms.Label lblPageSize;
        private System.Windows.Forms.CheckBox chkFilterProcessing;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            lblSearch = new Label();
            txtSearch = new TextBox();
            btnSearch = new Button();
            btnClear = new Button();
            btnDetail = new Button();
            btnAdd = new Button();
            btnDataSync = new Button();
            btnToggleNumeric = new Button();
            btnDelete = new Button();
            dgvData = new DataGridView();
            lblStatus = new Label();
            btnFirstPage = new Button();
            btnPrevPage = new Button();
            btnNextPage = new Button();
            btnLastPage = new Button();
            lblPageInfo = new Label();
            cmbPageSize = new ComboBox();
            lblPageSize = new Label();
            chkFilterProcessing = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)dgvData).BeginInit();
            SuspendLayout();
            // 
            // lblSearch
            // 
            lblSearch.Location = new Point(16, 18);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(40, 23);
            lblSearch.TabIndex = 0;
            lblSearch.Text = "検索:";
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(62, 15);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(160, 23);
            txtSearch.TabIndex = 1;
            txtSearch.KeyDown += txtSearch_KeyDown;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(346, 13);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(100, 28);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "🔍 検索";
            btnSearch.Click += btnSearch_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(452, 13);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(80, 28);
            btnClear.TabIndex = 4;
            btnClear.Text = "クリア";
            btnClear.Click += btnClear_Click;
            // 
            // btnDetail
            // 
            btnDetail.BackColor = Color.LightYellow;
            btnDetail.Enabled = false;
            btnDetail.Location = new Point(538, 13);
            btnDetail.Name = "btnDetail";
            btnDetail.Size = new Size(120, 28);
            btnDetail.TabIndex = 5;
            btnDetail.Text = "📋 案件詳細";
            btnDetail.UseVisualStyleBackColor = false;
            btnDetail.Click += btnDetail_Click;
            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Location = new Point(664, 13);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(120, 28);
            btnAdd.TabIndex = 6;
            btnAdd.Text = "➕ 新規追加";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnDataSync
            // 
            btnDataSync.BackColor = Color.LightGray;
            btnDataSync.Location = new Point(790, 13);
            btnDataSync.Name = "btnDataSync";
            btnDataSync.Size = new Size(140, 28);
            btnDataSync.TabIndex = 7;
            btnDataSync.Text = "⚙ データ管理";
            btnDataSync.UseVisualStyleBackColor = false;
            btnDataSync.Click += btnDataSync_Click;
            // 
            // btnToggleNumeric
            // 
            btnToggleNumeric.BackColor = Color.LightCyan;
            btnToggleNumeric.Location = new Point(940, 13);
            btnToggleNumeric.Name = "btnToggleNumeric";
            btnToggleNumeric.Size = new Size(130, 28);
            btnToggleNumeric.TabIndex = 8;
            btnToggleNumeric.Text = "🔢 数値モード";
            btnToggleNumeric.UseVisualStyleBackColor = false;
            btnToggleNumeric.Click += btnToggleNumeric_Click;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.BackColor = Color.LightCoral;
            btnDelete.Location = new Point(1160, 13);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(96, 28);
            btnDelete.TabIndex = 9;
            btnDelete.Text = "🗑 削除";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // dgvData
            // 
            dgvData.AllowUserToAddRows = false;
            dgvData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvData.ColumnHeadersHeight = 28;
            dgvData.Location = new Point(16, 52);
            dgvData.Name = "dgvData";
            dgvData.RowTemplate.Height = 24;
            dgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvData.Size = new Size(1240, 508);
            dgvData.TabIndex = 10;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.Location = new Point(16, 604);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(1240, 23);
            lblStatus.TabIndex = 18;
            lblStatus.Text = "準備完了";
            // 
            // btnFirstPage
            // 
            btnFirstPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnFirstPage.Location = new Point(460, 564);
            btnFirstPage.Name = "btnFirstPage";
            btnFirstPage.Size = new Size(40, 28);
            btnFirstPage.TabIndex = 13;
            btnFirstPage.Text = "⏮";
            btnFirstPage.Click += btnFirstPage_Click;
            // 
            // btnPrevPage
            // 
            btnPrevPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnPrevPage.Location = new Point(506, 564);
            btnPrevPage.Name = "btnPrevPage";
            btnPrevPage.Size = new Size(40, 28);
            btnPrevPage.TabIndex = 14;
            btnPrevPage.Text = "◀";
            btnPrevPage.Click += btnPrevPage_Click;
            // 
            // btnNextPage
            // 
            btnNextPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnNextPage.Location = new Point(758, 564);
            btnNextPage.Name = "btnNextPage";
            btnNextPage.Size = new Size(40, 28);
            btnNextPage.TabIndex = 16;
            btnNextPage.Text = "▶";
            btnNextPage.Click += btnNextPage_Click;
            // 
            // btnLastPage
            // 
            btnLastPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLastPage.Location = new Point(804, 564);
            btnLastPage.Name = "btnLastPage";
            btnLastPage.Size = new Size(40, 28);
            btnLastPage.TabIndex = 17;
            btnLastPage.Text = "⏭";
            btnLastPage.Click += btnLastPage_Click;
            // 
            // lblPageInfo
            // 
            lblPageInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblPageInfo.Location = new Point(552, 568);
            lblPageInfo.Name = "lblPageInfo";
            lblPageInfo.Size = new Size(200, 23);
            lblPageInfo.TabIndex = 15;
            lblPageInfo.Text = "1 / 1 ページ（0 件）";
            lblPageInfo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmbPageSize
            // 
            cmbPageSize.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmbPageSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPageSize.Location = new Point(90, 566);
            cmbPageSize.Name = "cmbPageSize";
            cmbPageSize.Size = new Size(70, 23);
            cmbPageSize.TabIndex = 12;
            cmbPageSize.SelectedIndexChanged += cmbPageSize_SelectedIndexChanged;
            // 
            // lblPageSize
            // 
            lblPageSize.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblPageSize.Location = new Point(16, 568);
            lblPageSize.Name = "lblPageSize";
            lblPageSize.Size = new Size(70, 23);
            lblPageSize.TabIndex = 11;
            lblPageSize.Text = "表示件数:";
            lblPageSize.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // chkFilterProcessing
            // 
            chkFilterProcessing.Location = new Point(230, 16);
            chkFilterProcessing.Name = "chkFilterProcessing";
            chkFilterProcessing.Size = new Size(110, 23);
            chkFilterProcessing.TabIndex = 2;
            chkFilterProcessing.Text = "処理中のみ";
            chkFilterProcessing.Click += chkFilterProcessing_CheckedChanged;
            // 
            // MainForm
            // 
            ClientSize = new Size(1264, 641);
            Controls.Add(chkFilterProcessing);
            Controls.Add(lblSearch);
            Controls.Add(txtSearch);
            Controls.Add(btnSearch);
            Controls.Add(btnClear);
            Controls.Add(btnDetail);
            Controls.Add(btnAdd);
            Controls.Add(btnDataSync);
            Controls.Add(btnToggleNumeric);
            Controls.Add(btnDelete);
            Controls.Add(dgvData);
            Controls.Add(lblPageSize);
            Controls.Add(cmbPageSize);
            Controls.Add(btnFirstPage);
            Controls.Add(btnPrevPage);
            Controls.Add(lblPageInfo);
            Controls.Add(btnNextPage);
            Controls.Add(btnLastPage);
            Controls.Add(lblStatus);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(900, 500);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "M&A案件管理";
            ((System.ComponentModel.ISupportInitialize)dgvData).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
    }
}