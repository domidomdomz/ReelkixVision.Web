using ReelkixVision.Web.Domain.Entities;

public class RequestLog
{
    public int Id { get; set; }
    public string? FileName { get; set; }
    public string? Url { get; set; }
    public DateTime RequestTime { get; set; }

    // Navigation Property
    public ShoeAnalysisResult? ShoeAnalysisResult { get; set; }
}