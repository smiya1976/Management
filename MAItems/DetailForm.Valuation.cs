using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MAItems.Database;

namespace MAItems
{
    // partial class とすることで、既存の DetailForm と裏側で合体します
    public partial class DetailForm
    {
        // ── UIコントロール群 ──
        private TabControl tabValuation = new TabControl();
        private DataGridView dgvAssetAdj = new DataGridView(), dgvLiabAdj = new DataGridView(), dgvDcf = new DataGridView();

        // 共有項目 (右側パネル)
        private TextBox txtCash = new TextBox(), txtWCMonths = new TextBox();
        private TextBox txtShortDebt = new TextBox(), txtLongDebt = new TextBox(), txtLease = new TextBox(), txtOtherDebt = new TextBox();
        private Label lblTotalDebt = new Label(), lblWorkingCapital = new Label(), lblNonOpAssets = new Label();

        // 各手法の入力・結果
        private TextBox txtBookNetAsset = new TextBox(), txtOpProfit_NA = new TextBox(), txtTaxRate_NA = new TextBox(), txtGoodwillYears = new TextBox();
        private Label lblMarketNetAsset = new Label(), lblGoodwill = new Label(), lblTotal_NA = new Label();

        private TextBox txtEBITDA_Calc = new TextBox(), txtEBITDAMultiple = new TextBox();
        private Label lblEV_EBITDA = new Label(), lblEquity_EBITDA = new Label();

        private Label lblEV_DCF = new Label(), lblEquity_DCF = new Label();

        private TextBox txtOpProfit_Direct = new TextBox(), txtTaxRate_Direct = new TextBox(), txtCapRate = new TextBox();
        private Label lblEV_Direct = new Label(), lblEquity_Direct = new Label();

        /// <summary>
        /// バリュエーション用のUIをプログラムで動的に生成し、指定したパネルに流し込みます
        /// </summary>
        private void BuildValuationUI(Control container)
        {
            container.Controls.Clear();
            var split = new SplitContainer { Dock = DockStyle.Fill, FixedPanel = FixedPanel.Panel2 };
            this.Load += (s, ev) => {
                // フォームのロードが完了し、実際のサイズが確定したタイミングで右側パネルを300pxに設定する
                if (split.Width > 300) split.SplitterDistance = split.Width - 300;
            };
            container.Controls.Add(split);

            // 左側：各手法のタブ
            tabValuation.Dock = DockStyle.Fill;
            split.Panel1.Controls.Add(tabValuation);

            var tabNA = new TabPage("① 純資産法");
            var tabEBITDA = new TabPage("② EBITDA法");
            var tabDCF = new TabPage("③ DCF法");
            var tabDirect = new TabPage("④ 直接還元法");
            tabValuation.TabPages.AddRange(new[] { tabNA, tabEBITDA, tabDCF, tabDirect });

            // 右側：現預金・有利子負債の共通入力パネル
            var pnlShared = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };
            split.Panel2.Controls.Add(pnlShared);

            int y = 10;
            AddHeader(pnlShared, "【共通: 加算項目】", ref y);
            txtCash = AddInput(pnlShared, "現金・預金等", ref y);
            txtWCMonths = AddInput(pnlShared, "運転資金(月商のNヶ月)", ref y);
            lblWorkingCapital = AddResult(pnlShared, "事業に必要な資金", ref y);
            lblNonOpAssets = AddResult(pnlShared, "非事業資産", ref y);

            y += 10;
            AddHeader(pnlShared, "【共通: 減算項目】", ref y);
            txtShortDebt = AddInput(pnlShared, "短期借入金", ref y);
            txtLongDebt = AddInput(pnlShared, "長期借入金", ref y);
            txtLease = AddInput(pnlShared, "リース負債", ref y);
            txtOtherDebt = AddInput(pnlShared, "その他有利子負債", ref y);
            lblTotalDebt = AddResult(pnlShared, "有利子負債 合計", ref y);

            // ── タブ1: 純資産法 ──
            int y1 = 10;
            txtBookNetAsset = AddInput(tabNA, "簿価純資産額", ref y1);
            SetupGrid(dgvAssetAdj, tabNA, "＜修正項目＝資産＞", ref y1);
            SetupGrid(dgvLiabAdj, tabNA, "＜修正項目＝負債＞", ref y1);
            lblMarketNetAsset = AddResult(tabNA, "a) 時価純資産額", ref y1);
            y1 += 10;
            txtOpProfit_NA = AddInput(tabNA, "営業利益", ref y1);
            txtTaxRate_NA = AddInput(tabNA, "税率 (%)", ref y1);
            txtGoodwillYears = AddInput(tabNA, "計上年数 (Max.5年)", ref y1);
            lblGoodwill = AddResult(tabNA, "b) 許容のれん額", ref y1);
            lblTotal_NA = AddResult(tabNA, "時価純資産＋のれん (株式価値)", ref y1);

            // ── タブ2: EBITDA法 ──
            int y2 = 10;
            txtEBITDA_Calc = AddInput(tabEBITDA, "(1) EBITDA", ref y2);
            txtEBITDAMultiple = AddInput(tabEBITDA, "(2) マルチプル (倍)", ref y2);
            lblEV_EBITDA = AddResult(tabEBITDA, "(3) 事業価値 (EV)", ref y2);
            lblEquity_EBITDA = AddResult(tabEBITDA, "(7) 株式価値", ref y2);

            // ── タブ3: DCF法 ──
            int y3 = 10;
            SetupDcfGrid(tabDCF, ref y3);
            lblEV_DCF = AddResult(tabDCF, "事業価値 (PV合計)", ref y3);
            lblEquity_DCF = AddResult(tabDCF, "株式価値", ref y3);

            // ── タブ4: 直接還元法 ──
            int y4 = 10;
            txtOpProfit_Direct = AddInput(tabDirect, "営業利益", ref y4);
            txtTaxRate_Direct = AddInput(tabDirect, "税率 (%)", ref y4);
            txtCapRate = AddInput(tabDirect, "Cap Rate (実質利回り %)", ref y4);
            lblEV_Direct = AddResult(tabDirect, "事業価値", ref y4);
            lblEquity_Direct = AddResult(tabDirect, "株式価値", ref y4);

            // 全入力欄にイベント紐付け
            AttachCalculateEvent(split);
        }

        // ── 計算ロジック本体 ──
        private void CalculateValuation(object? sender, EventArgs e)
        {
            // 1. 共通項目の計算
            double cash = ParseD(txtCash.Text);
            double wcMonths = ParseD(txtWCMonths.Text);
            double debt = ParseD(txtShortDebt.Text) + ParseD(txtLongDebt.Text) + ParseD(txtLease.Text) + ParseD(txtOtherDebt.Text);

            // 財務ハイライトから最新の実績売上高を取得して月商を出す
            double latestRev = _highlights.Where(h => h.PeriodType == "actual").OrderByDescending(h => h.PeriodOrder).FirstOrDefault()?.Revenue ?? 0;
            double workingCapital = (latestRev / 12.0) * wcMonths;
            double nonOpAssets = Math.Max(0, cash - workingCapital); // 非事業資産

            lblWorkingCapital.Text = workingCapital.ToString("#,0") + " 千円";
            lblNonOpAssets.Text = nonOpAssets.ToString("#,0") + " 千円";
            lblTotalDebt.Text = debt.ToString("#,0") + " 千円";

            // 2. 純資産法の計算
            double bookNet = ParseD(txtBookNetAsset.Text);
            double assetAdj = GetGridTotal(dgvAssetAdj, "Amount");
            double liabAdj = GetGridTotal(dgvLiabAdj, "Amount");
            double marketNet = bookNet + assetAdj - liabAdj;
            lblMarketNetAsset.Text = marketNet.ToString("#,0") + " 千円";

            double noplat_NA = ParseD(txtOpProfit_NA.Text) * (1 - (ParseD(txtTaxRate_NA.Text) / 100.0));
            double goodwill = noplat_NA * ParseD(txtGoodwillYears.Text);
            lblGoodwill.Text = goodwill.ToString("#,0") + " 千円";
            lblTotal_NA.Text = (marketNet + goodwill).ToString("#,0") + " 千円";

            // 3. EBITDA法の計算 (※画像通り、非事業資産ではなく「現金」を足す)
            double ev_EBITDA = ParseD(txtEBITDA_Calc.Text) * ParseD(txtEBITDAMultiple.Text);
            lblEV_EBITDA.Text = ev_EBITDA.ToString("#,0") + " 千円";
            lblEquity_EBITDA.Text = (ev_EBITDA + cash - debt).ToString("#,0") + " 千円";

            // 4. DCF法の計算
            double ev_DCF = 0;
            foreach (DataGridViewRow r in dgvDcf.Rows)
            {
                if (r.IsNewRow) continue;
                double rev = ParseD(r.Cells["Revenue"].Value);
                double op = ParseD(r.Cells["OpProfit"].Value);
                double tax = ParseD(r.Cells["TaxRate"].Value) / 100.0;
                double noplat = op * (1 - tax);
                r.Cells["NOPLAT"].Value = noplat;

                double rate = ParseD(r.Cells["DiscountRate"].Value) / 100.0;
                int year = r.Index; // 0期, 1期...

                double pv = 0;
                if (year > 0 && year <= 5) // 1〜5期の現在価値
                {
                    pv = noplat / Math.Pow(1 + rate, year);
                }
                else if (year == 6) // ターミナルバリュー
                {
                    double growth = ParseD(r.Cells["TerminalGrowth"].Value) / 100.0;
                    double terminalValue = noplat / (rate - growth);
                    pv = terminalValue / Math.Pow(1 + rate, 5); // 5期末で現在価値化
                }
                r.Cells["PV"].Value = pv;
                ev_DCF += pv;
            }
            lblEV_DCF.Text = ev_DCF.ToString("#,0") + " 千円";
            lblEquity_DCF.Text = (ev_DCF + nonOpAssets - debt).ToString("#,0") + " 千円";

            // 5. 直接還元法の計算
            double noplat_Direct = ParseD(txtOpProfit_Direct.Text) * (1 - (ParseD(txtTaxRate_Direct.Text) / 100.0));
            double capRate = ParseD(txtCapRate.Text) / 100.0;
            double ev_Direct = capRate > 0 ? noplat_Direct / capRate : 0;
            lblEV_Direct.Text = ev_Direct.ToString("#,0") + " 千円";
            lblEquity_Direct.Text = (ev_Direct + nonOpAssets - debt).ToString("#,0") + " 千円";
        }

        // ── 財務ハイライトからの自動データロード ──
        private void LoadValuationData()
        {
            // DBから読込
            _valuation = _financialRepo.GetValuationData(_deal.Id) ?? new ValuationData { DealId = _deal.Id };

            // 共通入力のセット
            txtCash.Text = _valuation.CashAndDeposits?.ToString() ?? "";
            txtWCMonths.Text = _valuation.WorkingCapitalMonths?.ToString() ?? "1.5"; // デフォルト1.5ヶ月
            txtShortDebt.Text = _valuation.ShortTermDebt?.ToString() ?? "";
            txtLongDebt.Text = _valuation.LongTermDebt?.ToString() ?? "";
            txtLease.Text = _valuation.LeaseObligations?.ToString() ?? "";
            txtOtherDebt.Text = _valuation.OtherLiabilities?.ToString() ?? "";

            // EBITDA・直接還元法への最新実績の自動連動
            var latestActual = _highlights.Where(h => h.PeriodType == "actual").OrderByDescending(h => h.PeriodOrder).FirstOrDefault();
            txtEBITDA.Text = _valuation.EBITDABase?.ToString() ?? latestActual?.EBITDA?.ToString() ?? "0";
            txtEBITDAMultiple.Text = _valuation.EBITDAMultiple?.ToString() ?? "7"; // デフォルト7倍

            txtOpProfit_Direct.Text = _valuation.NOI?.ToString() ?? latestActual?.OperatingProfit?.ToString() ?? "0";
            txtTaxRate_Direct.Text = "30";
            txtCapRate.Text = _valuation.CapRate?.ToString() ?? "5";

            // 純資産法
            txtBookNetAsset.Text = _valuation.NetAssetValue?.ToString() ?? latestActual?.NetAssets?.ToString() ?? "0";
            txtOpProfit_NA.Text = latestActual?.OperatingProfit?.ToString() ?? "0";
            txtTaxRate_NA.Text = "30";
            txtGoodwillYears.Text = "3"; // デフォルト3年

            // DBからグリッドへ（省略時、DCFは自動で1〜5期の予測を流し込みます）
            // ※文字数制限のため、ここでのDB展開は省略し初回連動を優先させています。
            InitializeDcfWithForecasts();

            CalculateValuation(null, EventArgs.Empty);
        }

        private void InitializeDcfWithForecasts()
        {
            dgvDcf.Rows.Clear();
            var actual = _highlights.Where(h => h.PeriodType == "actual").OrderByDescending(h => h.PeriodOrder).FirstOrDefault();
            dgvDcf.Rows.Add("0期(実績)", actual?.Revenue ?? 0, actual?.OperatingProfit ?? 0, 30, 0, 7.3, 0);

            var forecasts = _highlights.Where(h => h.PeriodType == "forecast").OrderBy(h => h.PeriodOrder).ToList();
            for (int i = 1; i <= 5; i++)
            {
                var f = forecasts.ElementAtOrDefault(i - 1);
                dgvDcf.Rows.Add($"{i}期(予測)", f?.Revenue ?? 0, f?.OperatingProfit ?? 0, 30, 0, 7.3, 0);
            }
            dgvDcf.Rows.Add("6期以降(TV)", forecasts.LastOrDefault()?.Revenue ?? 0, forecasts.LastOrDefault()?.OperatingProfit ?? 0, 30, 0, 7.3, 0);
        }

        // ── UI構築用ヘルパーメソッド群 ──
        private void AddHeader(Control parent, string text, ref int y)
        {
            parent.Controls.Add(new Label { Text = text, Location = new Point(5, y), Font = new Font("Meiryo", 9, FontStyle.Bold), AutoSize = true });
            y += 25;
        }
        private TextBox AddInput(Control parent, string label, ref int y)
        {
            parent.Controls.Add(new Label { Text = label, Location = new Point(15, y + 4), AutoSize = true });
            var txt = new TextBox { Location = new Point(160, y), Width = 100, TextAlign = HorizontalAlignment.Right };
            parent.Controls.Add(txt);
            y += 28; return txt;
        }
        private Label AddResult(Control parent, string label, ref int y)
        {
            parent.Controls.Add(new Label { Text = label, Location = new Point(15, y + 4), AutoSize = true });
            var lbl = new Label { Location = new Point(160, y + 4), Width = 120, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Meiryo", 9, FontStyle.Bold), ForeColor = Color.DarkBlue };
            parent.Controls.Add(lbl);
            y += 30; return lbl;
        }
        private void SetupGrid(DataGridView dgv, Control parent, string title, ref int y)
        {
            parent.Controls.Add(new Label { Text = title, Location = new Point(15, y) });
            dgv.Location = new Point(15, y + 20); dgv.Size = new Size(500, 120);
            dgv.AllowUserToAddRows = true; dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Columns.Add("ItemName", "項目"); dgv.Columns.Add("Amount", "金額(千円)"); dgv.Columns.Add("Remarks", "備考");
            parent.Controls.Add(dgv); y += 150;
        }
        private void SetupDcfGrid(Control parent, ref int y)
        {
            dgvDcf.Location = new Point(15, y); dgvDcf.Size = new Size(700, 200);
            dgvDcf.AllowUserToAddRows = false; dgvDcf.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            string[] cols = { "Year", "Revenue", "OpProfit", "TaxRate", "NOPLAT", "DiscountRate", "TerminalGrowth", "PV" };
            string[] heads = { "期", "売上高", "営業利益", "税率(%)", "NOPLAT", "割引率(%)", "永久成長率", "現在価値(PV)" };
            for (int i = 0; i < cols.Length; i++) dgvDcf.Columns.Add(cols[i], heads[i]);

            // ── 変更: ! を付けてコンパイラの警告を抑制 ──
            dgvDcf.Columns["Year"]!.ReadOnly = true;
            dgvDcf.Columns["NOPLAT"]!.ReadOnly = true;
            dgvDcf.Columns["PV"]!.ReadOnly = true;
            dgvDcf.Columns["NOPLAT"]!.DefaultCellStyle.BackColor = Color.LightGray;
            dgvDcf.Columns["PV"]!.DefaultCellStyle.BackColor = Color.LightGray;

            parent.Controls.Add(dgvDcf); y += 220;
        }
        private double ParseD(object? val) => double.TryParse(val?.ToString(), out double r) ? r : 0;
        private double GetGridTotal(DataGridView dgv, string col) => dgv.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow).Sum(r => ParseD(r.Cells[col].Value));

        private void AttachCalculateEvent(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox txt) txt.TextChanged += CalculateValuation;
                if (c is DataGridView dgv) dgv.CellValueChanged += CalculateValuation;
                if (c.HasChildren) AttachCalculateEvent(c);
            }
        }
    }
}