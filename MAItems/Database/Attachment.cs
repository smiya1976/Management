namespace MAItems.Database
{
    public class Attachment
    {
        public long Id { get; set; }
        public long DealId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UploadedAt { get; set; } = string.Empty;
    }
}