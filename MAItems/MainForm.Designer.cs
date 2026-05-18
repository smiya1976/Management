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

        private void InitializeComponent()
        {
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnDetail = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDataSync = new System.Windows.Forms.Button();
            this.btnToggleNumeric = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnFirstPage = new System.Windows.Forms.Button();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.btnLastPage = new System.Windows.Forms.Button();
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.cmbPageSize = new System.Windows.Forms.ComboBox();
            this.lblPageSize = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // ── フォーム ──────────────────────────────────
            this.Text = "M&A案件管理";
            this.Size = new System.Drawing.Size(1280, 680);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;

            // ── 検索バー（上部固定） ──────────────────────
            // 上・左 に固定
            var anchorTopLeft =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left;

            // 上・左・右 に固定（横幅追随）
            var anchorTopLeftRight =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;

            // 上・右 に固定
            var anchorTopRight =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Right;

            // 下・左 に固定
            var anchorBottomLeft =
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left;

            // 下・左・右 に固定（横幅追随）
            var anchorBottomLeftRight =
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;

            // 下・右 に固定
            var anchorBottomRight =
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Right;

            // 上下左右（全方向に伸縮）
            var anchorAll =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;

            // lblSearch
            this.lblSearch.Text = "検索:";
            this.lblSearch.Location = new System.Drawing.Point(16, 18);
            this.lblSearch.Size = new System.Drawing.Size(40, 23);
            this.lblSearch.Anchor = anchorTopLeft;

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(62, 15);
            this.txtSearch.Size = new System.Drawing.Size(220, 23);
            this.txtSearch.Anchor = anchorTopLeft;
            this.txtSearch.KeyDown +=
                new System.Windows.Forms.KeyEventHandler(
                    this.txtSearch_KeyDown);

            // btnSearch
            this.btnSearch.Text = "🔍 検索";
            this.btnSearch.Location = new System.Drawing.Point(292, 13);
            this.btnSearch.Size = new System.Drawing.Size(100, 28); // 80 -> 100
            this.btnSearch.Anchor = anchorTopLeft;
            this.btnSearch.Click +=
                new System.EventHandler(this.btnSearch_Click);

            // btnClear
            this.btnClear.Text = "クリア";
            this.btnClear.Location = new System.Drawing.Point(402, 13); // 382 -> 402
            this.btnClear.Size = new System.Drawing.Size(80, 28); // 70 -> 80
            this.btnClear.Anchor = anchorTopLeft;
            this.btnClear.Click +=
                new System.EventHandler(this.btnClear_Click);

            // btnDetail
            this.btnDetail.Text = "📋 案件詳細";
            this.btnDetail.Location = new System.Drawing.Point(492, 13);
            this.btnDetail.Size = new System.Drawing.Size(120, 28);
            this.btnDetail.BackColor = System.Drawing.Color.LightYellow;
            this.btnDetail.Enabled = false;
            this.btnDetail.Anchor = anchorTopLeft;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);

            // btnAdd
            this.btnAdd.Text = "➕ 新規追加";
            this.btnAdd.Location = new System.Drawing.Point(622, 13);
            this.btnAdd.Size = new System.Drawing.Size(120, 28);
            this.btnAdd.BackColor = System.Drawing.Color.LightGreen;
            this.btnAdd.Anchor = anchorTopLeft;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            // 🛠 追加：インポート・エクスポートを統合したデータ管理ボタンを配置
            this.btnDataSync = new System.Windows.Forms.Button();
            this.btnDataSync.Text = "⚙ データ管理";
            this.btnDataSync.Location = new System.Drawing.Point(752, 13);
            this.btnDataSync.Size = new System.Drawing.Size(140, 28);
            this.btnDataSync.BackColor = System.Drawing.Color.LightGray;
            this.btnDataSync.Anchor = anchorTopLeft;
            this.btnDataSync.Click += new System.EventHandler(this.btnDataSync_Click);

            // btnToggleNumeric (位置をずらして再調整)
            this.btnToggleNumeric.Text = "🔢 数値モード";
            this.btnToggleNumeric.Location = new System.Drawing.Point(902, 13); // 1022 -> 902
            this.btnToggleNumeric.Size = new System.Drawing.Size(130, 28);
            this.btnToggleNumeric.BackColor = System.Drawing.Color.LightCyan;
            this.btnToggleNumeric.Anchor = anchorTopLeft;
            this.btnToggleNumeric.Click += new System.EventHandler(this.btnToggleNumeric_Click);

            // btnDelete (右上に固定)
            this.btnDelete.Text = "🗑 削除";
            this.btnDelete.Location = new System.Drawing.Point(1160, 13);
            this.btnDelete.Size = new System.Drawing.Size(96, 28);
            this.btnDelete.BackColor = System.Drawing.Color.LightCoral;
            this.btnDelete.Anchor = anchorTopRight; 
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            // フォームへの追加配列に btnDataSync を反映させ、古い btnImport/btnExport は除去します
            this.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.lblSearch,    this.txtSearch,
                this.btnSearch,    this.btnClear,
                this.btnDetail,    this.btnAdd,
                this.btnDataSync,  this.btnToggleNumeric, this.btnDelete, // ← btnDataSync に差替え
                this.dgvData,
                this.lblPageSize,  this.cmbPageSize,
                this.btnFirstPage, this.btnPrevPage,
                this.lblPageInfo,
                this.btnNextPage,  this.btnLastPage,
                this.lblStatus,
            });

            // ── DataGridView（上下左右に伸縮） ────────────
            this.dgvData.Location =
                new System.Drawing.Point(16, 52);
            this.dgvData.Size =
                new System.Drawing.Size(1240, 508);
            this.dgvData.Anchor = anchorAll; // ✅ 全方向伸縮
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.ReadOnly = false;
            this.dgvData.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this.dgvData.ScrollBars =
                System.Windows.Forms.ScrollBars.Both;
            this.dgvData.ColumnHeadersHeight = 28;
            this.dgvData.RowTemplate.Height = 24;

            // ── ページングバー（下部固定） ─────────────────

            // 表示件数ラベル
            this.lblPageSize.Text = "表示件数:";
            this.lblPageSize.Location =
                new System.Drawing.Point(16, 568);
            this.lblPageSize.Size =
                new System.Drawing.Size(70, 23);
            this.lblPageSize.TextAlign =
                System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPageSize.Anchor = anchorBottomLeft;

            // 表示件数コンボ
            this.cmbPageSize.Location =
                new System.Drawing.Point(90, 566);
            this.cmbPageSize.Size =
                new System.Drawing.Size(70, 23);
            this.cmbPageSize.DropDownStyle =
                System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPageSize.Anchor = anchorBottomLeft;
            this.cmbPageSize.SelectedIndexChanged +=
                new System.EventHandler(
                    this.cmbPageSize_SelectedIndexChanged);

            // 先頭ページボタン
            this.btnFirstPage.Text = "⏮";
            this.btnFirstPage.Location =
                new System.Drawing.Point(460, 564);
            this.btnFirstPage.Size =
                new System.Drawing.Size(40, 28);
            this.btnFirstPage.Anchor = anchorBottomLeft;
            this.btnFirstPage.Click +=
                new System.EventHandler(this.btnFirstPage_Click);

            // 前ページボタン
            this.btnPrevPage.Text = "◀";
            this.btnPrevPage.Location =
                new System.Drawing.Point(506, 564);
            this.btnPrevPage.Size =
                new System.Drawing.Size(40, 28);
            this.btnPrevPage.Anchor = anchorBottomLeft;
            this.btnPrevPage.Click +=
                new System.EventHandler(this.btnPrevPage_Click);

            // ページ情報ラベル（下・左右中央に追随）
            this.lblPageInfo.Text = "1 / 1 ページ（0 件）";
            this.lblPageInfo.Location =
                new System.Drawing.Point(552, 568);
            this.lblPageInfo.Size =
                new System.Drawing.Size(200, 23);
            this.lblPageInfo.TextAlign =
                System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPageInfo.Anchor = anchorBottomLeft;

            // 次ページボタン
            this.btnNextPage.Text = "▶";
            this.btnNextPage.Location =
                new System.Drawing.Point(758, 564);
            this.btnNextPage.Size =
                new System.Drawing.Size(40, 28);
            this.btnNextPage.Anchor = anchorBottomLeft;
            this.btnNextPage.Click +=
                new System.EventHandler(this.btnNextPage_Click);

            // 最終ページボタン
            this.btnLastPage.Text = "⏭";
            this.btnLastPage.Location =
                new System.Drawing.Point(804, 564);
            this.btnLastPage.Size =
                new System.Drawing.Size(40, 28);
            this.btnLastPage.Anchor = anchorBottomLeft;
            this.btnLastPage.Click +=
                new System.EventHandler(this.btnLastPage_Click);

            // ── ステータスバー（下・左右に伸縮） ──────────
            this.lblStatus.Location =
                new System.Drawing.Point(16, 604);
            this.lblStatus.Size =
                new System.Drawing.Size(1240, 23);
            this.lblStatus.Text = "準備完了";
            this.lblStatus.Anchor = anchorBottomLeftRight; // ✅ 下・横伸縮

            this.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.lblSearch,    this.txtSearch,
                this.btnSearch,    this.btnClear,
                this.btnDetail,    this.btnAdd,
                this.btnDataSync,
                this.btnToggleNumeric, this.btnDelete,
                this.dgvData,
                this.lblPageSize,  this.cmbPageSize,
                this.btnFirstPage, this.btnPrevPage,
                this.lblPageInfo,
                this.btnNextPage,  this.btnLastPage,
                this.lblStatus,
            });

            this.ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
    }
}