namespace MAItems.MailParser
{
    /// <summary>
    /// メール本文から抽出した案件情報の一時格納モデル。
    /// null は「メール内に記載なし」を意味する。
    /// </summary>
    public class ParsedDeal
    {
        public string? InputDate { get; set; }
        public string? BrokerCompany { get; set; }
        public string? Route { get; set; }
        public string? Title { get; set; }
        public string? DealId { get; set; }
        public string? BusinessContent { get; set; }
        public string? Area { get; set; }
        public string? Revenue { get; set; }
        public string? OperatingProfit { get; set; }
        public string? EBITDA { get; set; }
        public string? NetAssets { get; set; }
        public string? TotalAssets { get; set; }
        public string? NetCashDebt { get; set; }
        public string? CashEquivalents { get; set; }
        public string? InterestBearingDebt { get; set; }
        public string? EmployeeCount { get; set; }
        public string? Features { get; set; }
        public string? AskingPrice { get; set; }
        public string? TransferType { get; set; }
        public string? TransferReason { get; set; }
        public string? TransferConditions { get; set; }
        public string? Status { get; set; }
    }
}