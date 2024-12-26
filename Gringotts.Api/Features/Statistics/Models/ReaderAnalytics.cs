namespace Gringotts.Api.Features.Statistics.Models
{
    public record ReaderAnalytics
    {
        public required Guid Id { get; init; }
        public required Guid ReaderId { get; set; }
    }
}