namespace MAItems.Database
{
    /// <summary>
    /// 財務ハイライト（1案件につき最大6件）
    /// PeriodType: "actual"=実績 / "forecast"=予測
    /// PeriodOrder: 1〜3（古い順）
    /// </summary>
    public class FinancialHighlight
    {
        public long Id { get; set; }
        public long DealId { get; set; }
        public string PeriodType { get; set; } = string.Empty;
        public int PeriodOrder { get; set; }
        public string PeriodLabel { get; set; } = string.Empty;

        // ── PL項目 ──────────────────────────────────
        public double? Revenue { get; set; }
        public double? CostRate { get; set; }
        public double? GrossProfit { get; set; }
        public double? GrossProfitRate { get; set; }
        public double? SGA { get; set; }
        public double? OperatingProfit { get; set; }
        public double? OperatingProfitRate { get; set; }
        public double? OrdinaryProfit { get; set; }
        public double? NetIncome { get; set; }
        public double? EBITDA { get; set; }
        public double? Depreciation { get; set; }
        public double? CapEx { get; set; }

        // ── BS項目 ──────────────────────────────────
        public double? CurrentAssets { get; set; }
        public double? CashEquivalents { get; set; }
        public double? AccountsReceivable { get; set; }
        public double? Inventory { get; set; }
        public double? OtherCurrentAssets { get; set; }
        public double? FixedAssets { get; set; }
        public double? TotalAssets { get; set; }
        public double? CurrentLiabilities { get; set; }
        public double? AccountsPayable { get; set; }
        public double? ShortTermDebt { get; set; }
        public double? OtherCurrentLiabilities { get; set; }
        public double? FixedLiabilities { get; set; }
        public double? LongTermDebt { get; set; }
        public double? OtherFixedLiabilities { get; set; }
        public double? TotalLiabilities { get; set; }
        public double? NetAssets { get; set; }
        public double? RetainedEarnings { get; set; }
    }
}