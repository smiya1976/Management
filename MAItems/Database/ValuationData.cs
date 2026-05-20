namespace MAItems.Database
{
    public class ValuationData
    {
        public long Id { get; set; }
        public long DealId { get; set; }

        // ── 既存のプロパティ ──
        public double? NetAssetValue { get; set; }
        public string NetAssetNote { get; set; } = string.Empty;

        public double? EBITDABase { get; set; }
        public string EBITDABaseYear { get; set; } = string.Empty;
        public double? EBITDAMultiple { get; set; }
        public double? EBITDANetCashDebt { get; set; }
        public string EBITDANote { get; set; } = string.Empty;

        public double? DCFDiscountRate { get; set; }
        public double? DCFTerminalGrowth { get; set; }
        public double? DCFEV { get; set; }
        public double? DCFNetCashDebt { get; set; }
        public string DCFNote { get; set; } = string.Empty;

        public double? NOI { get; set; }
        public double? CapRate { get; set; }
        public double? DirectNetCashDebt { get; set; }
        public string DirectNote { get; set; } = string.Empty;

        public double? EBITDAEquityValue { get; set; }
        public double? DCFEquityValue { get; set; }
        public double? DirectEquityValue { get; set; }
        public string ValuationNote { get; set; } = string.Empty;

        // ── ★今回追加したプロパティ（現預金・有利子負債など） ──
        public double? CashAndDeposits { get; set; }      // 現金・預金
        public double? MarketableSecurities { get; set; } // 商品有価証券
        public double? InsuranceReserves { get; set; }    // 保険積立金
        public double? OtherAssets { get; set; }          // その他（非事業資産）
        public double? WorkingCapitalMonths { get; set; } // 運転資金月数

        public double? ShortTermDebt { get; set; }        // 短期借入金
        public double? LongTermDebt { get; set; }         // 長期借入金
        public double? LeaseObligations { get; set; }     // リース負債
        public double? OtherLiabilities { get; set; }     // その他有利子負債
    }
    // ── ① 新規追加：純資産法の修正項目（1対多） ──
    public class NetAssetAdjustment
    {
        public long Id { get; set; }
        public long DealId { get; set; }
        public int AdjustType { get; set; } // 1: 資産の修正, 2: 負債の修正
        public string ItemName { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    // ── ② 新規追加：DCF法の将来計画（1対多） ──
    public class DcfProjection
    {
        public long Id { get; set; }
        public long DealId { get; set; }
        public int YearIndex { get; set; } // 0=直近実績, 1〜5=予測, 6=6期以降(ターミナル)
        public double? Revenue { get; set; }
        public double? OpProfit { get; set; }
        public double? TaxRate { get; set; }
        public double? DiscountRate { get; set; }
        public double? TerminalGrowth { get; set; }
    }

    // ── ③ 既存の ValuationData クラスに、全手法で共有する「現預金・有利子負債」のプロパティを追加 ──
    // ※ 既存の ValuationData クラスの中に以下のプロパティを書き足してください。
    /*
        public double? CashAndDeposits { get; set; }      // 現金・預金
        public double? MarketableSecurities { get; set; } // 商品有価証券
        public double? InsuranceReserves { get; set; }    // 保険積立金
        public double? OtherAssets { get; set; }          // その他（非事業資産）
        public double? WorkingCapitalMonths { get; set; } // 運転資金月数（例: 1.5）
        
        public double? ShortTermDebt { get; set; }        // 短期借入金
        public double? LongTermDebt { get; set; }         // 長期借入金
        public double? LeaseObligations { get; set; }     // リース負債
        public double? OtherLiabilities { get; set; }     // その他有利子負債
    */
}