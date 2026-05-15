namespace MAItems.Database
{
    public class Deal
    {
        public long Id { get; set; }
        public string InputDate { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string BrokerCompany { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string DealId { get; set; } = string.Empty;
        public string BusinessContent { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Revenue { get; set; } = string.Empty;
        public string OperatingProfit { get; set; } = string.Empty;
        public string EBITDA { get; set; } = string.Empty;
        public string NetAssets { get; set; } = string.Empty;
        public string TotalAssets { get; set; } = string.Empty;
        public string NetCashDebt { get; set; } = string.Empty;
        public string CashEquivalents { get; set; } = string.Empty;
        public string InterestBearingDebt { get; set; } = string.Empty;
        public string EmployeeCount { get; set; } = string.Empty;
        public string Features { get; set; } = string.Empty;
        public string AskingPrice { get; set; } = string.Empty;
        public string TransferType { get; set; } = string.Empty;
        public string TransferReason { get; set; } = string.Empty;
        public string TransferConditions { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}