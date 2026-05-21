using MAItems.Database;
using MAItems.MailParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MAItems
{
    public partial class DetailForm : Form
    {
        #region メンバ変数
        private readonly DealRepository _dealRepo;
        private readonly FinancialRepository _financialRepo;
        private readonly AttachmentRepository _attachmentRepo;
        private Deal _deal;
        private DatabaseContext _context;

        // 各タブで管理する拡張データ
        private CompanyProfile _profile = new();
        private List<FinancialHighlight> _highlights = new();
        private ValuationData _valuation = new();
        private List<Attachment> _attachments = new();

        // 保存完了を親フォーム(MainForm)に通知するためのイベント
        public event EventHandler? SaveCompleted;

        #endregion

        #region コンストラクタ・初期化

        public DetailForm(Deal deal, DatabaseContext context)
        {
            InitializeComponent();
            BuildTab1();
            BuildTab2();
            BuildTab3(); 
            BuildTab4();
            BuildTab5();



            _deal = deal;
            _context = context;

            // それぞれの専門リポジトリを生成
            _dealRepo = new DealRepository(context);
            _financialRepo = new FinancialRepository(context);
            _attachmentRepo = new AttachmentRepository(context);

            BuildFinancialGrid();
            LoadAll();
        }

        /// <summary>
        /// 全タブのデータを読み込み、画面に反映させます。
        /// </summary>
        private void LoadAll()
        {
            UpdateNavigationButtons();
            LoadTab1();
            LoadTab2();
            LoadTab3();

            // 保存されたデータの展開と、財務ハイライトからの自動計算を実行
            LoadValuationData();
//            LoadTab4();
            LoadTab5();
        }

        #endregion

        #region ナビゲーション (前へ・次へ)

        /// <summary>
        /// 現在の案件が先頭・末尾かどうかに応じて、前へ・次へボタンの有効/無効を切り替えます。
        /// </summary>
        private void UpdateNavigationButtons()
        {
            var allDeals = _dealRepo.GetAllDeals();
            int idx = allDeals.FindIndex(d => d.Id == _deal.Id);

            btnPrev.Enabled = (idx > 0);

            btnNext.Enabled = true;
        }

        private void btnPrev_Click(object? sender, EventArgs e)
        {
            NavigateTo(-1); // 1つ前のレコードへ
        }

        private void btnNext_Click(object? sender, EventArgs e)
        {
            NavigateTo(1);  // 1つ次のレコードへ
        }

        /// <summary>
        /// 指定した方向のレコードへ移動します。
        /// 移動前に現在の編集内容を自動的に保存します。
        /// </summary>
        private void NavigateTo(int direction)
        {
            // 移動する前に、現在の入力内容を自動保存する
            if (!SaveCurrentData())
            {
                // 保存エラー時は移動をキャンセル
                return;
            }

            var allDeals = _dealRepo.GetAllDeals();
            int idx = allDeals.FindIndex(d => d.Id == _deal.Id);
            if (idx == -1) return;

            int newIdx = idx + direction;

            if (newIdx >= 0 && newIdx < allDeals.Count)
            {
                // 新しい案件に入れ替えて全体を再読み込み
                _deal = allDeals[newIdx];
                LoadAll();
                SetStatus($"✔ 変更を保存し、案件 ID: {_deal.Id} を読み込みました", isError: false);
            }
            else if (newIdx == allDeals.Count)
            {
                // 最後のレコードからさらに「次へ」移動しようとした場合
                var confirm = MessageBox.Show(
                    "最後の案件です。新しく案件を追加しますか？",
                    "新規追加の確認",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    // データベースに空の新規案件を追加してIDを取得
                    long newId = _dealRepo.AddEmptyDeal();

                    // 追加した新しい案件データを取得して入れ替える
                    var newDeal = _dealRepo.GetAllDeals().Find(d => d.Id == newId);
                    if (newDeal != null)
                    {
                        _deal = newDeal;
                        LoadAll();
                        SetStatus($"✔ 新規案件（ID: {_deal.Id}）を追加しました", isError: false);
                    }
                }
            }
        }

        #endregion

        #region 保存処理

        /// <summary>
        /// 現在のフォームの内容をデータベースに保存します。
        /// </summary>
        /// <returns>保存に成功した場合は true</returns>
        private bool SaveCurrentData()
        {
            try
            {
                // Tab1, Tab5: 基本情報と添付資料の全体備考
                FormToDeal();
                FormToAttachments();
                _dealRepo.UpdateDeal(_deal);

                // Tab2: 会社基礎情報
                FormToProfile();
                _financialRepo.UpsertCompanyProfile(_profile);

                // Tab3: 財務ハイライト
                var highlights = GridToHighlights();
                foreach (var hl in highlights)
                {
                    _financialRepo.UpsertFinancialHighlight(hl);
                }

                // Tab4: 株式価値試算
                SaveValuationData();

                // 親フォームへ通知
                SaveCompleted?.Invoke(this, EventArgs.Empty);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 「保存して閉じる」ボタンのクリックイベント
        /// </summary>
        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (SaveCurrentData())
            {
                SetStatus("✔ 保存しました", isError: false);
                this.Close(); // 保存成功時のみフォームを閉じる
            }
        }


        #endregion

        #region Tab 1: 基本情報

        /// <summary>
        /// DBデータをTab1のテキストボックスに反映します。
        /// </summary>
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

        /// <summary>
        /// Tab1のテキストボックスの値をDBモデル(_deal)に反映します。
        /// </summary>
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

        #endregion

        #region Tab 2: 会社基礎情報

        /// <summary>
        /// DBデータをTab2のテキストボックスに反映します。
        /// </summary>
        private void LoadTab2()
        {
            var p = _financialRepo.GetCompanyProfile(_deal.Id) ?? new CompanyProfile { DealId = _deal.Id };
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

        /// <summary>
        /// Tab2のテキストボックスの値をDBモデル(_profile)に反映します。
        /// </summary>
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

        #endregion

        #region Tab 3: 財務ハイライト

        /// <summary>
        /// 財務ハイライト用の DataGridView の列・行を構築します。
        /// </summary>
        private void BuildFinancialGrid()
        {
                dgvFinancial.AllowUserToAddRows = false;
                dgvFinancial.ReadOnly = false;
                dgvFinancial.RowHeadersVisible = true;
                dgvFinancial.ColumnHeadersVisible = true;
                dgvFinancial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvFinancial.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dgvFinancial.ScrollBars = ScrollBars.Both;
                dgvFinancial.Columns.Clear();

                // 列の定義
                dgvFinancial.Columns.Add(new DataGridViewTextBoxColumn { Name = "Item", HeaderText = "項目（百万円）", Width = 160, ReadOnly = true, Frozen = true });

                string[] pt = { "actual", "actual", "actual", "forecast", "forecast", "forecast" };
                string[] pl = { "実績1期", "実績2期", "実績3期", "予測1期", "予測2期", "予測3期" };

                for (int i = 0; i < 6; i++)
                {
                    dgvFinancial.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = $"col_{pt[i]}_{i % 3 + 1}",
                        HeaderText = pl[i],
                        Width = 110,
                        // ★修正1: 明示的に new して安全にスタイルを設定
                        DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                    });
                }

                // 行の定義
                var rows = new (string label, string field, bool isSection)[] {
                    ("── PL ──────", "", true),
                    ("売上高", "Revenue", false), ("原価率(%)", "CostRate", false), ("粗利益", "GrossProfit", false), ("粗利率(%)", "GrossProfitRate", false),
                    ("販管費", "SGA", false), ("営業利益", "OperatingProfit", false), ("営業利益率(%)", "OperatingProfitRate", false),
                    ("経常利益", "OrdinaryProfit", false), ("当期純利益", "NetIncome", false), ("EBITDA", "EBITDA", false),
                    ("減価償却費", "Depreciation", false), ("設備投資額", "CapEx", false),

                    ("── BS（資産） ──", "", true),
                    ("流動資産", "CurrentAssets", false), ("　現金預金", "CashEquivalents", false), ("　売掛金", "AccountsReceivable", false),
                    ("　棚卸資産", "Inventory", false), ("　その他流動", "OtherCurrentAssets", false), ("固定資産", "FixedAssets", false), ("総資産", "TotalAssets", false),

                    ("── BS（負債） ──", "", true),
                    ("流動負債", "CurrentLiabilities", false), ("　買掛金", "AccountsPayable", false), ("　短期借入金", "ShortTermDebt", false),
                    ("　その他流動", "OtherCurrentLiabilities", false), ("固定負債", "FixedLiabilities", false), ("　長期借入金", "LongTermDebt", false),
                    ("　その他固定", "OtherFixedLiabilities", false), ("負債合計", "TotalLiabilities", false), ("純資産合計", "NetAssets", false),
                    ("　利益剰余金", "RetainedEarnings", false)
                };

                dgvFinancial.Rows.Clear();

                // ★修正2: dgvFinancial.Font が null になった場合でも落ちないようにする安全対策
                var safeFont = dgvFinancial.Font ?? this.Font ?? SystemFonts.DefaultFont;
                var boldFont = new Font(safeFont, FontStyle.Bold);

                foreach (var (label, field, isSection) in rows)
                {
                    int idx = dgvFinancial.Rows.Add();
                    var row = dgvFinancial.Rows[idx];

                    // ★修正3: 文字列検索("Item")ではなく、確実にインデックス[0]を指定してエラーを回避
                    row.Cells[0].Value = label;
                    row.Tag = field;

                    // セクション区切り行のデザイン設定
                    if (isSection)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightSlateGray;
                        row.DefaultCellStyle.ForeColor = Color.White;
                        row.DefaultCellStyle.Font = boldFont;
                        row.ReadOnly = true;
                    }
                }
        }

        private void LoadTab3()
        {
            _highlights = _financialRepo.GetFinancialHighlights(_deal.Id);

            string[] colNames = { "col_actual_1", "col_actual_2", "col_actual_3", "col_forecast_1", "col_forecast_2", "col_forecast_3" };
            string[] periodTypes = { "actual", "actual", "actual", "forecast", "forecast", "forecast" };
            int[] periodOrders = { 1, 2, 3, 1, 2, 3 };

            // カラムヘッダー（期ラベル）の復元
            for (int c = 0; c < 6; c++)
            {
                var hl = _highlights.Find(h => h.PeriodType == periodTypes[c] && h.PeriodOrder == periodOrders[c]);
                if (hl != null && !string.IsNullOrEmpty(hl.PeriodLabel))
                {
                    var targetColumn = dgvFinancial.Columns[colNames[c]];
                    if (targetColumn != null)
                    {
                        targetColumn.HeaderText = hl.PeriodLabel;
                    }
                }
            }

            // 各セルのデータ復元
            foreach (DataGridViewRow row in dgvFinancial.Rows)
            {
                string field = row.Tag as string ?? string.Empty;
                if (string.IsNullOrEmpty(field)) continue;

                for (int c = 0; c < 6; c++)
                {
                    var hl = _highlights.Find(h => h.PeriodType == periodTypes[c] && h.PeriodOrder == periodOrders[c]);
                    if (hl == null) continue;

                    double? val = GetHighlightField(hl, field);
                    row.Cells[colNames[c]].Value = val.HasValue ? (object)val.Value : DBNull.Value;
                }
            }
        }

        private List<FinancialHighlight> GridToHighlights()
        {
            string[] colNames = { "col_actual_1", "col_actual_2", "col_actual_3", "col_forecast_1", "col_forecast_2", "col_forecast_3" };
            string[] periodTypes = { "actual", "actual", "actual", "forecast", "forecast", "forecast" };
            int[] periodOrders = { 1, 2, 3, 1, 2, 3 };

            var result = new List<FinancialHighlight>();

            for (int c = 0; c < 6; c++)
            {
                var hl = new FinancialHighlight
                {
                    DealId = _deal.Id,
                    PeriodType = periodTypes[c],
                    PeriodOrder = periodOrders[c],
                    PeriodLabel = dgvFinancial.Columns[colNames[c]]?.HeaderText ?? string.Empty
                };

                foreach (DataGridViewRow row in dgvFinancial.Rows)
                {
                    string field = row.Tag as string ?? string.Empty;
                    if (string.IsNullOrEmpty(field)) continue;

                    var cell = row.Cells[colNames[c]];
                    double? val = null;

                    if (cell.Value != null && cell.Value != DBNull.Value && double.TryParse(cell.Value.ToString(), out double d))
                    {
                        val = d;
                    }

                    SetHighlightField(hl, field, val);
                }
                result.Add(hl);
            }
            return result;
        }

        // --- フィールドマッピング用ヘルパー ---
        private static double? GetHighlightField(FinancialHighlight h, string field)
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
                _ => null
            };
        }

        private static void SetHighlightField(FinancialHighlight h, string field, double? val)
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

        #endregion



        #region Tab 4: 株式価値試算






        #endregion


        #region Tab 5: 添付資料

        /// <summary>
        /// 案件に紐づくファイル一覧と全体備考を読み込みます。
        /// </summary>
        private void LoadTab5()
        {
            txtAttachmentsSummary.Text = _deal.AttachmentsSummary;
            _attachments = _attachmentRepo.GetAttachments(_deal.Id);
            dgvAttachments.DataSource = new BindingList<Attachment>(_attachments);

            // イベントの多重登録を防ぐために一度外す
            btnAddFile.Click -= btnAddFile_Click;
            btnOpenFile.Click -= btnOpenFile_Click;
            dgvAttachments.CellDoubleClick -= btnOpenFile_Click;
            btnDeleteFile.Click -= btnDeleteFile_Click;

            // イベント登録
            btnAddFile.Click += btnAddFile_Click;
            btnOpenFile.Click += btnOpenFile_Click;
            dgvAttachments.CellDoubleClick += btnOpenFile_Click;
            btnDeleteFile.Click += btnDeleteFile_Click;
        }

        private void FormToAttachments()
        {
            _deal.AttachmentsSummary = txtAttachmentsSummary.Text;

            foreach (var att in _attachments)
            {
                // グリッド内で編集した Description（ファイル備考）を保存
                _attachmentRepo.SaveAttachment(att);
            }
        }

        /// <summary>
        /// [ファイル追加] ボタンの処理
        /// ファイルを選択し、案件専用フォルダにコピーします。
        /// </summary>
        private void btnAddFile_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog { Title = "保管するファイルを選択", Multiselect = true };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            // 保存先ディレクトリ: /Attachments/{DealId}/
            string destDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments", _deal.Id.ToString());
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            foreach (string file in dlg.FileNames)
            {
                string fileName = Path.GetFileName(file);
                string destPath = Path.Combine(destDir, fileName);

                try
                {
                    File.Copy(file, destPath, overwrite: true);
                    var att = new Attachment { DealId = _deal.Id, FileName = fileName, FilePath = destPath };
                    _attachmentRepo.SaveAttachment(att);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ファイル '{fileName}' のコピーに失敗しました: {ex.Message}");
                }
            }

            LoadTab5(); // グリッドを再読込
            SetStatus($"✔ {dlg.FileNames.Length}件のファイルを追加しました", isError: false);
        }

        /// <summary>
        /// [開く] ボタン または ダブルクリック の処理
        /// </summary>
        private void btnOpenFile_Click(object? sender, EventArgs e)
        {
            if (dgvAttachments.CurrentRow?.DataBoundItem is Attachment att)
            {
                if (File.Exists(att.FilePath))
                {
                    // 関連付けられた既定のアプリで開く
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = att.FilePath, UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("ファイルが見つかりません。移動または削除された可能性があります。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// [削除] ボタンの処理
        /// </summary>
        private void btnDeleteFile_Click(object? sender, EventArgs e)
        {
            if (dgvAttachments.CurrentRow?.DataBoundItem is Attachment att)
            {
                var result = MessageBox.Show($"'{att.FileName}' を削除しますか？\n（PC上の実ファイルも削除されます）", "削除の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        if (File.Exists(att.FilePath))
                        {
                            File.Delete(att.FilePath);
                        }
                        _attachmentRepo.DeleteAttachment(att.Id);

                        LoadTab5(); // グリッドを再読込
                        SetStatus($"✔ ファイル '{att.FileName}' を削除しました", isError: false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ファイルの削除に失敗しました: " + ex.Message);
                    }
                }
            }
        }

        #endregion

        #region メールからの取込

        /// <summary>
        /// クリップボードのメールテキストから案件情報を抽出し、フォームに自動入力します。
        /// </summary>
        private void btnPasteFromMail_Click(object? sender, EventArgs e)
        {
            string mailBody = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(mailBody)) { SetStatus("⚠ クリップボードにテキストがありません", isError: true); return; }

            var parser = MailParserFactory.GetParser(mailBody);
            if (parser == null) { SetStatus("⚠ 対応する仲介会社のフォーマットが見つかりません", isError: true); return; }

            // 変更：パーサーから複数案件のリストを受け取る
            List<ParsedDeal> parsedList = parser.Parse(mailBody);

            if (parsedList.Count == 0) { SetStatus("⚠ 案件情報を抽出できませんでした", isError: true); return; }

            // 1件目は現在の画面（テキストボックス）に適用する
            ApplyParsedDeal(parsedList[0]);

            // 2件目以降が存在する場合は確認ダイアログを出す
            if (parsedList.Count > 1)
            {
                var confirm = MessageBox.Show(
                    $"メールから {parsedList.Count} 件の案件が検出されました。\n\n" +
                    "1件目を現在の画面に入力し、残りの案件をデータベースに新規追加登録しますか？",
                    "複数案件の取り込み", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    int addedCount = 0;
                    for (int i = 1; i < parsedList.Count; i++)
                    {
                        // 2件目以降を新しいDealモデルに詰め替えてDBへ保存
                        var newDeal = new Deal();
                        var p = parsedList[i];

                        newDeal.InputDate = p.InputDate ?? "";
                        newDeal.Route = p.Route ?? "";
                        newDeal.BrokerCompany = p.BrokerCompany ?? "";
                        newDeal.Title = p.Title ?? "";
                        newDeal.DealId = p.DealId ?? "";
                        newDeal.BusinessContent = p.BusinessContent ?? "";
                        newDeal.Area = p.Area ?? "";
                        newDeal.Revenue = p.Revenue ?? "";
                        newDeal.OperatingProfit = p.OperatingProfit ?? "";
                        newDeal.EBITDA = p.EBITDA ?? "";
                        newDeal.NetAssets = p.NetAssets ?? "";
                        newDeal.TotalAssets = p.TotalAssets ?? "";
                        newDeal.NetCashDebt = p.NetCashDebt ?? "";
                        newDeal.CashEquivalents = p.CashEquivalents ?? "";
                        newDeal.InterestBearingDebt = p.InterestBearingDebt ?? "";
                        newDeal.EmployeeCount = p.EmployeeCount ?? "";
                        newDeal.Features = p.Features ?? "";
                        newDeal.AskingPrice = p.AskingPrice ?? "";
                        newDeal.TransferType = p.TransferType ?? "";
                        newDeal.TransferReason = p.TransferReason ?? "";
                        newDeal.TransferConditions = p.TransferConditions ?? "";
                        newDeal.Status = p.Status ?? "";

                        _dealRepo.AddDeal(newDeal);
                        addedCount++;
                    }
                    SetStatus($"✔ 1件目を画面に入力し、{addedCount}件の案件をデータベースに新規追加しました", false);
                }
                else
                {
                    SetStatus("✔ 1件目のみを画面に入力しました", false);
                }
            }
            else
            {
                SetStatus($"✔ メール本文を取り込みました（{parsedList[0].BrokerCompany}）", false);
            }
        }

        private void ApplyParsedDeal(ParsedDeal parsed)
        {
            if (parsed.InputDate != null) txtInputDate.Text = parsed.InputDate;
            if (parsed.Route != null) txtRoute.Text = parsed.Route;
            if (parsed.BrokerCompany != null) txtBrokerCompany.Text = parsed.BrokerCompany;
            if (parsed.Title != null) txtTitle.Text = parsed.Title;
            if (parsed.DealId != null) txtDealId.Text = parsed.DealId;
            if (parsed.BusinessContent != null) txtBusinessContent.Text = parsed.BusinessContent;
            if (parsed.Area != null) txtArea.Text = parsed.Area;
            if (parsed.Revenue != null) txtRevenue.Text = parsed.Revenue;
            if (parsed.OperatingProfit != null) txtOperatingProfit.Text = parsed.OperatingProfit;
            if (parsed.EBITDA != null) txtEBITDA.Text = parsed.EBITDA;
            if (parsed.NetAssets != null) txtNetAssets.Text = parsed.NetAssets;
            if (parsed.TotalAssets != null) txtTotalAssets.Text = parsed.TotalAssets;
            if (parsed.NetCashDebt != null) txtNetCashDebt.Text = parsed.NetCashDebt;
            if (parsed.CashEquivalents != null) txtCashEquivalents.Text = parsed.CashEquivalents;
            if (parsed.InterestBearingDebt != null) txtInterestBearingDebt.Text = parsed.InterestBearingDebt;
            if (parsed.EmployeeCount != null) txtEmployeeCount.Text = parsed.EmployeeCount;
            if (parsed.Features != null) txtFeatures.Text = parsed.Features;
            if (parsed.AskingPrice != null) txtAskingPrice.Text = parsed.AskingPrice;
            if (parsed.TransferType != null) txtTransferType.Text = parsed.TransferType;
            if (parsed.TransferReason != null) txtTransferReason.Text = parsed.TransferReason;
            if (parsed.TransferConditions != null) txtTransferConditions.Text = parsed.TransferConditions;
            if (parsed.Status != null) txtStatus.Text = parsed.Status;
        }

        #endregion

        #region 財務ハイライト流し込み
        /// <summary>
        /// クリップボードのTSVデータを解析し、財務ハイライトのグリッドに自動入力します
        /// </summary>
        private void btnPasteFinancial_Click(object? sender, EventArgs e)
        {
            // 1. 手順とプロンプトの案内メッセージ
            string instruction =
                "クリップボードの表データ（タブ区切りテキスト）を財務ハイライトに取り込みます。\n\n" +
                "【AI（Gemini等）で画像からデータを作成する場合】\n" +
                "まだデータをコピーしていない場合は「いいえ」を押してください。\n" +
                "AIへ指示するための「専用プロンプト（指示文）」がクリップボードにコピーされます。\n\n" +
                "すでにAIから出力された表データをコピー済みの場合は「はい」を押して取り込みを開始してください。";

            var dialogResult = MessageBox.Show(
                instruction,
                "画像から表データの取り込み",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);

            if (dialogResult == DialogResult.Cancel)
            {
                return; // 何もせず終了
            }

            // 2. 「いいえ」の場合：プロンプトをクリップボードに送って終了
            if (dialogResult == DialogResult.No)
            {
                string prompt =
                    "添付の表画像からデータを読み取り、そのままExcelやアプリに貼り付けられるよう「タブ区切りテキスト（TSV）」で出力してください。\n\n" +
                    "＜出力ルール＞\n" +
                    "1. 1行目はヘッダーとし、「項目名」と各期のラベル（例：24/3期、実績1期など）を出力してください。\n" +
                    "2. 2行目以降に、各項目の数値を出力してください。\n" +
                    "3. 表内の階層（字下げ）は無視し、項目名は左詰めで出力してください。\n" +
                    "4. 数値の桁区切りカンマ（,）は除外し、マイナス表記は半角の「-」に統一してください。\n" +
                    "5. 【重要】金額の単位は必ず「百万円単位」で出力してください（元画像が「円」や「千円」単位の場合は、百万円単位に換算して出力すること。比率(%)の項目はそのままの数値で構いません）。\n" +
                    "6. 簡単にコピーできるよう、出力は必ず1つのコードブロック（```text 〜 ```）にまとめてください。";

                Clipboard.SetText(prompt);
                MessageBox.Show(
                    "抽出用のプロンプトをクリップボードにコピーしました！\n\n" +
                    "Gemini等のチャット欄に画像を貼り付け、そのまま「貼り付け（Ctrl+V）」でプロンプトを入力して送信してください。",
                    "プロンプトのコピー完了",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }

            // 3. 「はい」の場合：実際の取り込み処理を実行
            string text = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(text))
            {
                SetStatus("⚠ クリップボードにテキストデータがありません", isError: true);
                return;
            }

            // ロジッククラスを呼び出して、構造化されたデータを取得
            var parsedData = MAItems.Database.FinancialClipboardParser.ParseTsv(text, 6);

            string[] colNames = { "col_actual_1", "col_actual_2", "col_actual_3", "col_forecast_1", "col_forecast_2", "col_forecast_3" };
            int updateCount = 0;

            // ヘッダーの反映
            foreach (var kvp in parsedData.Headers)
            {
                // 一旦変数で受け取る
                var targetColumn = dgvFinancial.Columns[colNames[kvp.Key]];

                // nullではない（列が存在する）場合のみ、HeaderTextを書き換える
                if (targetColumn != null)
                {
                    targetColumn.HeaderText = kvp.Value;
                }
            }

            // データ行の反映
            foreach (DataGridViewRow row in dgvFinancial.Rows)
            {
                string tag = row.Tag as string ?? "";

                if (parsedData.Rows.TryGetValue(tag, out var values))
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i] is double val)
                        {
                            row.Cells[colNames[i]].Value = val;
                            updateCount++;
                        }
                    }
                }
            }

            SetStatus($"✔ 表データを取り込みました（{updateCount} セル更新）", isError: false);
        }
        #endregion



        #region 共通ユーティリティ

        private void btnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void SetStatus(string msg, bool isError)
        {
            lblStatus.ForeColor = isError ? Color.Red : Color.DarkGreen;
            lblStatus.Text = msg;
        }

        #endregion
    }
}