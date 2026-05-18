using System.Windows.Forms;

namespace MAItems
{
    partial class DataSyncForm
    {
        private System.ComponentModel.IContainer components = null;

        // 検索・フィルタ用
        private GroupBox grpFilter;
        private Label lblDateRange;
        private DateTimePicker dtpFrom;
        private Label lblTo;
        private DateTimePicker dtpTo;
        private CheckBox chkUseDate;
        private Label lblKeyword;
        private TextBox txtKeyword;

        // 各機能用のグループボックスとボタン
        private GroupBox grpCsvExport;
        private Button btnExportMultiCsv;
        private Label lblExportNote;

        private GroupBox grpBackup;
        private Button btnBackupZip;
        private Button btnRestoreZip;
        private Label lblBackupNote;

        private GroupBox grpCsvImport;
        private Button btnImportCsv;
        private Label lblImportNote;

        private Button btnClose;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpFilter = new GroupBox();
            this.lblDateRange = new Label();
            this.dtpFrom = new DateTimePicker();
            this.lblTo = new Label();
            this.dtpTo = new DateTimePicker();
            this.chkUseDate = new CheckBox();
            this.lblKeyword = new Label();
            this.txtKeyword = new TextBox();

            this.grpCsvExport = new GroupBox();
            this.btnExportMultiCsv = new Button();
            this.lblExportNote = new Label();

            this.grpBackup = new GroupBox();
            this.btnBackupZip = new Button();
            this.btnRestoreZip = new Button();
            this.lblBackupNote = new Label();

            this.grpCsvImport = new GroupBox();
            this.btnImportCsv = new Button();
            this.lblImportNote = new Label();

            this.btnClose = new Button();
            this.statusStrip = new StatusStrip();
            this.lblStatus = new ToolStripStatusLabel();

            this.grpFilter.SuspendLayout();
            this.grpCsvExport.SuspendLayout();
            this.grpBackup.SuspendLayout();
            this.grpCsvImport.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();

            // ── フォーム設定 ──
            this.Text = "⚙ データ管理（インポート・エクスポート・バックアップ）";
            this.Size = new System.Drawing.Size(640, 580);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // ── 1. 抽出フィルタ条件 ──
            this.grpFilter.Text = "対象案件の抽出条件（Excelエクスポート用）";
            this.grpFilter.Location = new System.Drawing.Point(16, 16);
            this.grpFilter.Size = new System.Drawing.Size(592, 110);

            this.chkUseDate.Text = "入力日で絞り込む";
            this.chkUseDate.Location = new System.Drawing.Point(16, 24);
            this.chkUseDate.Size = new System.Drawing.Size(150, 20);
            this.chkUseDate.Checked = false;
            this.chkUseDate.CheckedChanged += (s, e) => {
                dtpFrom.Enabled = chkUseDate.Checked;
                dtpTo.Enabled = chkUseDate.Checked;
            };

            this.dtpFrom.Format = DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(16, 48);
            this.dtpFrom.Size = new System.Drawing.Size(120, 23);
            this.dtpFrom.Enabled = false;

            this.lblTo.Text = "～";
            this.lblTo.Location = new System.Drawing.Point(142, 51);
            this.lblTo.Size = new System.Drawing.Size(20, 23);

            this.dtpTo.Format = DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(166, 48);
            this.dtpTo.Size = new System.Drawing.Size(120, 23);
            this.dtpTo.Enabled = false;

            this.lblKeyword.Text = "キーワード検索 (タイトル、事業内容、エリア、仲介会社、処理):";
            this.lblKeyword.Location = new System.Drawing.Point(16, 80);
            this.lblKeyword.Size = new System.Drawing.Size(350, 20);

            this.txtKeyword.Location = new System.Drawing.Point(366, 77);
            this.txtKeyword.Size = new System.Drawing.Size(210, 23);

            this.grpFilter.Controls.AddRange(new Control[] { this.chkUseDate, this.dtpFrom, this.lblTo, this.dtpTo, this.lblKeyword, this.txtKeyword });

            // ── 2. Excel用 複数CSVエクスポート ──
            this.grpCsvExport.Text = "Excel閲覧・集計用データ出力";
            this.grpCsvExport.Location = new System.Drawing.Point(16, 140);
            this.grpCsvExport.Size = new System.Drawing.Size(592, 100);

            this.btnExportMultiCsv.Text = "📤 関連全テーブルを複数CSVで一括出力";
            this.btnExportMultiCsv.Location = new System.Drawing.Point(16, 28);
            this.btnExportMultiCsv.Size = new System.Drawing.Size(280, 36);
            this.btnExportMultiCsv.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.btnExportMultiCsv.Click += new System.EventHandler(this.btnExportMultiCsv_Click);

            this.lblExportNote.Text = "※上記の抽出条件に合致する案件データと、それに紐づく会社基礎情報・財務情報・試算データ・添付ファイル情報を別々のCSVとしてフォルダ内に一括出力します。";
            this.lblExportNote.Location = new System.Drawing.Point(16, 68);
            this.lblExportNote.Size = new System.Drawing.Size(560, 25);
            this.lblExportNote.ForeColor = System.Drawing.Color.DimGray;

            this.grpCsvExport.Controls.AddRange(new Control[] { this.btnExportMultiCsv, this.lblExportNote });

            // ── 3. 環境丸ごとZIPバックアップ・復元 ──
            this.grpBackup.Text = "他PCへのデータ移行・完全バックアップ (推奨)";
            this.grpBackup.Location = new System.Drawing.Point(16, 255);
            this.grpBackup.Size = new System.Drawing.Size(592, 115);

            this.btnBackupZip.Text = "📦 まるごと丸ごとZIPバックアップ";
            this.btnBackupZip.Location = new System.Drawing.Point(16, 28);
            this.btnBackupZip.Size = new System.Drawing.Size(220, 36);
            this.btnBackupZip.BackColor = System.Drawing.Color.LightCyan;
            this.btnBackupZip.Click += new System.EventHandler(this.btnBackupZip_Click);

            this.btnRestoreZip.Text = "♻ ZIPからデータベース復元 (リストア)";
            this.btnRestoreZip.Location = new System.Drawing.Point(246, 28);
            this.btnRestoreZip.Size = new System.Drawing.Size(240, 36);
            this.btnRestoreZip.BackColor = System.Drawing.Color.MistyRose;
            this.btnRestoreZip.Click += new System.EventHandler(this.btnRestoreZip_Click);

            this.lblBackupNote.Text = "※注意: 復元を行うと、現在のアプリ内データ及び保管ファイルは全てバックアップ時点の状態に上書きされます。添付ファイルの実体も含めて完全移行が可能です。";
            this.lblBackupNote.Location = new System.Drawing.Point(16, 72);
            this.lblBackupNote.Size = new System.Drawing.Size(560, 35);
            this.lblBackupNote.ForeColor = System.Drawing.Color.Firebrick;

            this.grpBackup.Controls.AddRange(new Control[] { this.btnBackupZip, this.btnRestoreZip, this.lblBackupNote });

            // ── 4. 既存のCSV一括インポート機能の移設 ──
            this.grpCsvImport.Text = "新規案件一括登録";
            this.grpCsvImport.Location = new System.Drawing.Point(16, 385);
            this.grpCsvImport.Size = new System.Drawing.Size(592, 90);

            this.btnImportCsv.Text = "📂 新規案件CSVをインポート";
            this.btnImportCsv.Location = new System.Drawing.Point(16, 24);
            this.btnImportCsv.Size = new System.Drawing.Size(220, 36);
            this.btnImportCsv.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnImportCsv.Click += new System.EventHandler(this.btnImportCsv_Click);

            this.lblImportNote.Text = "※既存の案件一覧CSVファイルを読み込み、データベースへ一括で新規追加登録します（仲介会社名と案件IDが重複するデータは自動スキップされます）。";
            this.lblImportNote.Location = new System.Drawing.Point(16, 64);
            this.lblImportNote.Size = new System.Drawing.Size(560, 20);
            this.lblImportNote.ForeColor = System.Drawing.Color.DimGray;

            this.grpCsvImport.Controls.AddRange(new Control[] { this.btnImportCsv, this.lblImportNote });

            // ── 閉じるボタンとステータス ──
            this.btnClose.Text = "✖ 閉じる";
            this.btnClose.Location = new System.Drawing.Point(488, 490);
            this.btnClose.Size = new System.Drawing.Size(120, 32);
            this.btnClose.Click += (s, e) => this.Close();

            this.statusStrip.Items.AddRange(new ToolStripItem[] { this.lblStatus });
            this.lblStatus.Text = "準備完了";

            this.Controls.AddRange(new Control[] { this.grpFilter, this.grpCsvExport, this.grpBackup, this.grpCsvImport, this.btnClose, this.statusStrip });

            this.grpFilter.ResumeLayout(false);
            this.grpFilter.PerformLayout();
            this.grpCsvExport.ResumeLayout(false);
            this.grpBackup.ResumeLayout(false);
            this.grpCsvImport.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}