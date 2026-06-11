using System;
using System.IO;
using System.Windows.Forms;
using MAItems.Database;

namespace MAItems
{
    public partial class DataSyncForm : Form
    {
        private readonly DatabaseContext _context;
        private readonly DataSyncService _syncService;

        public DataSyncForm(DatabaseContext context)
        {
            InitializeComponent();
            _context = context;
            _syncService = new DataSyncService(_context);

            // 画面初期化：日付のデフォルト値を設定
            dtpFrom.Value = DateTime.Today.AddMonths(-3);
            dtpTo.Value = DateTime.Today;
        }

        // ══════════════════════════════════════════════════════
        // 1. エクスポート処理
        // ══════════════════════════════════════════════════════
        private void btnExportMultiCsv_Click(object sender, EventArgs e)
        {
            using var folderDlg = new FolderBrowserDialog
            {
                Description = "CSVを出力するフォルダを選択してください"
            };

            if (folderDlg.ShowDialog() != DialogResult.OK) return;

            string? fromDate = chkUseDate.Checked ? dtpFrom.Value.ToString("yyyy/MM/dd") : null;
            string? toDate = chkUseDate.Checked ? dtpTo.Value.ToString("yyyy/MM/dd") : null;
            string keyword = txtKeyword.Text.Trim();

            try
            {
                // エクスポート実行（※ご自身のDataSyncService内のメソッド名に合わせて適宜調整してください）
                int count = _syncService.ExportData(folderDlg.SelectedPath, fromDate, toDate, keyword);
                MessageBox.Show($"{count} 件の案件に関連するデータをCSVとして出力しました。\n\n出力先: {folderDlg.SelectedPath}",
                    "エクスポート完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エクスポート中にエラーが発生しました:\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ══════════════════════════════════════════════════════
        // 2. インポート処理 (Upsert)
        // ══════════════════════════════════════════════════════
        private void btnImportCsv_Click(object sender, EventArgs e)
        {
            using var openDlg = new OpenFileDialog { Filter = "CSVファイル|*.csv", Title = "1_案件一覧.csv を選択" };
            if (openDlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                int count = _syncService.ImportDeals(openDlg.FileName);
                MessageBox.Show($"{count} 件の案件一覧を取り込みました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void btnImportProfile_Click(object sender, EventArgs e)
        {
            using var openDlg = new OpenFileDialog { Filter = "CSVファイル|*.csv", Title = "2_会社基礎情報.csv を選択" };
            if (openDlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                int count = _syncService.ImportCompanyProfiles(openDlg.FileName);
                MessageBox.Show($"{count} 件の会社基礎情報を上書き更新しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void btnImportFinancial_Click(object sender, EventArgs e)
        {
            using var openDlg = new OpenFileDialog { Filter = "CSVファイル|*.csv", Title = "3_財務ハイライト.csv を選択" };
            if (openDlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                int count = _syncService.ImportFinancials(openDlg.FileName);
                MessageBox.Show($"{count} 件の財務ハイライト情報を上書き更新しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void btnImportValuations_Click(object sender, EventArgs e)
        {
            using var openDlg = new OpenFileDialog { Filter = "CSVファイル|*.csv", Title = "4_株式価値試算.csv を選択" };
            if (openDlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                int count = _syncService.ImportValuations(openDlg.FileName);
                MessageBox.Show($"{count} 件の株式価値試算データを上書き更新しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void btnImportAttachments_Click(object sender, EventArgs e)
        {
            using var openDlg = new OpenFileDialog { Filter = "CSVファイル|*.csv", Title = "5_添付ファイル情報.csv を選択" };
            if (openDlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                int count = _syncService.ImportAttachments(openDlg.FileName);
                MessageBox.Show($"{count} 件の添付ファイル情報を上書き更新しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        // ══════════════════════════════════════════════════════
        // 3. ZIP丸ごとバックアップ・復元処理
        // ══════════════════════════════════════════════════════
        private void btnBackupZip_Click(object sender, EventArgs e)
        {
            using var saveDlg = new SaveFileDialog
            {
                Filter = "ZIPファイル|*.zip",
                FileName = $"MA_Backup_{DateTime.Now:yyyyMMdd_HHmm}.zip",
                Title = "バックアップファイルの保存先を指定"
            };

            if (saveDlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                _syncService.CreateBackupZip(saveDlg.FileName);
                MessageBox.Show($"すべてのデータをバックアップしました。\n\n保存先: {saveDlg.FileName}",
                    "バックアップ完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void btnRestoreZip_Click(object sender, EventArgs e)
        {
            using var openDlg = new OpenFileDialog
            {
                Filter = "ZIPファイル|*.zip",
                Title = "復元するバックアップファイル(ZIP)を選択"
            };

            if (openDlg.ShowDialog() != DialogResult.OK) return;

            var confirm = MessageBox.Show(
                "現在のシステムデータはすべて削除され、選択したバックアップファイルの内容で上書きされます。\n本当に復元を実行しますか？",
                "復元の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                _syncService.RestoreFromZip(openDlg.FileName);
                MessageBox.Show("データの復元が完了しました。\nアプリを再起動するか、一覧画面を更新してデータを確認してください。",
                    "復元完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        // 共通エラーメッセージ表示メソッド
        private void ShowError(Exception ex)
        {
            MessageBox.Show($"エラーが発生しました:\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}