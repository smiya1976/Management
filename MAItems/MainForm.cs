using MAItems.Database;
using MAItems.MailParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MAItems
{
    public partial class MainForm : Form
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();
        private string _cellValueBeforeEdit = string.Empty;
        private bool _isNumericMode = false;

        // ── ページング管理 ────────────────────────────────
        private List<Deal> _allDeals = new List<Deal>();
        private List<DealNumeric> _allDealNumerics = new List<DealNumeric>();

        private int _currentPage = 1;
        private int _pageSize = 20;

        private int TotalPages => _isNumericMode
            ? (int)Math.Ceiling((double)_allDealNumerics.Count / _pageSize)
            : (int)Math.Ceiling((double)_allDeals.Count / _pageSize);

        public MainForm()
        {
            InitializeComponent();
            InitializePageSizeCombo();
            SetupGrid();
            LoadData();
        }

        // ══════════════════════════════════════════════════════
        // 初期化
        // ══════════════════════════════════════════════════════

        private void InitializePageSizeCombo()
        {
            cmbPageSize.Items.AddRange(
                new object[] { 10, 20, 50, 100 });
            cmbPageSize.SelectedItem = _pageSize;
        }

        // ══════════════════════════════════════════════════════
        // グリッド設定
        // ══════════════════════════════════════════════════════

        private void SetupGrid()
        {
            dgvData.AutoGenerateColumns = false;
            dgvData.Columns.Clear();

            var columns = new
                (string prop, string header, int width, bool readOnly)[]
            {
                ("Id",                  "ID",              45,  true),
                ("InputDate",           "入力日",           90,  false),
                ("Route",               "経路",             90,  false),
                ("BrokerCompany",       "仲介会社",        140,  false),
                ("Title",               "タイトル",        160,  false),
                ("DealId",              "案件ID",           80,  false),
                ("BusinessContent",     "事業内容",        200,  false),
                ("Area",                "エリア",           80,  false),
                ("Revenue",             "売上高",           90,  false),
                ("OperatingProfit",     "営業利益",         90,  false),
                ("EBITDA",              "EBITDA",           90,  false),
                ("NetAssets",           "純資産額",         90,  false),
                ("TotalAssets",         "総資産額",         90,  false),
                ("NetCashDebt",         "Net Cash/Debt",   110,  false),
                ("CashEquivalents",     "現金等",           90,  false),
                ("InterestBearingDebt", "有利子負債",       90,  false),
                ("EmployeeCount",       "従業員数",         80,  false),
                ("Features",            "特徴",            200,  false),
                ("AskingPrice",         "譲渡希望額",      100,  false),
                ("TransferType",        "譲渡形態",        100,  false),
                ("TransferReason",      "譲渡理由",        140,  false),
                ("TransferConditions",  "希望条件",        120,  false),
                ("Status",              "処理",             80,  false),
            };

            foreach (var (prop, header, width, ro) in columns)
            {
                dgvData.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = prop,
                    DataPropertyName = prop,
                    HeaderText = header,
                    Width = width,
                    ReadOnly = ro,
                });
            }

            dgvData.CellBeginEdit += DgvData_CellBeginEdit;
            dgvData.CellEndEdit += DgvData_CellEndEdit;
            dgvData.CellValueChanged += DgvData_CellValueChanged;
            dgvData.KeyDown += DgvData_KeyDown;
            dgvData.SelectionChanged += DgvData_SelectionChanged;
        }

        private void SetupGridNumeric()
        {
            dgvData.AutoGenerateColumns = false;
            dgvData.Columns.Clear();

            var columns = new
                (string prop, string header, int width, bool isNum)[]
            {
                ("Id",                  "ID",              45,  false),
                ("InputDate",           "入力日",           90,  false),
                ("Route",               "経路",             90,  false),
                ("BrokerCompany",       "仲介会社",        140,  false),
                ("Title",               "タイトル",        160,  false),
                ("DealId",              "案件ID",           80,  false),
                ("BusinessContent",     "事業内容",        160,  false),
                ("Area",                "エリア",           80,  false),
                ("Revenue",             "売上高",          110,  true),
                ("OperatingProfit",     "営業利益",        110,  true),
                ("EBITDA",              "EBITDA",          110,  true),
                ("NetAssets",           "純資産額",        110,  true),
                ("TotalAssets",         "総資産額",        110,  true),
                ("NetCashDebt",         "Net Cash/Debt",   110,  true),
                ("CashEquivalents",     "現金等",          110,  true),
                ("InterestBearingDebt", "有利子負債",      110,  true),
                ("EmployeeCount",       "従業員数",         80,  true),
                ("AskingPrice",         "譲渡希望額",      110,  true),
                ("TransferType",        "譲渡形態",        100,  false),
                ("Status",              "処理",             80,  false),
                ("ConvertedAt",         "変換日時",        140,  false),
            };

            foreach (var (prop, header, width, isNum) in columns)
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = prop,
                    DataPropertyName = prop,
                    HeaderText = header,
                    Width = width,
                    ReadOnly = true,
                };

                if (isNum)
                {
                    col.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                    col.DefaultCellStyle.Format = "N0";
                    col.DefaultCellStyle.NullValue = "—";
                }

                dgvData.Columns.Add(col);
            }
        }

        // ══════════════════════════════════════════════════════
        // データ読み込み
        // ══════════════════════════════════════════════════════

        private void LoadData(string keyword = "")
        {
            _allDeals = string.IsNullOrWhiteSpace(keyword)
                ? _db.GetAllDeals()
                : _db.SearchDeals(keyword);

            // 常に1ページ目（最新データ）を表示
            _currentPage = 1;

            ApplyPage();
            SetStatus($"{_allDeals.Count} 件");
        }

        private void LoadDataNumeric(string keyword = "")
        {
            _allDealNumerics = string.IsNullOrWhiteSpace(keyword)
                ? _db.GetAllDealNumerics()
                : _db.SearchDealNumerics(keyword);

            // 常に1ページ目（最新データ）を表示
            _currentPage = 1;

            ApplyPageNumeric();
            SetStatus($"🔢 数値モード：{_allDealNumerics.Count} 件");
        }

        // ══════════════════════════════════════════════════════
        // ページング
        // ══════════════════════════════════════════════════════

        private void ApplyPage()
        {
            if (_allDeals.Count == 0)
            {
                dgvData.DataSource = new List<Deal>();
                UpdatePageControls(0);
                return;
            }

            _currentPage = ClampPage(_currentPage);

            // 降順（新しい順）でページを切り出す
            var page = _allDeals
                .AsEnumerable()
                .Reverse()
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            dgvData.DataSource = page;

            foreach (DataGridViewRow row in dgvData.Rows)
                row.DefaultCellStyle.BackColor =
                    System.Drawing.Color.White;

            btnDetail.Enabled = false;
            UpdatePageControls(_allDeals.Count);
        }

        private void ApplyPageNumeric()
        {
            if (_allDealNumerics.Count == 0)
            {
                dgvData.DataSource = new List<DealNumeric>();
                UpdatePageControls(0);
                return;
            }

            _currentPage = ClampPage(_currentPage);

            // 降順（新しい順）でページを切り出す
            var page = _allDealNumerics
                .AsEnumerable()
                .Reverse()
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            dgvData.DataSource = page;
            UpdatePageControls(_allDealNumerics.Count);
        }

        private int ClampPage(int page)
            => Math.Max(1, Math.Min(page, Math.Max(1, TotalPages)));

        private void UpdatePageControls(int totalCount)
        {
            int total = Math.Max(1, TotalPages);

            lblPageInfo.Text = totalCount == 0
                ? "0 件"
                : $"{_currentPage} / {total} ページ（全 {totalCount} 件）";

            btnFirstPage.Enabled = _currentPage > 1;
            btnPrevPage.Enabled = _currentPage > 1;
            btnNextPage.Enabled = _currentPage < total;
            btnLastPage.Enabled = _currentPage < total;
        }

        // ══════════════════════════════════════════════════════
        // ページングボタンイベント
        // ══════════════════════════════════════════════════════

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            _currentPage = 1;
            if (_isNumericMode) ApplyPageNumeric();
            else ApplyPage();
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (_currentPage <= 1) return;
            _currentPage--;
            if (_isNumericMode) ApplyPageNumeric();
            else ApplyPage();
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (_currentPage >= TotalPages) return;
            _currentPage++;
            if (_isNumericMode) ApplyPageNumeric();
            else ApplyPage();
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            _currentPage = Math.Max(1, TotalPages);
            if (_isNumericMode) ApplyPageNumeric();
            else ApplyPage();
        }

        private void cmbPageSize_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (cmbPageSize.SelectedItem == null) return;

            _pageSize = (int)cmbPageSize.SelectedItem;
            _currentPage = 1;

            if (_isNumericMode) ApplyPageNumeric();
            else ApplyPage();
        }

        // ══════════════════════════════════════════════════════
        // グリッドイベント
        // ══════════════════════════════════════════════════════

        private void DgvData_SelectionChanged(object? sender, EventArgs e)
        {
            btnDetail.Enabled = !_isNumericMode
                             && dgvData.CurrentRow?.DataBoundItem is Deal;
        }

        private void DgvData_CellBeginEdit(object? sender,
            DataGridViewCellCancelEventArgs e)
        {
            _cellValueBeforeEdit =
                dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value
                    ?.ToString() ?? string.Empty;
            SetStatus("✏ 編集中... （Escキーでキャンセル）");
        }

        private void DgvData_CellEndEdit(object? sender,
            DataGridViewCellEventArgs e)
        {
            if (dgvData.Rows[e.RowIndex].DataBoundItem is not Deal deal)
                return;

            try
            {
                _db.UpdateDeal(deal);
                SetStatus($"✔ ID:{deal.Id} を更新しました");
            }
            catch (Exception ex)
            {
                dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value
                    = _cellValueBeforeEdit;
                SetStatus($"❌ 更新エラー: {ex.Message}", isError: true);
            }
        }

        private void DgvData_CellValueChanged(object? sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dgvData.Rows[e.RowIndex].DefaultCellStyle.BackColor
                = System.Drawing.Color.LightYellow;
        }

        private void DgvData_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                SetStatus("編集をキャンセルしました");
        }

        // ══════════════════════════════════════════════════════
        // ボタンイベント
        // ══════════════════════════════════════════════════════

        private void txtSearch_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSearch_Click(sender!, e);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (_isNumericMode)
                LoadDataNumeric(txtSearch.Text.Trim());
            else
                LoadData(txtSearch.Text.Trim());
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            if (_isNumericMode) LoadDataNumeric();
            else LoadData();
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            if (dgvData.CurrentRow?.DataBoundItem is not Deal deal)
            {
                SetStatus("⚠ 詳細を表示する行を選択してください",
                    isError: true);
                return;
            }

            var detailForm = new DetailForm(deal, _db);
            detailForm.SaveCompleted += (s, ev)
                => LoadData(txtSearch.Text.Trim());
            detailForm.ShowDialog(this);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                long newId = _db.AddEmptyDeal();
                txtSearch.Clear();
                LoadData();  // 1ページ目（最新）へ移動

                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    if (row.DataBoundItem is Deal deal && deal.Id == newId)
                    {
                        dgvData.ClearSelection();
                        row.Selected = true;
                        dgvData.FirstDisplayedScrollingRowIndex = row.Index;
                        dgvData.CurrentCell = row.Cells["Route"];
                        dgvData.BeginEdit(true);
                        break;
                    }
                }

                SetStatus(
                    $"✔ 新規行を追加しました（ID: {newId}）。" +
                    $"そのまま編集できます。");
            }
            catch (Exception ex)
            {
                SetStatus($"❌ 新規行追加エラー: {ex.Message}",
                    isError: true);
            }
        }

        // ── 追加: データ管理画面の呼び出しイベント ──
        private void btnDataSync_Click(object sender, EventArgs e)
        {
            using var syncForm = new DataSyncForm(_db);

            // データ管理画面でインポートや復元が実行された場合は、
            // ダイアログが閉じられた後にメイン画面の一覧をリフレッシュする
            if (syncForm.ShowDialog(this) == DialogResult.OK)
            {
                if (_isNumericMode)
                    LoadDataNumeric(txtSearch.Text.Trim());
                else
                    LoadData(txtSearch.Text.Trim());
            }
        }

        private void btnToggleNumeric_Click(object sender, EventArgs e)
        {
            _isNumericMode = !_isNumericMode;

            if (_isNumericMode)
            {
                try
                {
                    SetupGridNumeric();
                    LoadDataNumeric(txtSearch.Text.Trim());

                    btnToggleNumeric.Text = "📋 通常モード";
                    btnToggleNumeric.BackColor =
                        System.Drawing.Color.LightSalmon;
                    btnAdd.Enabled = false;
                    btnDelete.Enabled = false;
                    btnDetail.Enabled = false;
                    btnDataSync.Enabled = false;

                    SetStatus("🔢 数値モードに切り替えました");
                }
                catch (Exception ex)
                {
                    _isNumericMode = false;
                    SetStatus($"❌ 表示切替エラー: {ex.Message}",
                        isError: true);
                }
            }
            else
            {
                SetupGrid();
                LoadData(txtSearch.Text.Trim());

                btnToggleNumeric.Text = "🔢 数値モード";
                btnToggleNumeric.BackColor =
                    System.Drawing.Color.LightCyan;
                btnAdd.Enabled = true;
                btnDelete.Enabled = true;
                btnDataSync.Enabled = true;

                SetStatus("📋 通常モードに戻りました");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvData.CurrentRow?.DataBoundItem is not Deal selected)
            {
                SetStatus("⚠ 削除する行を選択してください",
                    isError: true);
                return;
            }

            var confirm = MessageBox.Show(
                $"選択中のレコード（ID: {selected.Id}）を削除しますか？",
                "削除確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                _db.DeleteDeal(selected.Id);
                LoadData(txtSearch.Text.Trim());
                SetStatus($"✔ ID:{selected.Id} を削除しました");
            }
            catch (Exception ex)
            {
                SetStatus($"❌ エラー: {ex.Message}", isError: true);
            }
        }

        // ══════════════════════════════════════════════════════
        // ステータス
        // ══════════════════════════════════════════════════════

        private void SetStatus(string msg, bool isError = false)
        {
            lblStatus.ForeColor = isError
                ? System.Drawing.Color.Red
                : System.Drawing.Color.DarkGreen;
            lblStatus.Text = msg;
        }
    }
}