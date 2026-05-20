namespace MAItems
{
    partial class DataSyncForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxExport = new System.Windows.Forms.GroupBox();
            this.btnExportMultiCsv = new System.Windows.Forms.Button();
            this.lblTilde = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.chkUseDate = new System.Windows.Forms.CheckBox();
            this.txtKeyword = new System.Windows.Forms.TextBox();
            this.lblKeyword = new System.Windows.Forms.Label();

            this.groupBoxImport = new System.Windows.Forms.GroupBox();
            this.btnImportCsv = new System.Windows.Forms.Button();
            this.btnImportProfile = new System.Windows.Forms.Button();
            this.btnImportFinancial = new System.Windows.Forms.Button();
            this.btnImportValuations = new System.Windows.Forms.Button();
            this.btnImportAttachments = new System.Windows.Forms.Button();

            this.groupBoxBackup = new System.Windows.Forms.GroupBox();
            this.btnRestoreZip = new System.Windows.Forms.Button();
            this.btnBackupZip = new System.Windows.Forms.Button();

            this.groupBoxExport.SuspendLayout();
            this.groupBoxImport.SuspendLayout();
            this.groupBoxBackup.SuspendLayout();
            this.SuspendLayout();

            // 
            // groupBoxExport
            // 
            this.groupBoxExport.Controls.Add(this.btnExportMultiCsv);
            this.groupBoxExport.Controls.Add(this.lblTilde);
            this.groupBoxExport.Controls.Add(this.dtpTo);
            this.groupBoxExport.Controls.Add(this.dtpFrom);
            this.groupBoxExport.Controls.Add(this.chkUseDate);
            this.groupBoxExport.Controls.Add(this.txtKeyword);
            this.groupBoxExport.Controls.Add(this.lblKeyword);
            this.groupBoxExport.Location = new System.Drawing.Point(20, 20);
            this.groupBoxExport.Name = "groupBoxExport";
            this.groupBoxExport.Size = new System.Drawing.Size(400, 160);
            this.groupBoxExport.TabIndex = 0;
            this.groupBoxExport.TabStop = false;
            this.groupBoxExport.Text = "1. エクスポート (検索条件に一致するデータをCSV出力)";

            // lblKeyword
            this.lblKeyword.AutoSize = true;
            this.lblKeyword.Location = new System.Drawing.Point(20, 35);
            this.lblKeyword.Name = "lblKeyword";
            this.lblKeyword.Size = new System.Drawing.Size(65, 15);
            this.lblKeyword.TabIndex = 0;
            this.lblKeyword.Text = "キーワード:";

            // txtKeyword
            this.txtKeyword.Location = new System.Drawing.Point(90, 32);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(280, 23);
            this.txtKeyword.TabIndex = 1;

            // chkUseDate
            this.chkUseDate.AutoSize = true;
            this.chkUseDate.Location = new System.Drawing.Point(23, 73);
            this.chkUseDate.Name = "chkUseDate";
            this.chkUseDate.Size = new System.Drawing.Size(98, 19);
            this.chkUseDate.TabIndex = 2;
            this.chkUseDate.Text = "日付で絞り込む";
            this.chkUseDate.UseVisualStyleBackColor = true;

            // dtpFrom
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(125, 70);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(100, 23);
            this.dtpFrom.TabIndex = 3;

            // lblTilde
            this.lblTilde.AutoSize = true;
            this.lblTilde.Location = new System.Drawing.Point(230, 75);
            this.lblTilde.Name = "lblTilde";
            this.lblTilde.Size = new System.Drawing.Size(19, 15);
            this.lblTilde.TabIndex = 4;
            this.lblTilde.Text = "～";

            // dtpTo
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(255, 70);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(100, 23);
            this.dtpTo.TabIndex = 5;

            // btnExportMultiCsv
            this.btnExportMultiCsv.BackColor = System.Drawing.Color.LightBlue;
            this.btnExportMultiCsv.Location = new System.Drawing.Point(20, 110);
            this.btnExportMultiCsv.Name = "btnExportMultiCsv";
            this.btnExportMultiCsv.Size = new System.Drawing.Size(350, 35);
            this.btnExportMultiCsv.TabIndex = 6;
            this.btnExportMultiCsv.Text = "📁 全5テーブルをCSVとしてフォルダに出力する";
            this.btnExportMultiCsv.UseVisualStyleBackColor = false;
            this.btnExportMultiCsv.Click += new System.EventHandler(this.btnExportMultiCsv_Click);

            // 
            // groupBoxImport
            // 
            this.groupBoxImport.Controls.Add(this.btnImportCsv);
            this.groupBoxImport.Controls.Add(this.btnImportProfile);
            this.groupBoxImport.Controls.Add(this.btnImportFinancial);
            this.groupBoxImport.Controls.Add(this.btnImportValuations);
            this.groupBoxImport.Controls.Add(this.btnImportAttachments);
            this.groupBoxImport.Location = new System.Drawing.Point(20, 195);
            this.groupBoxImport.Name = "groupBoxImport";
            this.groupBoxImport.Size = new System.Drawing.Size(400, 205);
            this.groupBoxImport.TabIndex = 1;
            this.groupBoxImport.TabStop = false;
            this.groupBoxImport.Text = "2. インポート (Excelで編集したCSVを上書き・追加)";

            // btnImportCsv
            this.btnImportCsv.Location = new System.Drawing.Point(20, 30);
            this.btnImportCsv.Name = "btnImportCsv";
            this.btnImportCsv.Size = new System.Drawing.Size(350, 28);
            this.btnImportCsv.TabIndex = 0;
            this.btnImportCsv.Text = "📄 1_案件一覧.csv を取り込む";
            this.btnImportCsv.UseVisualStyleBackColor = true;
            this.btnImportCsv.Click += new System.EventHandler(this.btnImportCsv_Click);

            // btnImportProfile
            this.btnImportProfile.Location = new System.Drawing.Point(20, 63);
            this.btnImportProfile.Name = "btnImportProfile";
            this.btnImportProfile.Size = new System.Drawing.Size(350, 28);
            this.btnImportProfile.TabIndex = 1;
            this.btnImportProfile.Text = "🏢 2_会社基礎情報.csv を取り込む";
            this.btnImportProfile.UseVisualStyleBackColor = true;
            this.btnImportProfile.Click += new System.EventHandler(this.btnImportProfile_Click);

            // btnImportFinancial
            this.btnImportFinancial.Location = new System.Drawing.Point(20, 96);
            this.btnImportFinancial.Name = "btnImportFinancial";
            this.btnImportFinancial.Size = new System.Drawing.Size(350, 28);
            this.btnImportFinancial.TabIndex = 2;
            this.btnImportFinancial.Text = "📊 3_財務ハイライト.csv を取り込む";
            this.btnImportFinancial.UseVisualStyleBackColor = true;
            this.btnImportFinancial.Click += new System.EventHandler(this.btnImportFinancial_Click);

            // btnImportValuations
            this.btnImportValuations.Location = new System.Drawing.Point(20, 129);
            this.btnImportValuations.Name = "btnImportValuations";
            this.btnImportValuations.Size = new System.Drawing.Size(350, 28);
            this.btnImportValuations.TabIndex = 3;
            this.btnImportValuations.Text = "📈 4_株式価値試算.csv を取り込む";
            this.btnImportValuations.UseVisualStyleBackColor = true;
            this.btnImportValuations.Click += new System.EventHandler(this.btnImportValuations_Click);

            // btnImportAttachments
            this.btnImportAttachments.Location = new System.Drawing.Point(20, 162);
            this.btnImportAttachments.Name = "btnImportAttachments";
            this.btnImportAttachments.Size = new System.Drawing.Size(350, 28);
            this.btnImportAttachments.TabIndex = 4;
            this.btnImportAttachments.Text = "📎 5_添付ファイル情報.csv を取り込む";
            this.btnImportAttachments.UseVisualStyleBackColor = true;
            this.btnImportAttachments.Click += new System.EventHandler(this.btnImportAttachments_Click);

            // 
            // groupBoxBackup
            // 
            this.groupBoxBackup.Controls.Add(this.btnRestoreZip);
            this.groupBoxBackup.Controls.Add(this.btnBackupZip);
            this.groupBoxBackup.Location = new System.Drawing.Point(20, 415);
            this.groupBoxBackup.Name = "groupBoxBackup";
            this.groupBoxBackup.Size = new System.Drawing.Size(400, 100);
            this.groupBoxBackup.TabIndex = 2;
            this.groupBoxBackup.TabStop = false;
            this.groupBoxBackup.Text = "3. システム丸ごとバックアップ・復元";

            // btnBackupZip
            this.btnBackupZip.BackColor = System.Drawing.Color.MistyRose;
            this.btnBackupZip.Location = new System.Drawing.Point(20, 35);
            this.btnBackupZip.Name = "btnBackupZip";
            this.btnBackupZip.Size = new System.Drawing.Size(170, 45);
            this.btnBackupZip.TabIndex = 0;
            this.btnBackupZip.Text = "📦 ZIPバックアップ作成";
            this.btnBackupZip.UseVisualStyleBackColor = false;
            this.btnBackupZip.Click += new System.EventHandler(this.btnBackupZip_Click);

            // btnRestoreZip
            this.btnRestoreZip.BackColor = System.Drawing.Color.LightYellow;
            this.btnRestoreZip.Location = new System.Drawing.Point(200, 35);
            this.btnRestoreZip.Name = "btnRestoreZip";
            this.btnRestoreZip.Size = new System.Drawing.Size(170, 45);
            this.btnRestoreZip.TabIndex = 1;
            this.btnRestoreZip.Text = "🔄 ZIPから復元";
            this.btnRestoreZip.UseVisualStyleBackColor = false;
            this.btnRestoreZip.Click += new System.EventHandler(this.btnRestoreZip_Click);

            // 
            // DataSyncForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 535);
            this.Controls.Add(this.groupBoxBackup);
            this.Controls.Add(this.groupBoxImport);
            this.Controls.Add(this.groupBoxExport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataSyncForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "データ同期・バックアップ管理";

            this.groupBoxExport.ResumeLayout(false);
            this.groupBoxExport.PerformLayout();
            this.groupBoxImport.ResumeLayout(false);
            this.groupBoxBackup.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxExport;
        private System.Windows.Forms.Button btnExportMultiCsv;
        private System.Windows.Forms.Label lblTilde;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.CheckBox chkUseDate;
        private System.Windows.Forms.TextBox txtKeyword;
        private System.Windows.Forms.Label lblKeyword;

        private System.Windows.Forms.GroupBox groupBoxImport;
        private System.Windows.Forms.Button btnImportCsv;
        private System.Windows.Forms.Button btnImportProfile;
        private System.Windows.Forms.Button btnImportFinancial;
        private System.Windows.Forms.Button btnImportValuations;
        private System.Windows.Forms.Button btnImportAttachments;

        private System.Windows.Forms.GroupBox groupBoxBackup;
        private System.Windows.Forms.Button btnRestoreZip;
        private System.Windows.Forms.Button btnBackupZip;
    }
}