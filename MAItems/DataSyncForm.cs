using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MAItems.Database;

namespace MAItems
{
    public partial class DataSyncForm : Form
    {
        private readonly DealRepository _dealRepo;
        private readonly DataSyncService _syncService;

        public DataSyncForm(DatabaseContext context)
        {
            InitializeComponent();

            // 画面用のサービス群を初期化
            _dealRepo = new DealRepository(context);
            _syncService = new DataSyncService(context, _dealRepo);
        }

        /// <summary>
        /// 1. 複数CSV一括エクスポート機能
        /// </summary>
        private void btnExportMultiCsv_Click(object sender, EventArgs e)
        {
            string? fromDate = chkUseDate.Checked ? dtpFrom.Value.ToString("yyyy/M/d") : null;
            string? toDate = chkUseDate.Checked ? dtpTo.Value.ToString("yyyy/M/d") : null;
            string keyword = txtKeyword.Text.Trim();

            // 検索はDealRepository経由
            List<Deal> targetDeals = _dealRepo.GetDealsByFilter(fromDate, toDate, keyword);
            if (targetDeals.Count == 0) return;

            using var folderDlg = new FolderBrowserDialog();
            if (folderDlg.ShowDialog() != DialogResult.OK) return;

            string baseDir = folderDlg.SelectedPath;
            List<long> dealIds = new List<long>();
            foreach (var d in targetDeals) dealIds.Add(d.Id);

            // 出力ロジックは DataSyncService 経由
            _syncService.ExportDealsToCsv(Path.Combine(baseDir, "1_案件一覧.csv"), targetDeals);
            _syncService.ExportProfilesToCsv(Path.Combine(baseDir, "2_会社基礎情報.csv"), dealIds);
            _syncService.ExportFinancialsToCsv(Path.Combine(baseDir, "3_財務ハイライト.csv"), dealIds);
            _syncService.ExportValuationsToCsv(Path.Combine(baseDir, "4_株式価値試算.csv"), dealIds);
            _syncService.ExportAttachmentsToCsv(Path.Combine(baseDir, "5_添付ファイル情報.csv"), dealIds);
        }

        /// <summary>
        /// 2. 環境丸ごとZIPバックアップ
        /// </summary>
        private void btnBackupZip_Click(object sender, EventArgs e)
        {
            using var saveDlg = new SaveFileDialog
            {
                Title = "完全バックアップZIPファイルの保存先指定",
                Filter = "ZIPファイル (*.zip)|*.zip",
                FileName = $"MA_FullBackup_{DateTime.Now:yyyyMMdd_HHmmss}.zip"
            };

            if (saveDlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                lblStatus.Text = "バックアップ作成中...";
                this.Refresh();

                _syncService.CreateBackupZip(saveDlg.FileName);

                lblStatus.Text = "バックアップ完了";
                MessageBox.Show("データベースと添付資料の実体を含めた完全バックアップZIPを作成しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "バックアップエラー";
                MessageBox.Show("バックアップ作成中にエラーが発生しました:\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 3. 環境丸ごとZIPからの復元 (リストア)
        /// </summary>
        private void btnRestoreZip_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "本当にバックアップZIPから復元を行いますか？\n\n" +
                "【警告】現在のすべてのデータ、財務情報、および保管ファイルの実体は消失し、バックアップ時点の状態に完全に上書きされます。",
                "データ復元の最終確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            using var openDlg = new OpenFileDialog
            {
                Title = "復元するバックアップZIPファイルを選択",
                Filter = "ZIPファイル (*.zip)|*.zip"
            };

            if (openDlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                lblStatus.Text = "データを復元中...";
                this.Refresh();

                _syncService.RestoreFromZip(openDlg.FileName);

                lblStatus.Text = "復元完了";
                MessageBox.Show("環境の完全復元が完了しました。アプリケーションを再起動してデータを反映してください。※不整合防止のため、画面を閉じます。", "復元成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK; // MainForm側に再インデックスを促すため設定
                this.Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "復元エラー";
                MessageBox.Show("復元処理中に致命的なエラーが発生しました:\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 4. 既存のCSVインポート
        /// </summary>
        private void btnImportCsv_Click(object sender, EventArgs e)
        {
            using var openDlg = new OpenFileDialog
            {
                Title = "一括インポートする新規案件CSVを選択",
                Filter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*"
            };

            if (openDlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                lblStatus.Text = "インポート中...";
                this.Refresh();

                var (added, skipped) = _syncService.ImportFromCsv(openDlg.FileName);

                lblStatus.Text = "インポート完了";
                MessageBox.Show($"インポートが完了しました。\n追加: {added} 件\nスキップ(重複): {skipped} 件", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "インポートエラー";
                MessageBox.Show("インポート中にエラーが発生しました:\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ══════════════════════════════════════════════════════
        // UI イベントハンドラ
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 入力日で絞り込むチェックボックスの切り替えイベント
        /// </summary>
        private void chkUseDate_CheckedChanged(object? sender, EventArgs e)
        {
            // チェック状態に応じて、日付選択コントロールの有効/無効を切り替える
            dtpFrom.Enabled = chkUseDate.Checked;
            dtpTo.Enabled = chkUseDate.Checked;
        }

        /// <summary>
        /// 閉じるボタンのクリックイベント
        /// </summary>
        private void btnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}