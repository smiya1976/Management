namespace MAItems.Database
{
    public class ValuationData
    {
        public long Id { get; set; }
        public long DealId { get; set; }

        // ── 純資産法 ──────────────────────────────
        public double? NetAssetValue { get; set; }
        public string NetAssetNote { get; set; } = string.Empty;

        // ── EBITDAマルチプル ──────────────────────
        public double? EBITDABase { get; set; }
        public string EBITDABaseYear { get; set; } = string.Empty;
        public double? EBITDAMultiple { get; set; }
        public double? EBITDANetCashDebt { get; set; }
        public string EBITDANote { get; set; } = string.Empty;

        // ── DCF法 ─────────────────────────────────
        public double? DCFDiscountRate { get; set; }
        public double? DCFTerminalGrowth { get; set; }
        public double? DCFEV { get; set; }
        public double? DCFNetCashDebt { get; set; }
        public string DCFNote { get; set; } = string.Empty;

        // ── 直接還元法 ────────────────────────────
        public double? NOI { get; set; }
        public double? CapRate { get; set; }
        public double? DirectNetCashDebt { get; set; }
        public string DirectNote { get; set; } = string.Empty;

        // ── 計算結果（自動算出・保存） ────────────
        public double? EBITDAEquityValue { get; set; }
        public double? DCFEquityValue { get; set; }
        public double? DirectEquityValue { get; set; }
        public string ValuationNote { get; set; } = string.Empty;
    }
}