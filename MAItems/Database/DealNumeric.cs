namespace MAItems.Database
{
    public class DealNumeric
    {
        public long Id { get; set; }
        public string InputDate { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string BrokerCompany { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string DealId { get; set; } = string.Empty;
        public string BusinessContent { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public double? Revenue { get; set; }
        public double? OperatingProfit { get; set; }
        public double? EBITDA { get; set; }
        public double? NetAssets { get; set; }
        public double? TotalAssets { get; set; }
        public double? NetCashDebt { get; set; }
        public double? CashEquivalents { get; set; }
        public double? InterestBearingDebt { get; set; }
        public double? EmployeeCount { get; set; }
        public string Features { get; set; } = string.Empty;
        public double? AskingPrice { get; set; }
        public string TransferType { get; set; } = string.Empty;
        public string TransferReason { get; set; } = string.Empty;
        public string TransferConditions { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ConvertedAt { get; set; } = string.Empty;
    }
}