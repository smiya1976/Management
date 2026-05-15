using MAItems.Database;
using MAItems.MailParser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MAItems
{
    public partial class DetailForm : Form
    {
        private readonly DatabaseHelper _db;
        private readonly Deal _deal;

        // 拡張データ
        private CompanyProfile _profile = new();
        private List<FinancialHighlight> _highlights = new();
        private ValuationData _valuation = new();

        public event EventHandler? SaveCompleted;

        public DetailForm(Deal deal, DatabaseHelper db)
        {
            InitializeComponent();
            _deal = deal;
            _db = db;

            BuildFinancialGrid();
            LoadAll();
        }

        // ══════════════════════════════════════════════════════
        // 全タブ読み込み
        // ══════════════════════════════════════════════════════

        private void LoadAll()
        {
            LoadTab1();
            LoadTab2();
            LoadTab3();
            LoadTab4();
        }

        // ── Tab1：基本情報 ─────────────────────────────────
        private void LoadTab1()
        {
            this.Text = $"案件詳細  ［ID: {_deal.Id}］";

            txtInputDate.Text = _deal.InputDate;
            txtRoute.Text = _deal.Route;
            txtBrokerCompany.Text = _deal.BrokerCompany;
            txtTitle.Text = _deal.Title;
            txtDealId.Text = _deal.DealId;
            txtBusinessContent.Text = _deal.BusinessContent;
            txtArea.Text = _deal.Area;
            txtRevenue.Text = _deal.Revenue;
            txtOperatingProfit.Text = _deal.OperatingProfit;
            txtEBITDA.Text = _deal.EBITDA;
            txtNetAssets.Text = _deal.NetAssets;
            txtTotalAssets.Text = _deal.TotalAssets;
            txtNetCashDebt.Text = _deal.NetCashDebt;
            txtCashEquivalents.Text = _deal.CashEquivalents;
            txtInterestBearingDebt.Text = _deal.InterestBearingDebt;
            txtEmployeeCount.Text = _deal.EmployeeCount;
            txtFeatures.Text = _deal.Features;
            txtAskingPrice.Text = _deal.AskingPrice;
            txtTransferType.Text = _deal.TransferType;
            txtTransferReason.Text = _deal.TransferReason;
            txtTransferConditions.Text = _deal.TransferConditions;
            txtStatus.Text = _deal.Status;
        }

        private void FormToDeal()
        {
            _deal.InputDate = txtInputDate.Text.Trim();
            _deal.Route = txtRoute.Text.Trim();
            _deal.BrokerCompany = txtBrokerCompany.Text.Trim();
            _deal.Title = txtTitle.Text.Trim();
            _deal.DealId = txtDealId.Text.Trim();
            _deal.BusinessContent = txtBusinessContent.Text.Trim();
            _deal.Area = txtArea.Text.Trim();
            _deal.Revenue = txtRevenue.Text.Trim();
            _deal.OperatingProfit = txtOperatingProfit.Text.Trim();
            _deal.EBITDA = txtEBITDA.Text.Trim();
            _deal.NetAssets = txtNetAssets.Text.Trim();
            _deal.TotalAssets = txtTotalAssets.Text.Trim();
            _deal.NetCashDebt = txtNetCashDebt.Text.Trim();
            _deal.CashEquivalents = txtCashEquivalents.Text.Trim();
            _deal.InterestBearingDebt = txtInterestBearingDebt.Text.Trim();
            _deal.EmployeeCount = txtEmployeeCount.Text.Trim();
            _deal.Features = txtFeatures.Text.Trim();
            _deal.AskingPrice = txtAskingPrice.Text.Trim();
            _deal.TransferType = txtTransferType.Text.Trim();
            _deal.TransferReason = txtTransferReason.Text.Trim();
            _deal.TransferConditions = txtTransferConditions.Text.Trim();
            _deal.Status = txtStatus.Text.Trim();
        }

        // ── Tab2：会社基礎情報 ────────────────────────────
        private void LoadTab2()
        {
            var p = _db.GetCompanyProfile(_deal.Id)
                    ?? new CompanyProfile { DealId = _deal.Id };
            _profile = p;

            txtCpCompanyName.Text = p.CompanyName;
            txtCpCompanyNameSub.Text = p.CompanyNameSub;
            txtCpHeadOffice.Text = p.HeadOfficeAddress;
            txtCpFactory.Text = p.FactoryAddress;
            txtCpOtherOffice.Text = p.OtherOffice;
            txtCpFounded.Text = p.Founded;
            txtCpFounded2.Text = p.Founded2;
            txtCpCapital.Text = p.Capital;
            txtCpRepName.Text = p.RepresentativeName;
            txtCpRepProfile.Text = p.RepresentativeProfile;
            txtCpShareholder.Text = p.ShareholderInfo;
            txtCpBusiness.Text = p.BusinessDetail;
            txtCpRevenue.Text = p.Revenue;
            txtCpEmployees.Text = p.Employees;
            txtCpClients.Text = p.MainClients;
            txtCpSuppliers.Text = p.MainSuppliers;
            txtCpCertifications.Text = p.Certifications;
            txtCpGroupCompanies.Text = p.GroupCompanies;
            txtCpTransferReason.Text = p.TransferReason;
            txtCpRemarks.Text = p.Remarks;
        }

        private void FormToProfile()
        {
            _profile.DealId = _deal.Id;
            _profile.CompanyName = txtCpCompanyName.Text.Trim();
            _profile.CompanyNameSub = txtCpCompanyNameSub.Text.Trim();
            _profile.HeadOfficeAddress = txtCpHeadOffice.Text.Trim();
            _profile.FactoryAddress = txtCpFactory.Text.Trim();
            _profile.OtherOffice = txtCpOtherOffice.Text.Trim();
            _profile.Founded = txtCpFounded.Text.Trim();
            _profile.Founded2 = txtCpFounded2.Text.Trim();
            _profile.Capital = txtCpCapital.Text.Trim();
            _profile.RepresentativeName = txtCpRepName.Text.Trim();
            _profile.RepresentativeProfile = txtCpRepProfile.Text.Trim();
            _profile.ShareholderInfo = txtCpShareholder.Text.Trim();
            _profile.BusinessDetail = txtCpBusiness.Text.Trim();
            _profile.Revenue = txtCpRevenue.Text.Trim();
            _profile.Employees = txtCpEmployees.Text.Trim();
            _profile.MainClients = txtCpClients.Text.Trim();
            _profile.MainSuppliers = txtCpSuppliers.Text.Trim();
            _profile.Certifications = txtCpCertifications.Text.Trim();
            _profile.GroupCompanies = txtCpGroupCompanies.Text.Trim();
            _profile.TransferReason = txtCpTransferReason.Text.Trim();
            _profile.Remarks = txtCpRemarks.Text.Trim();
        }

        // ── Tab3：財務ハイライト ──────────────────────────
        private void BuildFinancialGrid()
        {
            dgvFinancial.AllowUserToAddRows = false;
            dgvFinancial.ReadOnly = false;
            dgvFinancial.RowHeadersVisible = true;
            dgvFinancial.ColumnHeadersVisible = true;
            dgvFinancial.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.None;
            dgvFinancial.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.None;
            dgvFinancial.ScrollBars = ScrollBars.Both;
            dgvFinancial.Columns.Clear();

            // 列定義：項目名列 + 実績3期 + 予測3期
            dgvFinancial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Item",
                HeaderText = "項目（千円）",
                Width = 160,
                ReadOnly = true,
                Frozen = true,
            });

            string[] periodTypes = { "actual",  "actual",  "actual",
                                       "forecast","forecast","forecast" };
            string[] periodLabels = { "実績1期", "実績2期", "実績3期",
                                       "予測1期", "予測2期", "予測3期" };

            for (int i = 0; i < 6; i++)
            {
                dgvFinancial.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = $"col_{periodTypes[i]}_{i % 3 + 1}",
                    HeaderText = periodLabels[i],
                    Width = 110,
                    DefaultCellStyle =
                    {
                        Alignment = DataGridViewContentAlignment.MiddleRight,
                    },
                });
            }

            // 行定義
            var rows = new (string label, string field, bool isSection)[]
            {
                ("── PL ──────",    "",                  true),
                ("売上高",          "Revenue",           false),
                ("原価率(%)",       "CostRate",          false),
                ("粗利益",          "GrossProfit",       false),
                ("粗利率(%)",       "GrossProfitRate",   false),
                ("販管費",          "SGA",               false),
                ("営業利益",        "OperatingProfit",   false),
                ("営業利益率(%)",   "OperatingProfitRate",false),
                ("経常利益",        "OrdinaryProfit",    false),
                ("当期純利益",      "NetIncome",         false),
                ("EBITDA",          "EBITDA",            false),
                ("減価償却費",      "Depreciation",      false),
                ("設備投資額",      "CapEx",             false),
                ("── BS（資産） ──", "",                 true),
                ("流動資産",        "CurrentAssets",     false),
                ("　現金預金",      "CashEquivalents",   false),
                ("　売掛金",        "AccountsReceivable",false),
                ("　棚卸資産",      "Inventory",         false),
                ("　その他流動",    "OtherCurrentAssets",false),
                ("固定資産",        "FixedAssets",       false),
                ("総資産",          "TotalAssets",       false),
                ("── BS（負債） ──", "",                 true),
                ("流動負債",        "CurrentLiabilities",false),
                ("　買掛金",        "AccountsPayable",   false),
                ("　短期借入金",    "ShortTermDebt",     false),
                ("　その他流動",    "OtherCurrentLiabilities",false),
                ("固定負債",        "FixedLiabilities",  false),
                ("　長期借入金",    "LongTermDebt",      false),
                ("　その他固定",    "OtherFixedLiabilities",false),
                ("負債合計",        "TotalLiabilities",  false),
                ("純資産合計",      "NetAssets",         false),
                ("　利益剰余金",    "RetainedEarnings",  false),
            };

            dgvFinancial.Rows.Clear();
            foreach (var (label, field, isSection) in rows)
            {
                int idx = dgvFinancial.Rows.Add();
                var row = dgvFinancial.Rows[idx];
                row.Cells["Item"].Value = label;
                row.Tag = field;

                if (isSection)
                {
                    // セクションヘッダー行はグレー背景・編集不可
                    row.DefaultCellStyle.BackColor =
                        Color.LightSlateGray;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    row.DefaultCellStyle.Font =
                        new Font(dgvFinancial.Font, FontStyle.Bold);
                    row.ReadOnly = true;
                }
            }
        }

        private void LoadTab3()
        {
            _highlights = _db.GetFinancialHighlights(_deal.Id);

            // 期ラベルをヘッダーに反映
            string[] colNames =
            {
                "col_actual_1","col_actual_2","col_actual_3",
                "col_forecast_1","col_forecast_2","col_forecast_3",
            };
            string[] periodTypes =
            {
                "actual","actual","actual",
                "forecast","forecast","forecast"
            };
            int[] periodOrders = { 1, 2, 3, 1, 2, 3 };

            for (int c = 0; c < 6; c++)
            {
                var hl = _highlights.Find(h =>
                    h.PeriodType == periodTypes[c] &&
                    h.PeriodOrder == periodOrders[c]);

                if (hl != null &&
                    !string.IsNullOrEmpty(hl.PeriodLabel))
                {
                    dgvFinancial.Columns[colNames[c]].HeaderText
                        = hl.PeriodLabel;
                }
            }

            // グリッドに値をセット
            foreach (DataGridViewRow row in dgvFinancial.Rows)
            {
                string field = row.Tag as string ?? string.Empty;
                if (string.IsNullOrEmpty(field)) continue;

                for (int c = 0; c < 6; c++)
                {
                    var hl = _highlights.Find(h =>
                        h.PeriodType == periodTypes[c] &&
                        h.PeriodOrder == periodOrders[c]);

                    if (hl == null) continue;

                    double? val = GetHighlightField(hl, field);
                    row.Cells[colNames[c]].Value =
                        val.HasValue ? (object)val.Value : DBNull.Value;
                }
            }
        }

        /// <summary>フィールド名からプロパティ値を取得</summary>
        private static double? GetHighlightField(
            FinancialHighlight h, string field)
        {
            return field switch
            {
                "Revenue" => h.Revenue,
                "CostRate" => h.CostRate,
                "GrossProfit" => h.GrossProfit,
                "GrossProfitRate" => h.GrossProfitRate,
                "SGA" => h.SGA,
                "OperatingProfit" => h.OperatingProfit,
                "OperatingProfitRate" => h.OperatingProfitRate,
                "OrdinaryProfit" => h.OrdinaryProfit,
                "NetIncome" => h.NetIncome,
                "EBITDA" => h.EBITDA,
                "Depreciation" => h.Depreciation,
                "CapEx" => h.CapEx,
                "CurrentAssets" => h.CurrentAssets,
                "CashEquivalents" => h.CashEquivalents,
                "AccountsReceivable" => h.AccountsReceivable,
                "Inventory" => h.Inventory,
                "OtherCurrentAssets" => h.OtherCurrentAssets,
                "FixedAssets" => h.FixedAssets,
                "TotalAssets" => h.TotalAssets,
                "CurrentLiabilities" => h.CurrentLiabilities,
                "AccountsPayable" => h.AccountsPayable,
                "ShortTermDebt" => h.ShortTermDebt,
                "OtherCurrentLiabilities" => h.OtherCurrentLiabilities,
                "FixedLiabilities" => h.FixedLiabilities,
                "LongTermDebt" => h.LongTermDebt,
                "OtherFixedLiabilities" => h.OtherFixedLiabilities,
                "TotalLiabilities" => h.TotalLiabilities,
                "NetAssets" => h.NetAssets,
                "RetainedEarnings" => h.RetainedEarnings,
                _ => null,
            };
        }

        /// <summary>フィールド名でプロパティに値をセット</summary>
        private static void SetHighlightField(
            FinancialHighlight h, string field, double? val)
        {
            switch (field)
            {
                case "Revenue": h.Revenue = val; break;
                case "CostRate": h.CostRate = val; break;
                case "GrossProfit": h.GrossProfit = val; break;
                case "GrossProfitRate": h.GrossProfitRate = val; break;
                case "SGA": h.SGA = val; break;
                case "OperatingProfit": h.OperatingProfit = val; break;
                case "OperatingProfitRate": h.OperatingProfitRate = val; break;
                case "OrdinaryProfit": h.OrdinaryProfit = val; break;
                case "NetIncome": h.NetIncome = val; break;
                case "EBITDA": h.EBITDA = val; break;
                case "Depreciation": h.Depreciation = val; break;
                case "CapEx": h.CapEx = val; break;
                case "CurrentAssets": h.CurrentAssets = val; break;
                case "CashEquivalents": h.CashEquivalents = val; break;
                case "AccountsReceivable": h.AccountsReceivable = val; break;
                case "Inventory": h.Inventory = val; break;
                case "OtherCurrentAssets": h.OtherCurrentAssets = val; break;
                case "FixedAssets": h.FixedAssets = val; break;
                case "TotalAssets": h.TotalAssets = val; break;
                case "CurrentLiabilities": h.CurrentLiabilities = val; break;
                case "AccountsPayable": h.AccountsPayable = val; break;
                case "ShortTermDebt": h.ShortTermDebt = val; break;
                case "OtherCurrentLiabilities": h.OtherCurrentLiabilities = val; break;
                case "FixedLiabilities": h.FixedLiabilities = val; break;
                case "LongTermDebt": h.LongTermDebt = val; break;
                case "OtherFixedLiabilities": h.OtherFixedLiabilities = val; break;
                case "TotalLiabilities": h.TotalLiabilities = val; break;
                case "NetAssets": h.NetAssets = val; break;
                case "RetainedEarnings": h.RetainedEarnings = val; break;
            }
        }

        private List<FinancialHighlight> GridToHighlights()
        {
            string[] colNames =
            {
                "col_actual_1","col_actual_2","col_actual_3",
                "col_forecast_1","col_forecast_2","col_forecast_3",
            };
            string[] periodTypes =
            {
                "actual","actual","actual",
                "forecast","forecast","forecast"
            };
            int[] periodOrders = { 1, 2, 3, 1, 2, 3 };

            var result = new List<FinancialHighlight>();

            for (int c = 0; c < 6; c++)
            {
                var hl = new FinancialHighlight
                {
                    DealId = _deal.Id,
                    PeriodType = periodTypes[c],
                    PeriodOrder = periodOrders[c],
                    PeriodLabel = dgvFinancial
                        .Columns[colNames[c]].HeaderText,
                };

                foreach (DataGridViewRow row in dgvFinancial.Rows)
                {
                    string field = row.Tag as string ?? string.Empty;
                    if (string.IsNullOrEmpty(field)) continue;

                    var cell = row.Cells[colNames[c]];
                    double? val = null;
                    if (cell.Value != null &&
                        cell.Value != DBNull.Value &&
                        double.TryParse(
                            cell.Value.ToString(), out double d))
                        val = d;

                    SetHighlightField(hl, field, val);
                }

                result.Add(hl);
            }

            return result;
        }

        // ── Tab4：株式価値試算 ────────────────────────────
        private void LoadTab4()
        {
            var v = _db.GetValuationData(_deal.Id)
                    ?? new ValuationData { DealId = _deal.Id };
            _valuation = v;

            // 純資産法
            txtValNetAsset.Text = N(v.NetAssetValue);
            txtValNetNote.Text = v.NetAssetNote;

            // EBITDAマルチプル
            txtValEBITDA.Text = N(v.EBITDABase);
            txtValEBITDAYear.Text = v.EBITDABaseYear;
            txtValMultiple.Text = N(v.EBITDAMultiple);
            txtValEBITDANet.Text = N(v.EBITDANetCashDebt);
            txtValEBITDANote.Text = v.EBITDANote;

            // DCF法
            txtValDCFRate.Text = N(v.DCFDiscountRate);
            txtValDCFGrowth.Text = N(v.DCFTerminalGrowth);
            txtValDCFEV.Text = N(v.DCFEV);
            txtValDCFNet.Text = N(v.DCFNetCashDebt);
            txtValDCFNote.Text = v.DCFNote;

            // 直接還元法
            txtValNOI.Text = N(v.NOI);
            txtValCapRate.Text = N(v.CapRate);
            txtValDirectNet.Text = N(v.DirectNetCashDebt);
            txtValDirectNote.Text = v.DirectNote;

            // 備考
            txtValNote.Text = v.ValuationNote;

            // 計算結果を表示
            RecalcValuation();
        }

        private static string N(double? v)
            => v.HasValue ? v.Value.ToString("N0") : string.Empty;

        private static double? Parse(string s)
        {
            string cleaned = s.Replace(",", "").Trim();
            return double.TryParse(cleaned, out double d) ? d : null;
        }

        /// <summary>入力値から株式価値を自動計算して表示</summary>
        private void RecalcValuation()
        {
            // ── EBITDAマルチプル ─────────────────────────
            double? ebitda = Parse(txtValEBITDA.Text);
            double? multiple = Parse(txtValMultiple.Text);
            double? ebitdaNet = Parse(txtValEBITDANet.Text);

            if (ebitda.HasValue && multiple.HasValue)
            {
                double ev = ebitda.Value * multiple.Value;
                double eq = ev + (ebitdaNet ?? 0);
                _valuation.EBITDAEquityValue = eq;
                lblValEBITDAResult.Text =
                    $"EV: {ev:N0} 千円　→　株式価値: {eq:N0} 千円";
            }
            else
            {
                lblValEBITDAResult.Text = "（入力値が不足しています）";
            }

            // ── DCF法 ─────────────────────────────────────
            double? dcfEV = Parse(txtValDCFEV.Text);
            double? dcfNet = Parse(txtValDCFNet.Text);

            if (dcfEV.HasValue)
            {
                double eq = dcfEV.Value + (dcfNet ?? 0);
                _valuation.DCFEquityValue = eq;
                lblValDCFResult.Text =
                    $"株式価値: {eq:N0} 千円";
            }
            else
            {
                lblValDCFResult.Text = "（入力値が不足しています）";
            }

            // ── 直接還元法 ────────────────────────────────
            double? noi = Parse(txtValNOI.Text);
            double? capRate = Parse(txtValCapRate.Text);
            double? dirNet = Parse(txtValDirectNet.Text);

            if (noi.HasValue && capRate.HasValue && capRate.Value != 0)
            {
                double ev = noi.Value / (capRate.Value / 100.0);
                double eq = ev + (dirNet ?? 0);
                _valuation.DirectEquityValue = eq;
                lblValDirectResult.Text =
                    $"EV: {ev:N0} 千円　→　株式価値: {eq:N0} 千円";
            }
            else
            {
                lblValDirectResult.Text = "（入力値が不足しています）";
            }

            // ── 純資産法 ──────────────────────────────────
            double? netAsset = Parse(txtValNetAsset.Text);
            if (netAsset.HasValue)
                lblValNetAssetResult.Text =
                    $"株式価値: {netAsset.Value:N0} 千円";
            else
                lblValNetAssetResult.Text = "（入力値が不足しています）";

            // ── サマリー ──────────────────────────────────
            UpdateValuationSummary();
        }

        private void UpdateValuationSummary()
        {
            var values = new List<double>();

            if (_valuation.NetAssetValue.HasValue)
                values.Add(_valuation.NetAssetValue.Value);
            if (_valuation.EBITDAEquityValue.HasValue)
                values.Add(_valuation.EBITDAEquityValue.Value);
            if (_valuation.DCFEquityValue.HasValue)
                values.Add(_valuation.DCFEquityValue.Value);
            if (_valuation.DirectEquityValue.HasValue)
                values.Add(_valuation.DirectEquityValue.Value);

            if (values.Count == 0)
            {
                lblValSummary.Text = "試算結果がありません";
                return;
            }

            double min = double.MaxValue, max = double.MinValue;
            foreach (double v in values)
            {
                if (v < min) min = v;
                if (v > max) max = v;
            }

            lblValSummary.Text =
                $"株式価値レンジ：{min:N0} 〜 {max:N0} 千円";
        }

        private void FormToValuation()
        {
            _valuation.DealId = _deal.Id;
            _valuation.NetAssetValue = Parse(txtValNetAsset.Text);
            _valuation.NetAssetNote = txtValNetNote.Text.Trim();
            _valuation.EBITDABase = Parse(txtValEBITDA.Text);
            _valuation.EBITDABaseYear = txtValEBITDAYear.Text.Trim();
            _valuation.EBITDAMultiple = Parse(txtValMultiple.Text);
            _valuation.EBITDANetCashDebt = Parse(txtValEBITDANet.Text);
            _valuation.EBITDANote = txtValEBITDANote.Text.Trim();
            _valuation.DCFDiscountRate = Parse(txtValDCFRate.Text);
            _valuation.DCFTerminalGrowth = Parse(txtValDCFGrowth.Text);
            _valuation.DCFEV = Parse(txtValDCFEV.Text);
            _valuation.DCFNetCashDebt = Parse(txtValDCFNet.Text);
            _valuation.DCFNote = txtValDCFNote.Text.Trim();
            _valuation.NOI = Parse(txtValNOI.Text);
            _valuation.CapRate = Parse(txtValCapRate.Text);
            _valuation.DirectNetCashDebt = Parse(txtValDirectNet.Text);
            _valuation.DirectNote = txtValDirectNote.Text.Trim();
            _valuation.ValuationNote = txtValNote.Text.Trim();

            RecalcValuation();
        }

        // ── 自動計算イベント ──────────────────────────────
        private void ValuationInput_Changed(object sender, EventArgs e)
            => RecalcValuation();

        // ══════════════════════════════════════════════════════
        // ボタンイベント
        // ══════════════════════════════════════════════════════

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Tab1
                FormToDeal();
                _db.UpdateDeal(_deal);

                // Tab2
                FormToProfile();
                _db.UpsertCompanyProfile(_profile);

                // Tab3
                var highlights = GridToHighlights();
                foreach (var hl in highlights)
                    _db.UpsertFinancialHighlight(hl);

                // Tab4
                FormToValuation();
                _db.UpsertValuationData(_valuation);

                SetStatus("✔ 保存しました", isError: false);
                SaveCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                SetStatus($"❌ 保存エラー: {ex.Message}", isError: true);
            }
        }

        private void btnPasteFromMail_Click(object sender, EventArgs e)
        {
            string mailBody = Clipboard.GetText();

            if (string.IsNullOrWhiteSpace(mailBody))
            {
                SetStatus("⚠ クリップボードにテキストがありません",
                    isError: true);
                return;
            }

            var parser = MailParserFactory.GetParser(mailBody);

            if (parser == null)
            {
                SetStatus(
                    "⚠ 対応する仲介会社のフォーマットが見つかりません",
                    isError: true);
                return;
            }

            ParsedDeal parsed = parser.Parse(mailBody);
            ApplyParsedDeal(parsed);

            SetStatus(
                $"✔ メール本文を取り込みました（{parsed.BrokerCompany}）",
                isError: false);
        }

        private void ApplyParsedDeal(ParsedDeal parsed)
        {
            if (parsed.InputDate != null)
                txtInputDate.Text = parsed.InputDate;
            if (parsed.Route != null)
                txtRoute.Text = parsed.Route;
            if (parsed.BrokerCompany != null)
                txtBrokerCompany.Text = parsed.BrokerCompany;
            if (parsed.Title != null)
                txtTitle.Text = parsed.Title;
            if (parsed.DealId != null)
                txtDealId.Text = parsed.DealId;
            if (parsed.BusinessContent != null)
                txtBusinessContent.Text = parsed.BusinessContent;
            if (parsed.Area != null)
                txtArea.Text = parsed.Area;
            if (parsed.Revenue != null)
                txtRevenue.Text = parsed.Revenue;
            if (parsed.OperatingProfit != null)
                txtOperatingProfit.Text = parsed.OperatingProfit;
            if (parsed.EBITDA != null)
                txtEBITDA.Text = parsed.EBITDA;
            if (parsed.NetAssets != null)
                txtNetAssets.Text = parsed.NetAssets;
            if (parsed.TotalAssets != null)
                txtTotalAssets.Text = parsed.TotalAssets;
            if (parsed.NetCashDebt != null)
                txtNetCashDebt.Text = parsed.NetCashDebt;
            if (parsed.CashEquivalents != null)
                txtCashEquivalents.Text = parsed.CashEquivalents;
            if (parsed.InterestBearingDebt != null)
                txtInterestBearingDebt.Text = parsed.InterestBearingDebt;
            if (parsed.EmployeeCount != null)
                txtEmployeeCount.Text = parsed.EmployeeCount;
            if (parsed.Features != null)
                txtFeatures.Text = parsed.Features;
            if (parsed.AskingPrice != null)
                txtAskingPrice.Text = parsed.AskingPrice;
            if (parsed.TransferType != null)
                txtTransferType.Text = parsed.TransferType;
            if (parsed.TransferReason != null)
                txtTransferReason.Text = parsed.TransferReason;
            if (parsed.TransferConditions != null)
                txtTransferConditions.Text = parsed.TransferConditions;
            if (parsed.Status != null)
                txtStatus.Text = parsed.Status;
        }

        private void btnClose_Click(object sender, EventArgs e)
            => this.Close();

        private void SetStatus(string msg, bool isError)
        {
            lblStatus.ForeColor = isError ? Color.Red : Color.DarkGreen;
            lblStatus.Text = msg;
        }
    }
}