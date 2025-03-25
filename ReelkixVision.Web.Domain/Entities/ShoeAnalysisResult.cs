namespace ReelkixVision.Web.Domain.Entities
{
    public class ShoeAnalysisResult
    {
        public int Id { get; set; } // Primary Key
        public int RequestLogId { get; set; } // Foreign Key to RequestLogs
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Colorway { get; set; }
        public string? Sku { get; set; }
        public int? Confidence { get; set; }
        public string? Text { get; set; }

        // Navigation Property
        public RequestLog? RequestLog { get; set; }
    }
}