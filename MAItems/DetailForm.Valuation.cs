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

        // ★追加: 財務データから再取得するためのボタン
        private Button btnReflectFinancial = new Button();

        // 各手法の入力・結果
        private TextBox txtBookNetAsset = new TextBox(), txtOpProfit_NA = new TextBox(), txtTaxRate_NA = new TextBox(), txtGoodwillYears = new TextBox();
        private Label lblMarketNetAsset = new Label(), lblGoodwill = new Label(), lblTotal_NA = new Label();

        private TextBox txtEBITDA_Calc = new TextBox(), txtEBITDAMultiple = new TextBox();
        private Label lblEV_EBITDA = new Label(), lblEquity_EBITDA = new Label();

        private Label lblEV_DCF = new Label(), lblEquity_DCF = new Label();
        private TextBox txtWacc = new TextBox();

        private TextBox txtOpProfit_Direct = new TextBox(), txtTaxRate_Direct = new TextBox(), txtCapRate = new TextBox();
        private Label lblEV_Direct = new Label(), lblEquity_Direct = new Label();

        /// <summary>
        /// バリュエーション用のUIをプログラムで動的に生成し、指定したパネルに流し込みます
        /// </summary>
        private void BuildValuationUI(Control container)
        {
            tabValuation.TabPages.Clear();
            var split = new SplitContainer { Dock = DockStyle.Fill, FixedPanel = FixedPanel.Panel2 };
            this.Load += (s, ev) => {
                // フォームのロードが完了し、実際のサイズが確定したタイミングで右側パネルを300pxに設定する
                if (split.Width > 300) split.SplitterDistance = split.Width - 300;
            };
            container.Controls.Add(split);

            // 💡 追加: DockStyle.Fill のコントロールを残りのスペースに正しく広げるための魔法の1行
            split.BringToFront();

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

            // ── ★【ここから追加】共通パネルの最上部に「再取得ボタン」を配置 ──
            btnReflectFinancial = new Button
            {
                Text = "🔄 財務データから最新を取得",
                Location = new Point(15, y),
                Size = new Size(250, 30),
                BackColor = Color.LightYellow,
                Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular)
            };
            btnReflectFinancial.Click += btnReflectFinancial_Click;
            pnlShared.Controls.Add(btnReflectFinancial);
            y += 40; // ボタンの高さと余白分、配置位置を下にずらす
            // ── ★【ここまで追加】──

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
            txtWacc = AddInput(tabDCF, "WACC (割引率 %)", ref y3);
            txtWacc.Text = "7.3"; // 初期値

            // WACCを変更したら、表内のすべての「割引率」を一括更新するイベント
            txtWacc.TextChanged += (s, e) => {
                double currentWacc = ParseD(txtWacc.Text);
                foreach (DataGridViewRow r in dgvDcf.Rows)
                {
                    if (!r.IsNewRow) r.Cells["DiscountRate"].Value = currentWacc;
                }
            };

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

        // ══════════════════════════════════════════════════════
        // ★追加: 財務データから再取得ボタンが押された時の処理
        // ══════════════════════════════════════════════════════
        private void btnReflectFinancial_Click(object? sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "財務データタブの最新の実績値で、現在のバリュエーション入力欄を上書きしますか？\n（※手入力で修正していた数値はリセットされます）",
                "再取得の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                ReflectFinancialData();
            }
        }

        // ══════════════════════════════════════════════════════
        // ★修正・最適化: 財務データから実績値を引っ張ってきて画面に反映
        // ══════════════════════════════════════════════════════
        private void ReflectFinancialData()
        {
            // すでにクラス内に読み込まれている _highlights リストを安全に使用します
            if (_highlights == null || _highlights.Count == 0)
            {
             //   MessageBox.Show("財務データがまだ登録されていないか、読み込めていません。", "通知", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
 
            // ── 再取得ボタンを押した時も、DCFの表を最新の予測データで作り直す ──
            InitializeDcfWithForecasts();

            // 直近の実績期（PeriodType が \"actual\" の中で、順序 Order が一番大きいもの）を取得
            var latestActual = _highlights
                .Where(h => h.PeriodType == "actual")
                .OrderByDescending(h => h.PeriodOrder)
                .FirstOrDefault() ?? _highlights.OrderByDescending(h => h.PeriodOrder).FirstOrDefault();

            if (latestActual != null)
            {
                // 1. 共通項目（右側パネル）への自動転記
                if (latestActual.CashEquivalents.HasValue)
                    txtCash.Text = latestActual.CashEquivalents.Value.ToString();

                if (latestActual.ShortTermDebt.HasValue)
                    txtShortDebt.Text = latestActual.ShortTermDebt.Value.ToString();

                if (latestActual.LongTermDebt.HasValue)
                    txtLongDebt.Text = latestActual.LongTermDebt.Value.ToString();

                // 2. 各手法の初期値への自動転記
                if (latestActual.EBITDA.HasValue)
                    txtEBITDA_Calc.Text = latestActual.EBITDA.Value.ToString();

                if (latestActual.NetAssets.HasValue)
                    txtBookNetAsset.Text = latestActual.NetAssets.Value.ToString();

                // 3. 値がセットされたら、即座にバリュエーションの総再計算処理を走らせる
                CalculateValuation(null, EventArgs.Empty);

                MessageBox.Show($"直近の実績データ（{latestActual.PeriodLabel}）を反映し、再計算しました！", "反映完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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

            lblWorkingCapital.Text = workingCapital.ToString("#,0") + " 百万円";
            lblNonOpAssets.Text = nonOpAssets.ToString("#,0") + " 百万円";
            lblTotalDebt.Text = debt.ToString("#,0") + " 百万円";

            // 2. 純資産法の計算
            double bookNet = ParseD(txtBookNetAsset.Text);
            double assetAdj = GetGridTotal(dgvAssetAdj, "Amount");
            double liabAdj = GetGridTotal(dgvLiabAdj, "Amount");
            double marketNet = bookNet + assetAdj - liabAdj;
            lblMarketNetAsset.Text = marketNet.ToString("#,0") + " 百万円";

            double noplat_NA = ParseD(txtOpProfit_NA.Text) * (1 - (ParseD(txtTaxRate_NA.Text) / 100.0));
            double goodwill = noplat_NA * ParseD(txtGoodwillYears.Text);
            lblGoodwill.Text = goodwill.ToString("#,0") + " 百万円";
            lblTotal_NA.Text = (marketNet + goodwill).ToString("#,0") + " 百万円";

            // 3. EBITDA法の計算 (※画像通り、非事業資産ではなく「現金」を足す)
            double ev_EBITDA = ParseD(txtEBITDA_Calc.Text) * ParseD(txtEBITDAMultiple.Text);
            lblEV_EBITDA.Text = ev_EBITDA.ToString("#,0") + " 百万円";
            lblEquity_EBITDA.Text = (ev_EBITDA + cash - debt).ToString("#,0") + " 百万円";

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
            lblEV_DCF.Text = ev_DCF.ToString("#,0") + " 百万円";
            lblEquity_DCF.Text = (ev_DCF + nonOpAssets - debt).ToString("#,0") + " 百万円";

            // 5. 直接還元法の計算
            double noplat_Direct = ParseD(txtOpProfit_Direct.Text) * (1 - (ParseD(txtTaxRate_Direct.Text) / 100.0));
            double capRate = ParseD(txtCapRate.Text) / 100.0;
            double ev_Direct = capRate > 0 ? noplat_Direct / capRate : 0;
            lblEV_Direct.Text = ev_Direct.ToString("#,0") + " 百万円";
            lblEquity_Direct.Text = (ev_Direct + nonOpAssets - debt).ToString("#,0") + " 百万円";

            // 6.数式マップを更新する
            UpdateFormulaFlow();
        }

        // ══════════════════════════════════════════════════════
        // 入力値と計算式の関係性をリアルタイムに可視化する
        // ══════════════════════════════════════════════════════
        private void UpdateFormulaFlow()
        {
            // デザイナーファイルで定義したコントロールがまだ無い場合はスキップ
            if (rtbFormulaFlow == null) return;

            // 1. 共通項目（右側パネル）の数値をパース
            double cash = ParseD(txtCash.Text);
            double wcMonths = ParseD(txtWCMonths.Text);
            double debt = ParseD(txtShortDebt.Text) + ParseD(txtLongDebt.Text) + ParseD(txtLease.Text) + ParseD(txtOtherDebt.Text);

            // 財務ハイライトから最新の実績売上高を取得して月商を算出
            double latestRev = _highlights.Where(h => h.PeriodType == "actual").OrderByDescending(h => h.PeriodOrder).FirstOrDefault()?.Revenue ?? 0;
            double workingCapital = (latestRev / 12.0) * wcMonths;
            double nonOpAssets = Math.Max(0, cash - workingCapital);

            // 2. 各手法の固有入力をパース
            double bookNet = ParseD(txtBookNetAsset.Text);
            double assetAdj = GetGridTotal(dgvAssetAdj, "Amount");
            double liabAdj = GetGridTotal(dgvLiabAdj, "Amount");
            double marketNet = bookNet + assetAdj - liabAdj;

            double opProfit_NA = ParseD(txtOpProfit_NA.Text);
            double taxRate_NA = ParseD(txtTaxRate_NA.Text);
            double noplat_NA = opProfit_NA * (1 - (taxRate_NA / 100.0));
            double goodwillYears = ParseD(txtGoodwillYears.Text);
            double goodwill = noplat_NA * goodwillYears;

            double ebitda = ParseD(txtEBITDA_Calc.Text);
            double multiple = ParseD(txtEBITDAMultiple.Text);
            double ev_EBITDA = ebitda * multiple;

            double ev_DCF = ParseD(lblEV_DCF.Text.Replace(" 百万円", "").Replace(",", ""));
            double ev_Direct = ParseD(lblEV_Direct.Text.Replace(" 百万円", "").Replace(",", ""));

            // 3. 表示テキストの組み立て
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("■──【 共通の調整項目 】──────────────────");
            sb.AppendLine("  [ロジック] 非事業資産 ＝ 現金預金 － (月商 × 運転資金月数)");
            sb.AppendLine($"  [計 算] {cash:N0} － ({latestRev / 12.0:N0} × {wcMonths}ヶ月) ＝ ➔ 非事業資産: {nonOpAssets:N0} 百万円");
            sb.AppendLine($"  [負 債] 有利子負債合計 ＝ ➔ {debt:N0} 百万円");
            sb.AppendLine();

            sb.AppendLine("■──【 ① 純資産法（時価純資産＋営業権）】───────");
            sb.AppendLine("  [ロジック]:簿価純資産に時価修正を行い、税引後営業利益（NOPLAT）の計上年数分をのれんとして加算する。");
            sb.AppendLine("    marketNet (時価純資産) ＝ 簿価純資産 ＋ 資産修正 － 負債修正");
            sb.AppendLine("    goodwill  (のれん)   ＝ 営業利益 × (1 － 税率) × 計上年数");
            sb.AppendLine("    株式価値             ＝ 時価純資産 ＋ のれん");
            sb.AppendLine("  [実際の計算]");
            sb.AppendLine($"    時価純資産 : {bookNet:N0} ＋ {assetAdj:N0} － {liabAdj:N0} ＝ {marketNet:N0} 百万円");
            sb.AppendLine($"    のれん代   : {opProfit_NA:N0} × (1 － {taxRate_NA}% ) × {goodwillYears}年 ＝ {goodwill:N0} 百万円");
            sb.AppendLine($"    ➔ 株式価値 ＝ {marketNet:N0} ＋ {goodwill:N0} ＝ 【 {lblTotal_NA.Text} 】");
            sb.AppendLine();

            sb.AppendLine("■──【 ② EBITDA法（マルチプル）】─────────");
            sb.AppendLine("  [ロジック]:EBITDAにマルチプル（倍率）を掛け合わせて事業価値（EV）を算出し、そこから有利子負債を差し引き、現金同等物を加算する");
            sb.AppendLine("    事業価値 (EV) ＝ EBITDA × マルチプル倍率");
            sb.AppendLine("    株式価値      ＝ 事業価値 ＋ 現金預金 － 有利子負債");
            sb.AppendLine("  [実際の計算]");
            sb.AppendLine($"    事業価値 (EV) : {ebitda:N0} × {multiple}倍 ＝ {ev_EBITDA:N0} 百万円");
            sb.AppendLine($"    ➔ 株式価値 ＝ {ev_EBITDA:N0} ＋ {cash:N0} － {debt:N0} ＝ 【 {lblEquity_EBITDA.Text} 】");
            sb.AppendLine();

            sb.AppendLine("■──【 ③ DCF法 】─────────────────────────");
            sb.AppendLine("  [ロジック]:将来5年間の損益予測（減価償却＝設備投資、運転資本増減なしとしてNOPLAT＝FCFとする）をWACCで割り引き、" );
            sb.AppendLine("    1. 各期の予測FCF（＝税引後営業利益）をWACCで現在価値(PV)に割引");
            sb.AppendLine("    2. 6期以降の継続価値(TV) ＝ 6期NOPLAT ÷ (WACC － 永久成長率)");
            sb.AppendLine("    3. 事業価値 (EV) ＝ 1〜5期PV合計 ＋ TVの現在価値(PV)");
            sb.AppendLine("    4. 株式価値      ＝ 事業価値 ＋ 非事業資産 － 有利子負債");
            sb.AppendLine("  [実際の計算フロー]");

            // データグリッド（dgvDcf）から各期間のPVとTVを直接集計して正確に可視化
            double pvSum1to5 = 0;
            double tvRaw = 0;
            double tvPv = 0;

            foreach (DataGridViewRow r in dgvDcf.Rows)
            {
                if (r.IsNewRow) continue;
                string? yearText = r.Cells["Year"].Value?.ToString();
                double pv = ParseD(r.Cells["PV"].Value);

                if (yearText != null && (yearText.Contains("1期") || yearText.Contains("2期") || yearText.Contains("3期") || yearText.Contains("4期") || yearText.Contains("5期")))
                {
                    pvSum1to5 += pv;
                }
                else if (yearText != null && yearText.Contains("6期以降"))
                {
                    tvPv = pv;
                    // TV本体の額を逆算（ NOPLAT / (WACC - Growth) ）
                    double op = ParseD(r.Cells["OpProfit"].Value);
                    double tax = ParseD(r.Cells["TaxRate"].Value) / 100.0;
                    double rate = ParseD(r.Cells["DiscountRate"].Value) / 100.0;
                    double growth = ParseD(r.Cells["TerminalGrowth"].Value) / 100.0;
                    if (rate - growth > 0)
                    {
                        tvRaw = (op * (1 - tax)) / (rate - growth);
                    }
                }
            }

            sb.AppendLine($"    ① 1〜5期 予測FCFの現在価値合計 : {pvSum1to5:N0} 百万円");
            sb.AppendLine($"    ② 6期以降の継続価値(TV)本体    : {tvRaw:N0} 百万円");
            sb.AppendLine($"       ➔ TVを5期末時点で現在価値化 : {tvPv:N0} 百万円");
            sb.AppendLine($"    ③ 事業価値 (EV) [① ＋ ②のPV]  : {pvSum1to5:N0} ＋ {tvPv:N0} ＝ {ev_DCF:N0} 百万円");
            sb.AppendLine($"    ④ 株式価値の算定：");
            sb.AppendLine($"       事業価値 ({ev_DCF:N0}) ＋ 非事業資産 ({nonOpAssets:N0}) － 有利子負債 ({debt:N0})");
            sb.AppendLine($"       ➔ 株式価値 ＝ 【 {lblEquity_DCF.Text} 】");
            sb.AppendLine();

            sb.AppendLine("■──【 ④ 直接還元法 】───────────────────");
            sb.AppendLine("  [ロジック]標準営業利益からNOPLATを求め、還元利回りで割って事業価値を算定。そこに非事業資産を加え、有利子負債を差し引く。");
            sb.AppendLine("    事業価値 (EV) ＝ { 営業利益 × (1 － 税率) } ÷ 還元利回り");
            sb.AppendLine("    株式価値      ＝ 事業価値 ＋ 非事業資産 － 有利子負債");
            sb.AppendLine("  [実際の計算]");
            sb.AppendLine($"    事業価値 (EV) : {{ {ParseD(txtOpProfit_Direct.Text):N0} × (1 － {ParseD(txtTaxRate_Direct.Text)}%) }} ÷ {ParseD(txtCapRate.Text) / 100.0:P1} ＝ {ev_Direct:N0} 百万円");
            sb.AppendLine($"    ➔ 株式価値 ＝ {ev_Direct:N0} ＋ {nonOpAssets:N0} － {debt:N0} ＝ 【 {lblEquity_Direct.Text} 】");

            rtbFormulaFlow.Text = sb.ToString();
        }

        private void HighlightKeywords()
        {
            // 必要に応じて「株式価値」や「【 】」の文字色を太字・青色に変える処理
        }

        private void InitializeDcfWithForecasts()
        {
            dgvDcf.Rows.Clear();
            double wacc = ParseD(txtWacc.Text);

            // 0期（実績）のデータを取得
            var actual = _highlights.Where(h => h.PeriodType == "actual").OrderByDescending(h => h.PeriodOrder).FirstOrDefault();

            // ベースとなる初期値（実績がなければ0）
            double currentRev = actual?.Revenue ?? 0;
            double currentOp = actual?.OperatingProfit ?? 0;

            dgvDcf.Rows.Add("0期(実績)", currentRev, currentOp, 30, 0, wacc, 0);

            var forecasts = _highlights.Where(h => h.PeriodType == "forecast").OrderBy(h => h.PeriodOrder).ToList();

            // 1期〜5期の予測行を生成
            for (int i = 1; i <= 5; i++)
            {
                var f = forecasts.ElementAtOrDefault(i - 1);

                // 💡 予測データが存在すればそれを使い、無ければ「前年のデータ（currentRev/Op）」をそのまま引き継ぐ（横置き）
                if (f != null && (f.Revenue.HasValue || f.OperatingProfit.HasValue))
                {
                    currentRev = f.Revenue ?? currentRev;
                    currentOp = f.OperatingProfit ?? currentOp;
                }

                dgvDcf.Rows.Add($"{i}期(予測)", currentRev, currentOp, 30, 0, wacc, 0);
            }

            // 6期以降（ターミナルバリュー）は、最終的にもつれ込んだ5期目の値を引き継ぐ
            dgvDcf.Rows.Add("6期以降(TV)", currentRev, currentOp, 30, 0, wacc, 0);
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
            parent.Controls.Add(new Label { Text = title, Location = new Point(15, y), AutoSize = true });

            dgv.Location = new Point(15, y + 20);
            // 💡 修正: 幅を500の固定値ではなく、左側の画面幅に合わせて可変にする
            dgv.Size = new Size(Math.Max(400, parent.ClientSize.Width - 40), 120);
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; // 画面幅に合わせて伸縮

            dgv.AllowUserToAddRows = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // 3列しかないので枠いっぱいに広げる

            dgv.Columns.Clear();
            dgv.Columns.Add("ItemName", "項目");
            dgv.Columns.Add("Amount", "金額(百万円)");
            dgv.Columns.Add("Remarks", "備考");

            // 💡 追加: 各列のバランス調整とフォーマット設定
            dgv.Columns["ItemName"]!.FillWeight = 30; // 項目名はそこそこの幅

            dgv.Columns["Amount"]!.FillWeight = 20;   // 金額は少し狭めに
            dgv.Columns["Amount"]!.DefaultCellStyle.Format = "N0"; // 金額にカンマ区切りを入れる
            dgv.Columns["Amount"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; // 金額を右寄せ

            dgv.Columns["Remarks"]!.FillWeight = 50;  // 備考欄を一番広くする（残りスペースを独占）

            parent.Controls.Add(dgv);
            y += 150;
        }
        private void SetupDcfGrid(Control parent, ref int y)
        {
            {
                if (parent is TabPage tabPage)
                {
                    tabPage.AutoScroll = true;
                }

                dgvDcf.Location = new Point(15, y);
                // 💡 幅を700の固定値ではなく、左側の画面幅に自動で合わせる
                dgvDcf.Size = new Size(Math.Max(500, parent.ClientSize.Width - 40), 200);
                dgvDcf.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; // 画面幅に合わせて伸縮
                dgvDcf.AllowUserToAddRows = false;

                // 💡 Fill(無理やり押し込む)を解除し、内容に合わせて横スクロールバーを出す
                dgvDcf.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvDcf.ScrollBars = ScrollBars.Both;

                string[] cols = { "Year", "Revenue", "OpProfit", "TaxRate", "NOPLAT", "DiscountRate", "TerminalGrowth", "PV" };
                string[] heads = { "期", "売上高", "営業利益", "税率(%)", "NOPLAT", "割引率(%)", "永久成長率", "現在価値(PV)" };

                dgvDcf.Columns.Clear();
                for (int i = 0; i < cols.Length; i++)
                {
                    dgvDcf.Columns.Add(cols[i], heads[i]);
                    // 列の幅を「90px」など少し広めに確保（文字が見切れないように）
                   dgvDcf.Columns[cols[i]]!.Width = 90;
                }

                dgvDcf.Columns["Year"]!.ReadOnly = true;
                dgvDcf.Columns["NOPLAT"]!.ReadOnly = true;
                dgvDcf.Columns["PV"]!.ReadOnly = true;
                dgvDcf.Columns["NOPLAT"]!.DefaultCellStyle.BackColor = Color.LightGray;
                dgvDcf.Columns["PV"]!.DefaultCellStyle.BackColor = Color.LightGray;

                // PV（現在価値）等が見やすくなるようカンマ区切りフォーマットを追加
                dgvDcf.Columns["PV"]!.DefaultCellStyle.Format = "N0";
                dgvDcf.Columns["Revenue"]!.DefaultCellStyle.Format = "N0";
                dgvDcf.Columns["OpProfit"]!.DefaultCellStyle.Format = "N0";
                dgvDcf.Columns["NOPLAT"]!.DefaultCellStyle.Format = "N0";

                parent.Controls.Add(dgvDcf);
                y += 220;
            }
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

        // ══════════════════════════════════════════════════════
        // 株式価値試算データの保存 (正規データベースカラム完全マッピング版)
        // ══════════════════════════════════════════════════════
        // ══════════════════════════════════════════════════════
        // 株式価値試算データの保存 (UI連携の漏れをすべて解消版)
        // ══════════════════════════════════════════════════════
        private void SaveValuationData()
        {
            var v = new ValuationData
            {
                DealId = _deal.Id,

                // 共通項目 (右側パネル)
                CashAndDeposits = ParseNullableDouble(txtCash.Text),
                WorkingCapitalMonths = ParseNullableDouble(txtWCMonths.Text),
                ShortTermDebt = ParseNullableDouble(txtShortDebt.Text),
                LongTermDebt = ParseNullableDouble(txtLongDebt.Text),
                LeaseObligations = ParseNullableDouble(txtLease.Text),
                OtherLiabilities = ParseNullableDouble(txtOtherDebt.Text),

                // ① 純資産法 (★不足していた項目を追加)
                NetAssetValue = ParseNullableDouble(txtBookNetAsset.Text),
                OpProfit_NA = ParseNullableDouble(txtOpProfit_NA.Text),
                TaxRate_NA = ParseNullableDouble(txtTaxRate_NA.Text),
                GoodwillYears = ParseNullableDouble(txtGoodwillYears.Text),

                // ② EBITDA法
                EBITDABase = ParseNullableDouble(txtEBITDA_Calc.Text),
                EBITDAMultiple = ParseNullableDouble(txtEBITDAMultiple.Text),

                // ④ 直接還元法 (★不足していた項目を追加)
                OpProfit_Direct = ParseNullableDouble(txtOpProfit_Direct.Text),
                TaxRate_Direct = ParseNullableDouble(txtTaxRate_Direct.Text),
                CapRate = ParseNullableDouble(txtCapRate.Text)
            };

            var finRepo = new FinancialRepository(_context);
            finRepo.UpsertValuationData(v);

            // ① 純資産法の修正項目グリッドの保存
            finRepo.DeleteNetAssetAdjustments(_deal.Id);
            foreach (DataGridViewRow r in dgvAssetAdj.Rows)
            {
                if (r.IsNewRow) continue;
                finRepo.AddNetAssetAdjustment(new NetAssetAdjustment { DealId = _deal.Id, AdjustType = 1, ItemName = r.Cells["ItemName"].Value?.ToString() ?? "", Amount = ParseD(r.Cells["Amount"].Value), Remarks = r.Cells["Remarks"].Value?.ToString() ?? "" });
            }
            foreach (DataGridViewRow r in dgvLiabAdj.Rows)
            {
                if (r.IsNewRow) continue;
                finRepo.AddNetAssetAdjustment(new NetAssetAdjustment { DealId = _deal.Id, AdjustType = 2, ItemName = r.Cells["ItemName"].Value?.ToString() ?? "", Amount = ParseD(r.Cells["Amount"].Value), Remarks = r.Cells["Remarks"].Value?.ToString() ?? "" });
            }

            // ③ DCF法の事業計画グリッドの保存
            finRepo.DeleteDcfProjections(_deal.Id);
            foreach (DataGridViewRow r in dgvDcf.Rows)
            {
                if (r.IsNewRow) continue;
                finRepo.AddDcfProjection(new DcfProjection { DealId = _deal.Id, YearIndex = r.Index, Revenue = ParseNullableDouble(r.Cells["Revenue"].Value?.ToString()), OpProfit = ParseNullableDouble(r.Cells["OpProfit"].Value?.ToString()), TaxRate = ParseNullableDouble(r.Cells["TaxRate"].Value?.ToString()), DiscountRate = ParseNullableDouble(r.Cells["DiscountRate"].Value?.ToString()), TerminalGrowth = ParseNullableDouble(r.Cells["TerminalGrowth"].Value?.ToString()) });
            }
        }

        // ══════════════════════════════════════════════════════
        // 株式価値試算データの読み込み (UI連携の漏れをすべて解消版)
        // ══════════════════════════════════════════════════════
        private void LoadValuationData()
        {
            var finRepo = new FinancialRepository(_context);
            var v = finRepo.GetValuationData(_deal.Id);

            InitializeDcfWithForecasts();

            if (v != null)
            {
                // 右側共通パネルの復元
                txtCash.Text = v.CashAndDeposits?.ToString();
                txtWCMonths.Text = v.WorkingCapitalMonths?.ToString();
                txtShortDebt.Text = v.ShortTermDebt?.ToString();
                txtLongDebt.Text = v.LongTermDebt?.ToString();
                txtLease.Text = v.LeaseObligations?.ToString();
                txtOtherDebt.Text = v.OtherLiabilities?.ToString();

                // ① 純資産法の復元 (★不足していた項目を追加)
                txtBookNetAsset.Text = v.NetAssetValue?.ToString();
                txtOpProfit_NA.Text = v.OpProfit_NA?.ToString();
                txtTaxRate_NA.Text = v.TaxRate_NA?.ToString();
                txtGoodwillYears.Text = v.GoodwillYears?.ToString();

                // ② EBITDA法の復元
                txtEBITDA_Calc.Text = v.EBITDABase?.ToString();
                txtEBITDAMultiple.Text = v.EBITDAMultiple?.ToString();

                // ④ 直接還元法の復元 (★不足していた項目を追加)
                txtCapRate.Text = v.CapRate?.ToString();
                txtOpProfit_Direct.Text = v.OpProfit_Direct?.ToString();
                txtTaxRate_Direct.Text = v.TaxRate_Direct?.ToString();

                // ① グリッドの復元
                var adjList = finRepo.GetNetAssetAdjustments(_deal.Id);
                dgvAssetAdj.Rows.Clear();
                dgvLiabAdj.Rows.Clear();
                foreach (var adj in adjList)
                {
                    if (adj.AdjustType == 1) dgvAssetAdj.Rows.Add(adj.ItemName, adj.Amount, adj.Remarks);
                    else if (adj.AdjustType == 2) dgvLiabAdj.Rows.Add(adj.ItemName, adj.Amount, adj.Remarks);
                }

                // ③ グリッドの復元
                var dcfList = finRepo.GetDcfProjections(_deal.Id);
                var savedWacc = dcfList.FirstOrDefault(p => p.DiscountRate.HasValue)?.DiscountRate;
                if (savedWacc.HasValue)
                {
                    txtWacc.Text = savedWacc.Value.ToString();
                }
                foreach (var proj in dcfList)
                {
                    if (proj.YearIndex >= 0 && proj.YearIndex < dgvDcf.Rows.Count)
                    {
                        var row = dgvDcf.Rows[proj.YearIndex];
                        if (proj.Revenue.HasValue) row.Cells["Revenue"].Value = proj.Revenue.Value;
                        if (proj.OpProfit.HasValue) row.Cells["OpProfit"].Value = proj.OpProfit.Value;
                        if (proj.TaxRate.HasValue) row.Cells["TaxRate"].Value = proj.TaxRate.Value;
                        if (proj.DiscountRate.HasValue) row.Cells["DiscountRate"].Value = proj.DiscountRate.Value;
                        if (proj.TerminalGrowth.HasValue) row.Cells["TerminalGrowth"].Value = proj.TerminalGrowth.Value;
                    }
                }

                CalculateValuation(null, EventArgs.Empty);
                return;
            }

            ReflectFinancialData();
        }
        // 文字列から安全に Nullable double へ変換するヘルパー
        private double? ParseNullableDouble(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            if (double.TryParse(text.Replace(",", ""), out double r)) return r;
            return null;
        }

    }
}