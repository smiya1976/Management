namespace MAItems.Database
{
    public class CompanyProfile
    {
        public long Id { get; set; }
        public long DealId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyNameSub { get; set; } = string.Empty;
        public string HeadOfficeAddress { get; set; } = string.Empty;
        public string FactoryAddress { get; set; } = string.Empty;
        public string OtherOffice { get; set; } = string.Empty;
        public string Founded { get; set; } = string.Empty;
        public string Founded2 { get; set; } = string.Empty;
        public string Capital { get; set; } = string.Empty;
        public string RepresentativeName { get; set; } = string.Empty;
        public string RepresentativeProfile { get; set; } = string.Empty;
        public string ShareholderInfo { get; set; } = string.Empty;
        public string BusinessDetail { get; set; } = string.Empty;
        public string Revenue { get; set; } = string.Empty;
        public string Employees { get; set; } = string.Empty;
        public string MainClients { get; set; } = string.Empty;
        public string MainSuppliers { get; set; } = string.Empty;
        public string Certifications { get; set; } = string.Empty;
        public string GroupCompanies { get; set; } = string.Empty;
        public string TransferReason { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}