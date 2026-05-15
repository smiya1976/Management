namespace MAItems
{
    partial class DetailForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── タブコントロール ──────────────────────────────
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;

        // ── Tab1：基本情報 ────────────────────────────────
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

        // ── Tab2：会社基礎情報 ────────────────────────────
        private System.Windows.Forms.TableLayoutPanel tblProfile;
        private System.Windows.Forms.TextBox txtCpCompanyName;
        private System.Windows.Forms.TextBox txtCpCompanyNameSub;
        private System.Windows.Forms.TextBox txtCpHeadOffice;
        private System.Windows.Forms.TextBox txtCpFactory;
        private System.Windows.Forms.TextBox txtCpOtherOffice;
        private System.Windows.Forms.TextBox txtCpFounded;
        private System.Windows.Forms.TextBox txtCpFounded2;
        private System.Windows.Forms.TextBox txtCpCapital;
        private System.Windows.Forms.TextBox txtCpRepName;
        private System.Windows.Forms.TextBox txtCpRepProfile;
        private System.Windows.Forms.TextBox txtCpShareholder;
        private System.Windows.Forms.TextBox txtCpBusiness;
        private System.Windows.Forms.TextBox txtCpRevenue;
        private System.Windows.Forms.TextBox txtCpEmployees;
        private System.Windows.Forms.TextBox txtCpClients;
        private System.Windows.Forms.TextBox txtCpSuppliers;
        private System.Windows.Forms.TextBox txtCpCertifications;
        private System.Windows.Forms.TextBox txtCpGroupCompanies;
        private System.Windows.Forms.TextBox txtCpTransferReason;
        private System.Windows.Forms.TextBox txtCpRemarks;

        // ── Tab3：財務ハイライト ──────────────────────────
        private System.Windows.Forms.DataGridView dgvFinancial;

        // ── Tab4：株式価値試算 ────────────────────────────
        private System.Windows.Forms.Panel pnlValuation;

        // 純資産法
        private System.Windows.Forms.TextBox txtValNetAsset;
        private System.Windows.Forms.TextBox txtValNetNote;
        private System.Windows.Forms.Label lblValNetAssetResult;

        // EBITDAマルチプル
        private System.Windows.Forms.TextBox txtValEBITDA;
        private System.Windows.Forms.TextBox txtValEBITDAYear;
        private System.Windows.Forms.TextBox txtValMultiple;
        private System.Windows.Forms.TextBox txtValEBITDANet;
        private System.Windows.Forms.TextBox txtValEBITDANote;
        private System.Windows.Forms.Label lblValEBITDAResult;

        // DCF法
        private System.Windows.Forms.TextBox txtValDCFRate;
        private System.Windows.Forms.TextBox txtValDCFGrowth;
        private System.Windows.Forms.TextBox txtValDCFEV;
        private System.Windows.Forms.TextBox txtValDCFNet;
        private System.Windows.Forms.TextBox txtValDCFNote;
        private System.Windows.Forms.Label lblValDCFResult;

        // 直接還元法
        private System.Windows.Forms.TextBox txtValNOI;
        private System.Windows.Forms.TextBox txtValCapRate;
        private System.Windows.Forms.TextBox txtValDirectNet;
        private System.Windows.Forms.TextBox txtValDirectNote;
        private System.Windows.Forms.Label lblValDirectResult;

        // サマリー・備考
        private System.Windows.Forms.Label lblValSummary;
        private System.Windows.Forms.TextBox txtValNote;

        // ── 共通ボタン・ステータス ────────────────────────
        private System.Windows.Forms.Button btnPasteFromMail;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblStatus;

        private void InitializeComponent()
        {
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblIdValue = new System.Windows.Forms.Label();
            this.tblLayout = new System.Windows.Forms.TableLayoutPanel();
            this.tblProfile = new System.Windows.Forms.TableLayoutPanel();
            this.dgvFinancial = new System.Windows.Forms.DataGridView();
            this.pnlValuation = new System.Windows.Forms.Panel();

            this.btnPasteFromMail = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // ── フォーム ──────────────────────────────────
            this.Text = "案件詳細";
            this.Size = new System.Drawing.Size(860, 720);
            this.MinimumSize = new System.Drawing.Size(700, 600);
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle =
                System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;

            // ── ヘッダーパネル ────────────────────────────
            this.pnlHeader.Location =
                new System.Drawing.Point(0, 0);
            this.pnlHeader.Size =
                new System.Drawing.Size(860, 36);
            this.pnlHeader.BackColor =
                System.Drawing.Color.SteelBlue;
            this.pnlHeader.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;

            this.lblIdValue.ForeColor =
                System.Drawing.Color.White;
            this.lblIdValue.Font =
                new System.Drawing.Font(
                    "Yu Gothic UI", 12F,
                    System.Drawing.FontStyle.Bold);
            this.lblIdValue.Location =
                new System.Drawing.Point(12, 6);
            this.lblIdValue.Size =
                new System.Drawing.Size(600, 24);
            this.lblIdValue.Text = "案件詳細";
            this.pnlHeader.Controls.Add(this.lblIdValue);

            // ── TabControl ────────────────────────────────
            this.tabMain.Location =
                new System.Drawing.Point(0, 36);
            this.tabMain.Size =
                new System.Drawing.Size(844, 610);
            this.tabMain.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;

            this.tabPage1.Text = "基本情報";
            this.tabPage2.Text = "会社基礎情報";
            this.tabPage3.Text = "財務ハイライト";
            this.tabPage4.Text = "株式価値試算";

            this.tabMain.Controls.Add(this.tabPage1);
            this.tabMain.Controls.Add(this.tabPage2);
            this.tabMain.Controls.Add(this.tabPage3);
            this.tabMain.Controls.Add(this.tabPage4);

            // ── 共通ボタン・ステータス（フォーム下部） ───
            this.btnPasteFromMail.Text = "📧 メールから取込";
            this.btnPasteFromMail.Location =
                new System.Drawing.Point(12, 652);
            this.btnPasteFromMail.Size =
                new System.Drawing.Size(130, 30);
            this.btnPasteFromMail.BackColor =
                System.Drawing.Color.LightSteelBlue;
            this.btnPasteFromMail.Anchor =
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left;
            this.btnPasteFromMail.Click +=
                new System.EventHandler(this.btnPasteFromMail_Click);

            this.btnSave.Text = "💾 保存";
            this.btnSave.Location =
                new System.Drawing.Point(596, 652);
            this.btnSave.Size =
                new System.Drawing.Size(110, 30);
            this.btnSave.BackColor =
                System.Drawing.Color.LightGreen;
            this.btnSave.Anchor =
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Right;
            this.btnSave.Click +=
                new System.EventHandler(this.btnSave_Click);

            this.btnClose.Text = "✖ 閉じる";
            this.btnClose.Location =
                new System.Drawing.Point(718, 652);
            this.btnClose.Size =
                new System.Drawing.Size(110, 30);
            this.btnClose.Anchor =
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.Click +=
                new System.EventHandler(this.btnClose_Click);

            this.lblStatus.Location =
                new System.Drawing.Point(12, 688);
            this.lblStatus.Size =
                new System.Drawing.Size(816, 23);
            this.lblStatus.Text = string.Empty;
            this.lblStatus.Anchor =
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;

            this.Controls.AddRange(
                new System.Windows.Forms.Control[]
            {
                this.pnlHeader,
                this.tabMain,
                this.btnPasteFromMail,
                this.btnSave,
                this.btnClose,
                this.lblStatus,
            });

            this.ResumeLayout(false);

            // ✅ 各タブの内容はデザイナー非対象メソッドで構築
            BuildTab1();
            BuildTab2();
            BuildTab3();
            BuildTab4();
        }

        // ── Tab1 構築 ─────────────────────────────────────
        private void BuildTab1()
        {
            this.tblLayout.Location =
                new System.Drawing.Point(4, 4);
            this.tblLayout.Size =
                new System.Drawing.Size(820, 570);
            this.tblLayout.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.tblLayout.ColumnCount = 2;
            this.tblLayout.RowCount = 22;
            this.tblLayout.AutoScroll = true;

            this.tblLayout.ColumnStyles.Add(
                new System.Windows.Forms.ColumnStyle(
                    System.Windows.Forms.SizeType.Absolute, 140F));
            this.tblLayout.ColumnStyles.Add(
                new System.Windows.Forms.ColumnStyle(
                    System.Windows.Forms.SizeType.Percent, 100F));

            int[] rowHeights =
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
                    TextAlign =
                        System.Drawing.ContentAlignment.MiddleRight,
                    Font =
                        new System.Drawing.Font("Yu Gothic UI", 9F),
                    Padding =
                        new System.Windows.Forms.Padding(0, 0, 8, 0),
                };

                var txt = new System.Windows.Forms.TextBox
                {
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    Multiline = multiLine,
                    ScrollBars = multiLine
                        ? System.Windows.Forms.ScrollBars.Vertical
                        : System.Windows.Forms.ScrollBars.None,
                    Font =
                        new System.Drawing.Font("Yu Gothic UI", 9F),
                    Margin =
                        new System.Windows.Forms.Padding(0, 2, 4, 2),
                };

                SetTextField(fieldName, txt);
                this.tblLayout.Controls.Add(lbl, 0, i);
                this.tblLayout.Controls.Add(txt, 1, i);
            }

            this.tabPage1.Controls.Add(this.tblLayout);
        }

        // ── Tab2 構築 ─────────────────────────────────────
        private void BuildTab2()
        {
            var scrollPanel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                AutoScroll = true,
            };

            this.tblProfile.ColumnCount = 2;
            this.tblProfile.ColumnStyles.Add(
                new System.Windows.Forms.ColumnStyle(
                    System.Windows.Forms.SizeType.Absolute, 140F));
            this.tblProfile.ColumnStyles.Add(
                new System.Windows.Forms.ColumnStyle(
                    System.Windows.Forms.SizeType.Percent, 100F));

            string[,] profileDefs = new string[,]
            {
                { "会社名",       "CompanyName",          "false" },
                { "別会社名",     "CompanyNameSub",       "false" },
                { "本社住所",     "HeadOfficeAddress",    "false" },
                { "工場住所",     "FactoryAddress",       "false" },
                { "その他事務所", "OtherOffice",          "false" },
                { "設立",         "Founded",              "false" },
                { "関連会社設立", "Founded2",             "false" },
                { "資本金",       "Capital",              "false" },
                { "代表者名",     "RepresentativeName",   "false" },
                { "代表者略歴",   "RepresentativeProfile","true"  },
                { "株主構成",     "ShareholderInfo",      "true"  },
                { "事業内容詳細", "BusinessDetail",       "true"  },
                { "売上高",       "Revenue",              "false" },
                { "従業員数",     "Employees",            "false" },
                { "主要取引先",   "MainClients",          "true"  },
                { "主要仕入先",   "MainSuppliers",        "true"  },
                { "認証・許認可", "Certifications",       "false" },
                { "グループ会社", "GroupCompanies",       "false" },
                { "譲渡理由",     "TransferReason",       "true"  },
                { "備考",         "Remarks",              "true"  },
            };

            int[] profileHeights =
            {
                28, 28, 28, 28, 28,
                28, 28, 28, 28, 60,
                60, 80, 28, 28, 60,
                60, 28, 28, 60, 60,
            };

            this.tblProfile.RowCount = profileDefs.GetLength(0);
            this.tblProfile.RowStyles.Clear();
            foreach (int h in profileHeights)
                this.tblProfile.RowStyles.Add(
                    new System.Windows.Forms.RowStyle(
                        System.Windows.Forms.SizeType.Absolute, h));

            // 合計高さで幅を設定
            int totalHeight = 0;
            foreach (int h in profileHeights) totalHeight += h;

            this.tblProfile.Size =
                new System.Drawing.Size(800, totalHeight + 10);
            this.tblProfile.AutoSize = false;

            for (int i = 0; i < profileDefs.GetLength(0); i++)
            {
                string labelText = profileDefs[i, 0];
                string fieldName = profileDefs[i, 1];
                bool multiLine = profileDefs[i, 2] == "true";

                var lbl = new System.Windows.Forms.Label
                {
                    Text = labelText,
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    TextAlign =
                        System.Drawing.ContentAlignment.MiddleRight,
                    Font =
                        new System.Drawing.Font("Yu Gothic UI", 9F),
                    Padding =
                        new System.Windows.Forms.Padding(0, 0, 8, 0),
                };

                var txt = new System.Windows.Forms.TextBox
                {
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    Multiline = multiLine,
                    ScrollBars = multiLine
                        ? System.Windows.Forms.ScrollBars.Vertical
                        : System.Windows.Forms.ScrollBars.None,
                    Font =
                        new System.Drawing.Font("Yu Gothic UI", 9F),
                    Margin =
                        new System.Windows.Forms.Padding(0, 2, 4, 2),
                };

                SetProfileField(fieldName, txt);
                this.tblProfile.Controls.Add(lbl, 0, i);
                this.tblProfile.Controls.Add(txt, 1, i);
            }

            scrollPanel.Controls.Add(this.tblProfile);
            this.tabPage2.Controls.Add(scrollPanel);
        }

        // ── Tab3 構築 ─────────────────────────────────────
        private void BuildTab3()
        {
            // ヘッダー説明ラベル
            var lblNote = new System.Windows.Forms.Label
            {
                Text = "単位：千円　　期ラベルはヘッダーをダブルクリックして編集できます",
                Location = new System.Drawing.Point(4, 4),
                Size = new System.Drawing.Size(800, 20),
                Font = new System.Drawing.Font("Yu Gothic UI", 8.5F),
                ForeColor = System.Drawing.Color.DimGray,
            };

            this.dgvFinancial.Location =
                new System.Drawing.Point(4, 28);
            this.dgvFinancial.Size =
                new System.Drawing.Size(820, 545);
            this.dgvFinancial.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;

            this.tabPage3.Controls.Add(lblNote);
            this.tabPage3.Controls.Add(this.dgvFinancial);
        }

        // ── Tab4 構築 ─────────────────────────────────────
        private void BuildTab4()
        {
            var scrollPanel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                AutoScroll = true,
            };

            this.pnlValuation.Size =
                new System.Drawing.Size(820, 900);
            this.pnlValuation.AutoSize = false;

            int y = 8;
            int lw = 140, tw = 200, rw = 300;

            // ── 純資産法 ──────────────────────────────────
            y = AddValuationSection(
                this.pnlValuation, "① 純資産法", y);

            this.txtValNetAsset = AddValRow(
                pnlValuation, "修正純資産（千円）",
                ref y, lw, tw);
            this.txtValNetNote = AddValRow(
                pnlValuation, "備考",
                ref y, lw, tw, multiLine: true, height: 40);

            this.lblValNetAssetResult = AddResultLabel(
                pnlValuation, ref y, rw);

            // ── EBITDAマルチプル ──────────────────────────
            y += 8;
            y = AddValuationSection(
                this.pnlValuation, "② EBITDAマルチプル", y);

            this.txtValEBITDA = AddValRow(
                pnlValuation, "EBITDA（千円）", ref y, lw, tw);
            this.txtValEBITDAYear = AddValRow(
                pnlValuation, "基準年度", ref y, lw, tw);
            this.txtValMultiple = AddValRow(
                pnlValuation, "マルチプル（倍）", ref y, lw, tw);
            this.txtValEBITDANet = AddValRow(
                pnlValuation, "ネットキャッシュ（千円）", ref y, lw, tw);
            this.txtValEBITDANote = AddValRow(
                pnlValuation, "備考",
                ref y, lw, tw, multiLine: true, height: 40);

            this.lblValEBITDAResult = AddResultLabel(
                pnlValuation, ref y, rw);

            // 自動計算イベント登録
            this.txtValEBITDA.TextChanged +=
                new System.EventHandler(this.ValuationInput_Changed);
            this.txtValMultiple.TextChanged +=
                new System.EventHandler(this.ValuationInput_Changed);
            this.txtValEBITDANet.TextChanged +=
                new System.EventHandler(this.ValuationInput_Changed);

            // ── DCF法 ─────────────────────────────────────
            y += 8;
            y = AddValuationSection(
                this.pnlValuation, "③ DCF法", y);

            this.txtValDCFRate = AddValRow(
                pnlValuation, "割引率（%）", ref y, lw, tw);
            this.txtValDCFGrowth = AddValRow(
                pnlValuation, "永続成長率（%）", ref y, lw, tw);
            this.txtValDCFEV = AddValRow(
                pnlValuation, "EV（千円）", ref y, lw, tw);
            this.txtValDCFNet = AddValRow(
                pnlValuation, "ネットキャッシュ（千円）", ref y, lw, tw);
            this.txtValDCFNote = AddValRow(
                pnlValuation, "備考",
                ref y, lw, tw, multiLine: true, height: 40);

            this.lblValDCFResult = AddResultLabel(
                pnlValuation, ref y, rw);

            this.txtValDCFEV.TextChanged +=
                new System.EventHandler(this.ValuationInput_Changed);
            this.txtValDCFNet.TextChanged +=
                new System.EventHandler(this.ValuationInput_Changed);

            // ── 直接還元法 ────────────────────────────────
            y += 8;
            y = AddValuationSection(
                this.pnlValuation, "④ 直接還元法（直接還元法）", y);

            this.txtValNOI = AddValRow(
                pnlValuation, "NOI（千円）", ref y, lw, tw);
            this.txtValCapRate = AddValRow(
                pnlValuation, "キャップレート（%）", ref y, lw, tw);
            this.txtValDirectNet = AddValRow(
                pnlValuation, "ネットキャッシュ（千円）", ref y, lw, tw);
            this.txtValDirectNote = AddValRow(
                pnlValuation, "備考",
                ref y, lw, tw, multiLine: true, height: 40);

            this.lblValDirectResult = AddResultLabel(
                pnlValuation, ref y, rw);

            this.txtValNOI.TextChanged +=
                new System.EventHandler(this.ValuationInput_Changed);
            this.txtValCapRate.TextChanged +=
                new System.EventHandler(this.ValuationInput_Changed);
            this.txtValDirectNet.TextChanged +=
                new System.EventHandler(this.ValuationInput_Changed);

            // ── サマリー ──────────────────────────────────
            y += 16;
            this.lblValSummary = new System.Windows.Forms.Label
            {
                Location = new System.Drawing.Point(8, y),
                Size = new System.Drawing.Size(790, 28),
                Font = new System.Drawing.Font(
                    "Yu Gothic UI", 11F,
                    System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.DarkBlue,
                Text = "試算結果がありません",
            };
            this.pnlValuation.Controls.Add(this.lblValSummary);
            y += 36;

            // ── 総合備考 ──────────────────────────────────
            this.pnlValuation.Controls.Add(
                new System.Windows.Forms.Label
                {
                    Text = "総合備考:",
                    Location = new System.Drawing.Point(8, y),
                    Size = new System.Drawing.Size(lw, 23),
                    TextAlign =
                        System.Drawing.ContentAlignment.MiddleRight,
                });

            this.txtValNote = new System.Windows.Forms.TextBox
            {
                Location = new System.Drawing.Point(lw + 12, y),
                Size = new System.Drawing.Size(640, 60),
                Multiline = true,
                ScrollBars =
                    System.Windows.Forms.ScrollBars.Vertical,
            };
            this.pnlValuation.Controls.Add(this.txtValNote);

            scrollPanel.Controls.Add(this.pnlValuation);
            this.tabPage4.Controls.Add(scrollPanel);
        }

        // ── Tab4 ヘルパー ─────────────────────────────────
        private static int AddValuationSection(
            System.Windows.Forms.Panel panel,
            string title, int y)
        {
            var lbl = new System.Windows.Forms.Label
            {
                Text = title,
                Location = new System.Drawing.Point(4, y),
                Size = new System.Drawing.Size(800, 24),
                Font = new System.Drawing.Font(
                    "Yu Gothic UI", 10F,
                    System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.SteelBlue,
                ForeColor = System.Drawing.Color.White,
            };
            panel.Controls.Add(lbl);
            return y + 28;
        }

        private static System.Windows.Forms.TextBox AddValRow(
            System.Windows.Forms.Panel panel,
            string label, ref int y,
            int lw, int tw,
            bool multiLine = false, int height = 26)
        {
            panel.Controls.Add(new System.Windows.Forms.Label
            {
                Text = label,
                Location = new System.Drawing.Point(8, y + 2),
                Size = new System.Drawing.Size(lw, height),
                TextAlign =
                    System.Drawing.ContentAlignment.MiddleRight,
                Font =
                    new System.Drawing.Font("Yu Gothic UI", 9F),
            });

            var txt = new System.Windows.Forms.TextBox
            {
                Location =
                    new System.Drawing.Point(lw + 12, y),
                Size = new System.Drawing.Size(tw, height),
                Multiline = multiLine,
                ScrollBars = multiLine
                    ? System.Windows.Forms.ScrollBars.Vertical
                    : System.Windows.Forms.ScrollBars.None,
                Font =
                    new System.Drawing.Font("Yu Gothic UI", 9F),
            };
            panel.Controls.Add(txt);
            y += height + 4;
            return txt;
        }

        private static System.Windows.Forms.Label AddResultLabel(
            System.Windows.Forms.Panel panel,
            ref int y, int width)
        {
            var lbl = new System.Windows.Forms.Label
            {
                Location = new System.Drawing.Point(8, y),
                Size = new System.Drawing.Size(width, 24),
                ForeColor = System.Drawing.Color.DarkGreen,
                Font = new System.Drawing.Font(
                    "Yu Gothic UI", 9.5F,
                    System.Drawing.FontStyle.Bold),
                Text = string.Empty,
            };
            panel.Controls.Add(lbl);
            y += 28;
            return lbl;
        }

        // ── フィールドセッター ────────────────────────────
        private void SetTextField(
            string fieldName,
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

        private void SetProfileField(
            string fieldName,
            System.Windows.Forms.TextBox txt)
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
    }
}