using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAItems
{
    partial class DetailForm
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabMain;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private TabPage tabPage5;

        private Panel pnlHeader;
        private Label lblIdValue;
        private TableLayoutPanel tblLayout;
        private TextBox txtInputDate,  txtBrokerCompany, txtTitle, txtDealId, txtBusinessContent, txtArea, txtRevenue, txtOperatingProfit, txtEBITDA, txtNetAssets, txtTotalAssets, txtNetCashDebt, txtCashEquivalents, txtInterestBearingDebt, txtEmployeeCount, txtFeatures, txtAskingPrice, txtTransferType, txtTransferReason, txtTransferConditions;
        private ComboBox cmbRoute,cmbStatus;



        private TableLayoutPanel tblProfile;
        private TextBox txtCpCompanyName, txtCpCompanyNameSub, txtCpHeadOffice, txtCpFactory, txtCpOtherOffice, txtCpFounded, txtCpFounded2, txtCpCapital, txtCpRepName, txtCpRepProfile, txtCpShareholder, txtCpBusiness, txtCpRevenue, txtCpEmployees, txtCpClients, txtCpSuppliers, txtCpCertifications, txtCpGroupCompanies, txtCpTransferReason, txtCpRemarks;

        private DataGridView dgvFinancial;

        private TextBox txtAttachmentsSummary;
        private DataGridView dgvAttachments;
        private Button btnAddFile, btnOpenFile, btnDeleteFile;
        private Button btnPasteFinancial;

        // ▼ 今回追加：株式価値試算の数式・ロジック可視化用コントロール
        private Label lblLogicTitle;
        private RichTextBox rtbFormulaFlow;

        // フッターボタン群
        private Button btnPasteFromMail;
        private Button btnPrev;
        private Button btnNext;
        private Button btnSave;
        private Button btnClose;
        private Label lblStatus;

        private void InitializeComponent()
        {
            tabMain = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            tabPage4 = new TabPage();
            pnlHeader = new Panel();
            lblIdValue = new Label();
            btnPasteFromMail = new Button();
            btnPrev = new Button();
            btnNext = new Button();
            btnSave = new Button();
            btnClose = new Button();
            lblStatus = new Label();
            tabMain.SuspendLayout();
            pnlHeader.SuspendLayout();
            SuspendLayout();
            // 
            // tabMain
            // 
            tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabMain.Controls.Add(tabPage1);
            tabMain.Controls.Add(tabPage2);
            tabMain.Controls.Add(tabPage3);
            tabMain.Controls.Add(tabPage4);
            tabMain.Location = new Point(0, 36);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(980, 630);
            tabMain.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(972, 602);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "基本情報";
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(972, 602);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "会社基礎情報";
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(972, 602);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "財務ハイライト";
            // 
            // tabPage4
            // 
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(972, 602);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "株式価値試算";
            // 
            // pnlHeader
            // 
            pnlHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlHeader.BackColor = Color.SteelBlue;
            pnlHeader.Controls.Add(lblIdValue);
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(980, 36);
            pnlHeader.TabIndex = 0;
            // 
            // lblIdValue
            // 
            lblIdValue.Font = new Font("Yu Gothic UI", 12F, FontStyle.Bold);
            lblIdValue.ForeColor = Color.White;
            lblIdValue.Location = new Point(12, 6);
            lblIdValue.Name = "lblIdValue";
            lblIdValue.Size = new Size(600, 24);
            lblIdValue.TabIndex = 0;
            // 
            // btnPasteFromMail
            // 
            btnPasteFromMail.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnPasteFromMail.BackColor = Color.LightSteelBlue;
            btnPasteFromMail.Location = new Point(12, 669);
            btnPasteFromMail.Name = "btnPasteFromMail";
            btnPasteFromMail.Size = new Size(130, 30);
            btnPasteFromMail.TabIndex = 2;
            btnPasteFromMail.Text = "📧 メールから取込";
            btnPasteFromMail.UseVisualStyleBackColor = false;
            btnPasteFromMail.Click += btnPasteFromMail_Click;
            // 
            // btnPrev
            // 
            btnPrev.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnPrev.Location = new Point(490, 669);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(80, 30);
            btnPrev.TabIndex = 3;
            btnPrev.Text = "◀ 前へ";
            btnPrev.Click += btnPrev_Click;
            // 
            // btnNext
            // 
            btnNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnNext.Location = new Point(576, 669);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(80, 30);
            btnNext.TabIndex = 4;
            btnNext.Text = "次へ ▶";
            btnNext.Click += btnNext_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.BackColor = Color.LightGreen;
            btnSave.Location = new Point(662, 669);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(160, 30);
            btnSave.TabIndex = 5;
            btnSave.Text = "💾 保存して閉じる";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Location = new Point(828, 669);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(130, 30);
            btnClose.TabIndex = 6;
            btnClose.Text = "✖ キャンセル";
            btnClose.Click += btnClose_Click;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.Location = new Point(12, 708);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(950, 23);
            lblStatus.TabIndex = 7;
            // 
            // DetailForm
            // 
            ClientSize = new Size(980, 701);
            Controls.Add(pnlHeader);
            Controls.Add(tabMain);
            Controls.Add(btnPasteFromMail);
            Controls.Add(btnPrev);
            Controls.Add(btnNext);
            Controls.Add(btnSave);
            Controls.Add(btnClose);
            Controls.Add(lblStatus);
            MinimumSize = new Size(900, 600);
            Name = "DetailForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "案件詳細";
            tabMain.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void BuildTab1()
        {
            var scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            this.tblLayout = new TableLayoutPanel();
            this.tblLayout.Dock = DockStyle.Top;
            this.tblLayout.AutoSize = true;
            this.tblLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tblLayout.Padding = new Padding(4, 4, 24, 4);
            this.tblLayout.ColumnCount = 2;
            this.tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            this.tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            string[,] rowDefs = new string[,] {
                { "入力日", "InputDate", "false" }, { "経路", "Route", "false" }, { "仲介会社", "BrokerCompany", "false" }, { "タイトル", "Title", "false" }, { "案件ID", "DealId", "false" },
                { "事業内容", "BusinessContent", "true" }, { "エリア", "Area", "false" }, { "売上高", "Revenue", "false" }, { "営業利益", "OperatingProfit", "false" }, { "EBITDA", "EBITDA", "false" },
                { "純資産額", "NetAssets", "false" }, { "総資産額", "TotalAssets", "false" }, { "Net Cash/Debt", "NetCashDebt", "false" }, { "現金等", "CashEquivalents", "false" }, { "有利子負債", "InterestBearingDebt", "false" },
                { "従業員数", "EmployeeCount", "false" }, { "特徴", "Features", "true" }, { "譲渡希望額", "AskingPrice", "false" }, { "譲渡形態", "TransferType", "false" }, { "譲渡理由", "TransferReason", "true" },
                { "希望条件", "TransferConditions", "true" }, { "処理", "Status", "false" }
            };

            int[] rowHeights = { 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28 };

            for (int i = 0; i < rowDefs.GetLength(0); i++)
            {
                bool multiLine = rowDefs[i, 2] == "true";
                if (multiLine) this.tblLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                else this.tblLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeights[i]));

                var lbl = new Label { Text = rowDefs[i, 0], Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Yu Gothic UI", 9F), Padding = new Padding(0, 0, 8, 0) };

                string fieldName = rowDefs[i, 1]; // "Route" などの項目名を取得

                // 💡 分岐処理：経路(Route)の場合
                if (fieldName == "Route")
                {
                    cmbRoute = new ComboBox();
                    cmbRoute.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbRoute.Dock = DockStyle.Fill;
                    cmbRoute.Font = new Font("Yu Gothic UI", 9F);
                    cmbRoute.Margin = new Padding(0, 2, 4, 2);
                    cmbRoute.Items.Clear();
                    cmbRoute.Items.AddRange(new string[] { "メール", "直接" });

                    this.tblLayout.Controls.Add(lbl, 0, i);
                    this.tblLayout.Controls.Add(cmbRoute, 1, i);
                    continue; // テキストボックスを作る処理をスキップして次の項目へ
                }

                // 💡 分岐処理：ステータス(Status)の場合
                if (fieldName == "Status")
                {
                    cmbStatus = new ComboBox();
                    cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbStatus.Dock = DockStyle.Fill;
                    cmbStatus.Font = new Font("Yu Gothic UI", 9F);
                    cmbStatus.Margin = new Padding(0, 2, 4, 2);
                    cmbStatus.Items.Clear();
                    cmbStatus.Items.AddRange(new string[] {
                        "00_情報受領","01_初期検討 (ノンネーム)", "02_ネームクリア・NDA締結", "03_IM受領・詳細検討",
                        "04_トップ面談", "05_意向表明(LOI)提出", "06_基本合意(MOU)締結",
                        "07_買収監査(DD)実施", "08_最終譲渡契約(DA)締結", "09_クロージング完了",
                        "98_保留・ペンディング", "99_見送り・断念"
                    });

                    this.tblLayout.Controls.Add(lbl, 0, i);
                    this.tblLayout.Controls.Add(cmbStatus, 1, i);
                    continue; // テキストボックスを作る処理をスキップして次の項目へ
                }

                // ── ここから下は通常のテキストボックス作成処理（元のコードと同じ） ──
                var txt = new TextBox { Dock = DockStyle.Fill, Multiline = multiLine, ScrollBars = ScrollBars.None, Font = new Font("Yu Gothic UI", 9F), Margin = new Padding(0, 2, 4, 2) };

                if (multiLine)
                {
                    int baseHeight = rowHeights[i];
                    txt.MinimumSize = new Size(0, baseHeight);
                    Action adjustHeight = () => {
                        if (txt.ClientSize.Width < 50) return;
                        var size = TextRenderer.MeasureText(txt.Text + "\n", txt.Font, new Size(txt.ClientSize.Width - 10, int.MaxValue), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
                        int newHeight = Math.Max(baseHeight, size.Height + 12);
                        if (txt.MinimumSize.Height != newHeight) txt.MinimumSize = new Size(0, newHeight);
                    };
                    txt.TextChanged += (s, e) => adjustHeight();
                    txt.SizeChanged += (s, e) => adjustHeight();
                }

                SetTextField(fieldName, txt);
                this.tblLayout.Controls.Add(lbl, 0, i);
                this.tblLayout.Controls.Add(txt, 1, i);
            }

            scrollPanel.Controls.Add(this.tblLayout);
            this.tabPage1.Controls.Add(scrollPanel);
        }

        private void BuildTab2()
        {
            var scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            this.tblProfile = new TableLayoutPanel();
            this.tblProfile.Dock = DockStyle.Top;
            this.tblProfile.AutoSize = true;
            this.tblProfile.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tblProfile.Padding = new Padding(4, 4, 24, 4);
            this.tblProfile.ColumnCount = 2;
            this.tblProfile.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            this.tblProfile.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            string[,] profileDefs = new string[,] {
                { "会社名", "CompanyName", "false" }, { "別会社名", "CompanyNameSub", "false" }, { "本社住所", "HeadOfficeAddress", "false" }, { "工場住所", "FactoryAddress", "false" }, { "その他事務所", "OtherOffice", "false" },
                { "設立", "Founded", "false" }, { "関連会社設立", "Founded2", "false" }, { "資本金", "Capital", "false" }, { "代表者名", "RepresentativeName", "false" }, { "代表者略歴", "RepresentativeProfile", "true" },
                { "株主構成", "ShareholderInfo", "true" }, { "事業内容詳細", "BusinessDetail", "true" }, { "売上高", "Revenue", "false" }, { "従業員数", "Employees", "false" }, { "主要取引先", "MainClients", "true" },
                { "主要仕入先", "MainSuppliers", "true" }, { "認証・許認可", "Certifications", "false" }, { "グループ会社", "GroupCompanies", "false" }, { "譲渡理由", "TransferReason", "true" }, { "備考", "Remarks", "true" }
            };

            int[] profileHeights = { 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28 };

            for (int i = 0; i < profileDefs.GetLength(0); i++)
            {
                bool multiLine = profileDefs[i, 2] == "true";
                if (multiLine) this.tblProfile.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                else this.tblProfile.RowStyles.Add(new RowStyle(SizeType.Absolute, profileHeights[i]));

                var lbl = new Label { Text = profileDefs[i, 0], Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Yu Gothic UI", 9F), Padding = new Padding(0, 0, 8, 0) };
                var txt = new TextBox { Dock = DockStyle.Fill, Multiline = multiLine, ScrollBars = ScrollBars.None, Font = new Font("Yu Gothic UI", 9F), Margin = new Padding(0, 2, 4, 2) };

                if (multiLine)
                {
                    int baseHeight = profileHeights[i];
                    txt.MinimumSize = new Size(0, baseHeight);
                    Action adjustHeight = () => {
                        if (txt.ClientSize.Width < 50) return;
                        var size = TextRenderer.MeasureText(txt.Text + "\n", txt.Font, new Size(txt.ClientSize.Width - 10, int.MaxValue), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
                        int newHeight = Math.Max(baseHeight, size.Height + 12);
                        if (txt.MinimumSize.Height != newHeight) txt.MinimumSize = new Size(0, newHeight);
                    };
                    txt.TextChanged += (s, e) => adjustHeight();
                    txt.SizeChanged += (s, e) => adjustHeight();
                }

                SetProfileField(profileDefs[i, 1], txt);
                this.tblProfile.Controls.Add(lbl, 0, i);
                this.tblProfile.Controls.Add(txt, 1, i);
            }

            scrollPanel.Controls.Add(this.tblProfile);
            this.tabPage2.Controls.Add(scrollPanel);
        }

        private void BuildTab3()
        {
            this.dgvFinancial = new DataGridView();
            var lblNote = new Label { Text = "単位：百万円  期ラベルはヘッダーをダブルクリックして編集できます", Location = new Point(4, 4), Size = new Size(400, 20), Font = new Font("Yu Gothic UI", 8.5F), ForeColor = Color.DimGray };

            this.btnPasteFinancial = new Button
            {
                Text = "📋 表から自動入力",
                Location = new Point(420, 2),
                Size = new Size(140, 24),
                BackColor = Color.LightYellow
            };
            this.btnPasteFinancial.Click += new EventHandler(this.btnPasteFinancial_Click);

            this.dgvFinancial.Location = new Point(4, 28);
            this.dgvFinancial.Size = new Size(960, 545);
            this.dgvFinancial.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.dgvFinancial.ColumnHeaderMouseDoubleClick += DgvFinancial_ColumnHeaderMouseDoubleClick;
            SetupFinancialGridContextMenu();

            this.tabPage3.Controls.Add(lblNote);
            this.tabPage3.Controls.Add(this.btnPasteFinancial);
            this.tabPage3.Controls.Add(this.dgvFinancial);
        }

        // ══════════════════════════════════════════════════════
        // ★修正: タブ4（バリュエーション画面）のUI構築
        // ══════════════════════════════════════════════════════
        private void BuildTab4()
        {
            // ── 新規追加: 右側に「数式マップ」を表示するためのパネルを配置 ──
            Panel pnlFormula = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340, // 右側に340pxの専用幅を確保
                Padding = new Padding(8),
                BackColor = Color.FromArgb(245, 248, 250) // 見やすい薄いブルーグレーの背景
            };

            lblLogicTitle = new Label
            {
                Text = "💡 計算フロー ＆ ロジック",
                Dock = DockStyle.Top,
                Font = new Font("Yu Gothic UI", 10F, FontStyle.Bold),
                Height = 30,
                TextAlign = ContentAlignment.BottomLeft,
                ForeColor = Color.DarkSlateGray,
                Padding = new Padding(0, 0, 0, 4)
            };

            rtbFormulaFlow = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Yu Gothic UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "左側の数値を入力すると、ここに計算式と結果がリアルタイムに表示されます。"
            };

            pnlFormula.Controls.Add(rtbFormulaFlow);
            pnlFormula.Controls.Add(lblLogicTitle);

            // 先に右パネルをTabPageに追加（これにより、後続のUIが左側の残りスペースを正しく埋めます）
            this.tabPage4.Controls.Add(pnlFormula);

            // 既存のバリュエーション入力UIの構築（左側に展開されます）
            BuildValuationUI(this.tabPage4);
        }

        private void BuildTab5()
        {
            this.tabPage5 = new TabPage("添付資料");
            this.tabMain.TabPages.Add(this.tabPage5);

            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            mainLayout.Controls.Add(new Label { Text = "案件資料の全体概況:", TextAlign = ContentAlignment.BottomLeft, Dock = DockStyle.Fill }, 0, 0);
            this.txtAttachmentsSummary = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                MinimumSize = new Size(0, 60),
                Font = new Font("Yu Gothic UI", 9F),
                ScrollBars = ScrollBars.None
            };

            Action adjustSummaryHeight = () => {
                if (this.txtAttachmentsSummary.ClientSize.Width < 50) return;
                var size = TextRenderer.MeasureText(this.txtAttachmentsSummary.Text + "\n", this.txtAttachmentsSummary.Font, new Size(this.txtAttachmentsSummary.ClientSize.Width - 10, int.MaxValue), TextFormatFlags.WordBreak);
                int newHeight = Math.Max(60, size.Height + 12);
                if (this.txtAttachmentsSummary.MinimumSize.Height != newHeight) this.txtAttachmentsSummary.MinimumSize = new Size(0, newHeight);
            };
            this.txtAttachmentsSummary.TextChanged += (s, e) => adjustSummaryHeight();
            this.txtAttachmentsSummary.SizeChanged += (s, e) => adjustSummaryHeight();
            mainLayout.Controls.Add(this.txtAttachmentsSummary, 0, 1);

            mainLayout.Controls.Add(new Label { Text = "保管ファイル一覧 (ダブルクリックで開く):", TextAlign = ContentAlignment.BottomLeft, Dock = DockStyle.Fill }, 0, 2);
            this.dgvAttachments = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };
            this.dgvAttachments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FileName", HeaderText = "ファイル名", Width = 250, ReadOnly = true });
            this.dgvAttachments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Description", HeaderText = "ファイル内容・備考", Width = 450 });
            this.dgvAttachments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UploadedAt", HeaderText = "追加日時", Width = 150, ReadOnly = true });
            mainLayout.Controls.Add(this.dgvAttachments, 0, 3);

            var pnlButtons = new FlowLayoutPanel { Dock = DockStyle.Fill };
            this.btnAddFile = new Button { Text = "➕ ファイル追加", Width = 120, Height = 30, BackColor = Color.LightCyan };
            this.btnOpenFile = new Button { Text = "📂 開く", Width = 100, Height = 30 };
            this.btnDeleteFile = new Button { Text = "🗑 削除", Width = 100, Height = 30, BackColor = Color.MistyRose };
            pnlButtons.Controls.AddRange(new Control[] { btnAddFile, btnOpenFile, btnDeleteFile });
            mainLayout.Controls.Add(pnlButtons, 0, 4);

            this.tabPage5.Controls.Add(mainLayout);
        }

        private void SetTextField(string fieldName, TextBox txt)
        {
            switch (fieldName)
            {
                case "InputDate": txtInputDate = txt; break;
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
            }
        }

        private void SetProfileField(string fieldName, TextBox txt)
        {
            switch (fieldName)
            {
                case "CompanyName": txtCpCompanyName = txt; break;
                case "CompanyNameSub": txtCpCompanyNameSub = txt; break;
                case "HeadOfficeAddress": txtCpHeadOffice = txt; break;
                case "FactoryAddress": txtCpFactory = txt; break;
                case "OtherOffice": txtCpOtherOffice = txt; break;
                case "Founded": txtCpFounded = txt; break;
                case "Founded2": txtCpFounded2 = txt; break;
                case "Capital": txtCpCapital = txt; break;
                case "RepresentativeName": txtCpRepName = txt; break;
                case "RepresentativeProfile": txtCpRepProfile = txt; break;
                case "ShareholderInfo": txtCpShareholder = txt; break;
                case "BusinessDetail": txtCpBusiness = txt; break;
                case "Revenue": txtCpRevenue = txt; break;
                case "Employees": txtCpEmployees = txt; break;
                case "MainClients": txtCpClients = txt; break;
                case "MainSuppliers": txtCpSuppliers = txt; break;
                case "Certifications": txtCpCertifications = txt; break;
                case "GroupCompanies": txtCpGroupCompanies = txt; break;
                case "TransferReason": txtCpTransferReason = txt; break;
                case "Remarks": txtCpRemarks = txt; break;
            }
        }

        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }
    }
}