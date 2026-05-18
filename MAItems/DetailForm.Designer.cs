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
        private TextBox txtInputDate, txtRoute, txtBrokerCompany, txtTitle, txtDealId, txtBusinessContent, txtArea, txtRevenue, txtOperatingProfit, txtEBITDA, txtNetAssets, txtTotalAssets, txtNetCashDebt, txtCashEquivalents, txtInterestBearingDebt, txtEmployeeCount, txtFeatures, txtAskingPrice, txtTransferType, txtTransferReason, txtTransferConditions, txtStatus;

        private TableLayoutPanel tblProfile;
        private TextBox txtCpCompanyName, txtCpCompanyNameSub, txtCpHeadOffice, txtCpFactory, txtCpOtherOffice, txtCpFounded, txtCpFounded2, txtCpCapital, txtCpRepName, txtCpRepProfile, txtCpShareholder, txtCpBusiness, txtCpRevenue, txtCpEmployees, txtCpClients, txtCpSuppliers, txtCpCertifications, txtCpGroupCompanies, txtCpTransferReason, txtCpRemarks;

        private DataGridView dgvFinancial;

        private Panel pnlValuation;
        private TextBox txtValNetAsset, txtValNetNote, txtValEBITDA, txtValEBITDAYear, txtValMultiple, txtValEBITDANet, txtValEBITDANote, txtValDCFRate, txtValDCFGrowth, txtValDCFEV, txtValDCFNet, txtValDCFNote, txtValNOI, txtValCapRate, txtValDirectNet, txtValDirectNote, txtValNote;
        private Label lblValNetAssetResult, lblValEBITDAResult, lblValDCFResult, lblValDirectResult, lblValSummary;

        private TextBox txtAttachmentsSummary;
        private DataGridView dgvAttachments;
        private Button btnAddFile, btnOpenFile, btnDeleteFile;
        private Button btnPasteFinancial;

        // フッターボタン群
        private Button btnPasteFromMail;
        private Button btnPrev; // 追加: 前へボタン
        private Button btnNext; // 追加: 次へボタン
        private Button btnSave;
        private Button btnClose;
        private Label lblStatus;

        private void InitializeComponent()
        {
            this.tabMain = new TabControl();
            this.tabPage1 = new TabPage();
            this.tabPage2 = new TabPage();
            this.tabPage3 = new TabPage();
            this.tabPage4 = new TabPage();
            this.pnlHeader = new Panel();
            this.lblIdValue = new Label();

            this.btnPasteFromMail = new Button();
            this.btnPrev = new Button();
            this.btnNext = new Button();
            this.btnSave = new Button();
            this.btnClose = new Button();
            this.lblStatus = new Label();

            this.SuspendLayout();

            this.Text = "案件詳細";
            this.Size = new Size(860, 720);
            this.MinimumSize = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            this.pnlHeader.Location = new Point(0, 0);
            this.pnlHeader.Size = new Size(860, 36);
            this.pnlHeader.BackColor = Color.SteelBlue;
            this.pnlHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            this.lblIdValue.ForeColor = Color.White;
            this.lblIdValue.Font = new Font("Yu Gothic UI", 12F, FontStyle.Bold);
            this.lblIdValue.Location = new Point(12, 6);
            this.lblIdValue.Size = new Size(600, 24);
            this.pnlHeader.Controls.Add(this.lblIdValue);

            this.tabMain.Location = new Point(0, 36);
            this.tabMain.Size = new Size(844, 610);
            this.tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.tabPage1.Text = "基本情報";
            this.tabPage2.Text = "会社基礎情報";
            this.tabPage3.Text = "財務ハイライト";
            this.tabPage4.Text = "株式価値試算";

            this.tabMain.Controls.Add(this.tabPage1);
            this.tabMain.Controls.Add(this.tabPage2);
            this.tabMain.Controls.Add(this.tabPage3);
            this.tabMain.Controls.Add(this.tabPage4);

            // フッターボタンの配置
            this.btnPasteFromMail.Text = "📧 メールから取込";
            this.btnPasteFromMail.Location = new Point(12, 652);
            this.btnPasteFromMail.Size = new Size(130, 30);
            this.btnPasteFromMail.BackColor = Color.LightSteelBlue;
            this.btnPasteFromMail.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnPasteFromMail.Click += new EventHandler(this.btnPasteFromMail_Click);

            this.btnPrev.Text = "◀ 前へ";
            this.btnPrev.Location = new Point(356, 652);
            this.btnPrev.Size = new Size(80, 30);
            this.btnPrev.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnPrev.Click += new EventHandler(this.btnPrev_Click);

            this.btnNext.Text = "次へ ▶";
            this.btnNext.Location = new Point(442, 652);
            this.btnNext.Size = new Size(80, 30);
            this.btnNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnNext.Click += new EventHandler(this.btnNext_Click);

            this.btnSave.Text = "💾 保存して閉じる";
            this.btnSave.Location = new Point(528, 652);
            this.btnSave.Size = new Size(160, 30);
            this.btnSave.BackColor = Color.LightGreen;
            this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            this.btnClose.Text = "✖ キャンセル";
            this.btnClose.Location = new Point(694, 652);
            this.btnClose.Size = new Size(130, 30);
            this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            this.lblStatus.Location = new Point(12, 688);
            this.lblStatus.Size = new Size(816, 23);
            this.lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.Controls.AddRange(new Control[] { this.pnlHeader, this.tabMain, this.btnPasteFromMail, this.btnPrev, this.btnNext, this.btnSave, this.btnClose, this.lblStatus });

            BuildTab1();
            BuildTab2();
            BuildTab3();
            BuildTab4();
            BuildTab5();

            this.ResumeLayout(false);
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

                SetTextField(rowDefs[i, 1], txt);
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
            var lblNote = new Label { Text = "単位：千円　　期ラベルはヘッダーをダブルクリックして編集できます", Location = new Point(4, 4), Size = new Size(400, 20), Font = new Font("Yu Gothic UI", 8.5F), ForeColor = Color.DimGray };

            // 👇 ここから追加：貼付ボタンの生成
            this.btnPasteFinancial = new Button
            {
                Text = "📋 表から自動入力",
                Location = new Point(420, 2), // ラベルの右側に配置
                Size = new Size(140, 24),
                BackColor = Color.LightYellow
            };
            this.btnPasteFinancial.Click += new EventHandler(this.btnPasteFinancial_Click);
            // 👆 ここまで追加

            this.dgvFinancial.Location = new Point(4, 28);
            this.dgvFinancial.Size = new Size(820, 545);
            this.dgvFinancial.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.tabPage3.Controls.Add(lblNote);
            this.tabPage3.Controls.Add(this.btnPasteFinancial); // 👈 タブにボタンを追加
            this.tabPage3.Controls.Add(this.dgvFinancial);
        }

        private void BuildTab4()
        {
            var scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            this.pnlValuation = new Panel { Size = new Size(820, 900) };

            int y = 8; int lw = 140, tw = 200, rw = 300;

            y = AddValuationSection(this.pnlValuation, "① 純資産法", y);
            this.txtValNetAsset = AddValRow(pnlValuation, "修正純資産（千円）", ref y, lw, tw);
            this.txtValNetNote = AddValRow(pnlValuation, "備考", ref y, lw, tw, multiLine: true, height: 40);
            this.lblValNetAssetResult = AddResultLabel(pnlValuation, ref y, rw);

            y += 8; y = AddValuationSection(this.pnlValuation, "② EBITDAマルチプル", y);
            this.txtValEBITDA = AddValRow(pnlValuation, "EBITDA（千円）", ref y, lw, tw);
            this.txtValEBITDAYear = AddValRow(pnlValuation, "基準年度", ref y, lw, tw);
            this.txtValMultiple = AddValRow(pnlValuation, "マルチプル（倍）", ref y, lw, tw);
            this.txtValEBITDANet = AddValRow(pnlValuation, "ネットキャッシュ（千円）", ref y, lw, tw);
            this.txtValEBITDANote = AddValRow(pnlValuation, "備考", ref y, lw, tw, multiLine: true, height: 40);
            this.lblValEBITDAResult = AddResultLabel(pnlValuation, ref y, rw);
            this.txtValEBITDA.TextChanged += new EventHandler(this.ValuationInput_Changed);
            this.txtValMultiple.TextChanged += new EventHandler(this.ValuationInput_Changed);
            this.txtValEBITDANet.TextChanged += new EventHandler(this.ValuationInput_Changed);

            y += 8; y = AddValuationSection(this.pnlValuation, "③ DCF法", y);
            this.txtValDCFRate = AddValRow(pnlValuation, "割引率（%）", ref y, lw, tw);
            this.txtValDCFGrowth = AddValRow(pnlValuation, "永続成長率（%）", ref y, lw, tw);
            this.txtValDCFEV = AddValRow(pnlValuation, "EV（千円）", ref y, lw, tw);
            this.txtValDCFNet = AddValRow(pnlValuation, "ネットキャッシュ（千円）", ref y, lw, tw);
            this.txtValDCFNote = AddValRow(pnlValuation, "備考", ref y, lw, tw, multiLine: true, height: 40);
            this.lblValDCFResult = AddResultLabel(pnlValuation, ref y, rw);
            this.txtValDCFEV.TextChanged += new EventHandler(this.ValuationInput_Changed);
            this.txtValDCFNet.TextChanged += new EventHandler(this.ValuationInput_Changed);

            y += 8; y = AddValuationSection(this.pnlValuation, "④ 直接還元法", y);
            this.txtValNOI = AddValRow(pnlValuation, "NOI（千円）", ref y, lw, tw);
            this.txtValCapRate = AddValRow(pnlValuation, "キャップレート（%）", ref y, lw, tw);
            this.txtValDirectNet = AddValRow(pnlValuation, "ネットキャッシュ（千円）", ref y, lw, tw);
            this.txtValDirectNote = AddValRow(pnlValuation, "備考", ref y, lw, tw, multiLine: true, height: 40);
            this.lblValDirectResult = AddResultLabel(pnlValuation, ref y, rw);
            this.txtValNOI.TextChanged += new EventHandler(this.ValuationInput_Changed);
            this.txtValCapRate.TextChanged += new EventHandler(this.ValuationInput_Changed);
            this.txtValDirectNet.TextChanged += new EventHandler(this.ValuationInput_Changed);

            y += 16;
            this.lblValSummary = new Label { Location = new Point(8, y), Size = new Size(790, 28), Font = new Font("Yu Gothic UI", 11F, FontStyle.Bold), ForeColor = Color.DarkBlue, Text = "試算結果" };
            y += 36;
            this.pnlValuation.Controls.Add(new Label { Text = "総合備考:", Location = new Point(8, y), Size = new Size(lw, 23), TextAlign = ContentAlignment.MiddleRight });
            this.txtValNote = new TextBox { Location = new Point(lw + 12, y), Size = new Size(640, 60), Multiline = true, ScrollBars = ScrollBars.Vertical };
            this.pnlValuation.Controls.Add(this.txtValNote);

            scrollPanel.Controls.Add(this.pnlValuation);
            this.tabPage4.Controls.Add(scrollPanel);
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

        private static int AddValuationSection(Panel panel, string title, int y)
        {
            var lbl = new Label { Text = title, Location = new Point(4, y), Size = new Size(800, 24), Font = new Font("Yu Gothic UI", 10F, FontStyle.Bold), BackColor = Color.SteelBlue, ForeColor = Color.White };
            panel.Controls.Add(lbl);
            return y + 28;
        }

        private static TextBox AddValRow(Panel panel, string label, ref int y, int lw, int tw, bool multiLine = false, int height = 26)
        {
            panel.Controls.Add(new Label { Text = label, Location = new Point(8, y + 2), Size = new Size(lw, height), TextAlign = ContentAlignment.MiddleRight });
            var txt = new TextBox { Location = new Point(lw + 12, y), Size = new Size(tw, height), Multiline = multiLine, ScrollBars = multiLine ? ScrollBars.Vertical : ScrollBars.None };
            panel.Controls.Add(txt);
            y += height + 4;
            return txt;
        }

        private static Label AddResultLabel(Panel panel, ref int y, int width)
        {
            var lbl = new Label { Location = new Point(8, y), Size = new Size(width, 24), ForeColor = Color.DarkGreen, Font = new Font("Yu Gothic UI", 9.5F, FontStyle.Bold) };
            panel.Controls.Add(lbl);
            y += 28;
            return lbl;
        }

        private void SetTextField(string fieldName, TextBox txt)
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