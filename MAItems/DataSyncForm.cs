using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MAItems.Database;

namespace MAItems
{
    public partial class DataSyncForm : Form
    {
        private readonly DatabaseHelper _db;

        public DataSyncForm(DatabaseHelper db)
        {
            InitializeComponent();
            _db = db;
        }

        /// <summary>
        /// 1. 複数CSV一括エクスポート機能
        /// </summary>
        private void btnExportMultiCsv_Click(object sender, EventArgs e)
        {
            string? fromDate = chkUseDate.Checked ? dtpFrom.Value.ToString("yyyy/M/d") : null;
            string? toDate = chkUseDate.Checked ? dtpTo.Value.ToString("yyyy/M/d") : null;
            string keyword = txtKeyword.Text.Trim();

            // フィルタに一致する案件を取得
            List<Deal> targetDeals = _db.GetDealsByFilter(fromDate, toDate, keyword);

            if (targetDeals.Count == 0)
            {
                MessageBox.Show("指定された条件に合致する案件データが見つかりません。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var folderDlg = new FolderBrowserDialog();
            folderDlg.Description = $"{targetDeals.Count} 件の関連データをエクスポートするフォルダを選択してください。";

            if (folderDlg.ShowDialog() != DialogResult.OK) return;

            string baseDir = folderDlg.SelectedPath;
            List<long> dealIds = new List<long>();
            foreach (var d in targetDeals) dealIds.Add(d.Id);

            try
            {
                lblStatus.Text = "エクスポート中...";
                this.Refresh();

                // 各テーブルを個別のCSVに出力
                _db.ExportDealsToCsv(Path.Combine(baseDir, "1_案件一覧(Deals).csv"), targetDeals);
                _db.ExportProfilesToCsv(Path.Combine(baseDir, "2_会社基礎情報(Profiles).csv"), dealIds);
                _db.ExportFinancialsToCsv(Path.Combine(baseDir, "3_財務ハイライト(Financials).csv"), dealIds);
                _db.ExportValuationsToCsv(Path.Combine(baseDir, "4_株式価値試算(Valuations).csv"), dealIds);
                _db.ExportAttachmentsToCsv(Path.Combine(baseDir, "5_添付ファイル情報(Attachments).csv"), dealIds);

                lblStatus.Text = "エクスポート完了";
                MessageBox.Show($"指定フォルダに関連全データのCSVファイル（5枚）を出力しました。\n出力件数: {targetDeals.Count}件", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                System.Diagnostics.Process.Start("explorer.exe", baseDir);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "エクスポートエラー";
                MessageBox.Show("エクスポート中にエラーが発生しました:\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                _db.CreateBackupZip(saveDlg.FileName);

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

                _db.RestoreFromZip(openDlg.FileName);

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

                var (added, skipped) = _db.ImportFromCsv(openDlg.FileName);

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
    }
}