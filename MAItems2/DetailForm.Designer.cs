namespace MAItems
{
    partial class DetailForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblIdValue;
        private System.Windows.Forms.TableLayoutPanel tblLayout;

        private System.Windows.Forms.TextBox txtInputDate;
        private System.Windows.Forms.TextBox txtRoute;
        private System.Windows.Forms.TextBox txtBrokerCompany;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtDealId;
        private System.Windows.Forms.TextBox txtBusinessContent;
        private System.Windows.Forms.TextBox txtArea;
        private System.Windows.Forms.TextBox txtRevenue;
        private System.Windows.Forms.TextBox txtOperatingProfit;
        private System.Windows.Forms.TextBox txtEBITDA;
        private System.Windows.Forms.TextBox txtNetAssets;
        private System.Windows.Forms.TextBox txtTotalAssets;
        private System.Windows.Forms.TextBox txtNetCashDebt;
        private System.Windows.Forms.TextBox txtCashEquivalents;
        private System.Windows.Forms.TextBox txtInterestBearingDebt;
        private System.Windows.Forms.TextBox txtEmployeeCount;
        private System.Windows.Forms.TextBox txtFeatures;
        private System.Windows.Forms.TextBox txtAskingPrice;
        private System.Windows.Forms.TextBox txtTransferType;
        private System.Windows.Forms.TextBox txtTransferReason;
        private System.Windows.Forms.TextBox txtTransferConditions;
        private System.Windows.Forms.TextBox txtStatus;

        private System.Windows.Forms.Button btnPasteFromMail;

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblStatus;

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblIdValue = new System.Windows.Forms.Label();
            this.tblLayout = new System.Windows.Forms.TableLayoutPanel();

            this.btnPasteFromMail = new System.Windows.Forms.Button();

            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // フォーム設定
            this.Text = "案件詳細";
            this.Size = new System.Drawing.Size(700, 780);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // ヘッダーパネル
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Size = new System.Drawing.Size(700, 36);
            this.pnlHeader.BackColor = System.Drawing.Color.SteelBlue;

            this.lblIdValue.ForeColor = System.Drawing.Color.White;
            this.lblIdValue.Font = new System.Drawing.Font(
                "Yu Gothic UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblIdValue.Location = new System.Drawing.Point(12, 6);
            this.lblIdValue.Size = new System.Drawing.Size(400, 24);
            this.lblIdValue.Text = "案件詳細";
            this.pnlHeader.Controls.Add(this.lblIdValue);

            // TableLayoutPanel（行列の詳細設定は BuildTableLayout() で行う）
            this.tblLayout.Location = new System.Drawing.Point(12, 44);
            this.tblLayout.Size = new System.Drawing.Size(662, 660);
            this.tblLayout.ColumnCount = 2;
            this.tblLayout.RowCount = 22;

            // メールから取込ボタン
            this.btnPasteFromMail.Text = "📧 メールから取込";
            this.btnPasteFromMail.Location = new System.Drawing.Point(12, 712);
            this.btnPasteFromMail.Size = new System.Drawing.Size(130, 30);
            this.btnPasteFromMail.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnPasteFromMail.Click +=
                new System.EventHandler(this.btnPasteFromMail_Click);


            // 保存ボタン
            this.btnSave.Text = "💾 保存";
            this.btnSave.Location = new System.Drawing.Point(480, 712);
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.BackColor = System.Drawing.Color.LightGreen;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // 閉じるボタン
            this.btnClose.Text = "✖ 閉じる";
            this.btnClose.Location = new System.Drawing.Point(582, 712);
            this.btnClose.Size = new System.Drawing.Size(90, 30);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // ステータスラベル
            this.lblStatus.Location = new System.Drawing.Point(12, 716);
            this.lblStatus.Size = new System.Drawing.Size(460, 23);
            this.lblStatus.Text = string.Empty;

            this.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.pnlHeader,
                this.tblLayout,
                this.btnPasteFromMail,
                this.btnSave,
                this.btnClose,
                this.lblStatus,
            });

            this.ResumeLayout(false);

            // ✅ 配列・ループ処理はデザイナー非対象の別メソッドへ
            BuildTableLayout();
        }

        /// <summary>
        /// TableLayoutPanel の列・行スタイルと
        /// 各入力行のコントロール生成をここで行う。
        /// InitializeComponent() から分離することで
        /// デザイナーの CodeDOM パーサーエラーを回避。
        /// </summary>
        private void BuildTableLayout()
        {
            // 列スタイル
            this.tblLayout.ColumnStyles.Add(
                new System.Windows.Forms.ColumnStyle(
                    System.Windows.Forms.SizeType.Absolute, 140F));
            this.tblLayout.ColumnStyles.Add(
                new System.Windows.Forms.ColumnStyle(
                    System.Windows.Forms.SizeType.Percent, 100F));

            // 行の高さ定義
            int[] rowHeights = new int[]
            {
                28, 28, 28, 28, 28,
                60, 28, 28, 28, 28,
                28, 28, 28, 28, 28,
                28, 80, 28, 28, 60,
                60, 28,
            };

            this.tblLayout.RowStyles.Clear();
            foreach (int h in rowHeights)
                this.tblLayout.RowStyles.Add(
                    new System.Windows.Forms.RowStyle(
                        System.Windows.Forms.SizeType.Absolute, h));

            // 行データ定義（ラベル名・フィールド名・複数行フラグ）
            string[,] rowDefs = new string[,]
            {
                { "入力日",        "InputDate",           "false" },
                { "経路",          "Route",               "false" },
                { "仲介会社",      "BrokerCompany",       "false" },
                { "タイトル",      "Title",               "false" },
                { "案件ID",        "DealId",              "false" },
                { "事業内容",      "BusinessContent",     "true"  },
                { "エリア",        "Area",                "false" },
                { "売上高",        "Revenue",             "false" },
                { "営業利益",      "OperatingProfit",     "false" },
                { "EBITDA",        "EBITDA",              "false" },
                { "純資産額",      "NetAssets",           "false" },
                { "総資産額",      "TotalAssets",         "false" },
                { "Net Cash/Debt", "NetCashDebt",         "false" },
                { "現金等",        "CashEquivalents",     "false" },
                { "有利子負債",    "InterestBearingDebt", "false" },
                { "従業員数",      "EmployeeCount",       "false" },
                { "特徴",          "Features",            "true"  },
                { "譲渡希望額",    "AskingPrice",         "false" },
                { "譲渡形態",      "TransferType",        "false" },
                { "譲渡理由",      "TransferReason",      "true"  },
                { "希望条件",      "TransferConditions",  "true"  },
                { "処理",          "Status",              "false" },
            };

            for (int i = 0; i < rowDefs.GetLength(0); i++)
            {
                string labelText = rowDefs[i, 0];
                string fieldName = rowDefs[i, 1];
                bool multiLine = rowDefs[i, 2] == "true";

                var lbl = new System.Windows.Forms.Label
                {
                    Text = labelText,
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                    Font = new System.Drawing.Font("Yu Gothic UI", 9F),
                    Padding = new System.Windows.Forms.Padding(0, 0, 8, 0),
                };

                var txt = new System.Windows.Forms.TextBox
                {
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    Multiline = multiLine,
                    ScrollBars = multiLine
                        ? System.Windows.Forms.ScrollBars.Vertical
                        : System.Windows.Forms.ScrollBars.None,
                    Font = new System.Drawing.Font("Yu Gothic UI", 9F),
                    Margin = new System.Windows.Forms.Padding(0, 2, 4, 2),
                };

                SetTextField(fieldName, txt);

                this.tblLayout.Controls.Add(lbl, 0, i);
                this.tblLayout.Controls.Add(txt, 1, i);
            }
        }

        private void SetTextField(string fieldName,
            System.Windows.Forms.TextBox txt)
        {
            switch (fieldName)
            {
                case "InputDate": txtInputDate = txt; break;
                case "Route": txtRoute = txt; break;
                case "BrokerCompany": txtBrokerCompany = txt; break;
                case "Title": txtTitle = txt; break;
                case "DealId": txtDealId = txt; break;
                case "BusinessContent": txtBusinessContent = txt; break;
                case "Area": txtArea = txt; break;
                case "Revenue": txtRevenue = txt; break;
                case "OperatingProfit": txtOperatingProfit = txt; break;
                case "EBITDA": txtEBITDA = txt; break;
                case "NetAssets": txtNetAssets = txt; break;
                case "TotalAssets": txtTotalAssets = txt; break;
                case "NetCashDebt": txtNetCashDebt = txt; break;
                case "CashEquivalents": txtCashEquivalents = txt; break;
                case "InterestBearingDebt": txtInterestBearingDebt = txt; break;
                case "EmployeeCount": txtEmployeeCount = txt; break;
                case "Features": txtFeatures = txt; break;
                case "AskingPrice": txtAskingPrice = txt; break;
                case "TransferType": txtTransferType = txt; break;
                case "TransferReason": txtTransferReason = txt; break;
                case "TransferConditions": txtTransferConditions = txt; break;
                case "Status": txtStatus = txt; break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
    }
}