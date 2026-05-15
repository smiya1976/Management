using MAItems.Database;
using MAItems.MailParser;
using System.Windows.Forms;  // Clipboard
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAItems
{
    public partial class DetailForm : Form
    {
        private readonly DatabaseHelper _db;
        private readonly Deal _deal;

        public event EventHandler? SaveCompleted;

        public DetailForm(Deal deal, DatabaseHelper db)
        {
            InitializeComponent();
            _deal = deal;
            _db = db;
            LoadDealToForm();
        }

        private void LoadDealToForm()
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            FormToDeal();
            try
            {
                _db.UpdateDeal(_deal);
                SetStatus("✔ 保存しました", isError: false);
                SaveCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                SetStatus($"❌ 保存エラー: {ex.Message}", isError: true);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
            => this.Close();

        private void SetStatus(string msg, bool isError)
        {
            lblStatus.ForeColor = isError ? Color.Red : Color.DarkGreen;
            lblStatus.Text = msg;
        }

        // ─── クリップボードから取込ボタン ─────────────────────
        private void btnPasteFromMail_Click(object sender, EventArgs e)
        {
            // クリップボードからテキスト取得
            string mailBody = Clipboard.GetText();

            if (string.IsNullOrWhiteSpace(mailBody))
            {
                SetStatus("⚠ クリップボードにテキストがありません", isError: true);
                return;
            }

            // パーサーを自動選択
            var parser = MailParserFactory.GetParser(mailBody);

            if (parser == null)
            {
                SetStatus("⚠ 対応する仲介会社のフォーマットが見つかりません",
                    isError: true);
                return;
            }

            // 解析実行
            ParsedDeal parsed = parser.Parse(mailBody);

            // フォームに反映（null の項目は上書きしない）
            ApplyParsedDeal(parsed);

            SetStatus($"✔ メール本文を取り込みました（{parsed.BrokerCompany}）",
                isError: false);
        }

        // ─── ParsedDeal をフォームの各テキストボックスに反映 ───
        private void ApplyParsedDeal(ParsedDeal parsed)
        {
            // null でない場合のみ上書き
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
            if (parsed.InterestBearingDebt != null)
                txtInterestBearingDebt.Text = parsed.InterestBearingDebt;
            if (parsed.EmployeeCount != null) txtEmployeeCount.Text = parsed.EmployeeCount;
            if (parsed.Features != null) txtFeatures.Text = parsed.Features;
            if (parsed.AskingPrice != null) txtAskingPrice.Text = parsed.AskingPrice;
            if (parsed.TransferType != null) txtTransferType.Text = parsed.TransferType;
            if (parsed.TransferReason != null) txtTransferReason.Text = parsed.TransferReason;
            if (parsed.TransferConditions != null)
                txtTransferConditions.Text = parsed.TransferConditions;
            if (parsed.Status != null) txtStatus.Text = parsed.Status;
        }

    }
}