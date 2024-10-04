namespace Gringotts.Api.Shared.Database.Models.Readers
{
    public record ReaderAnalytics
    {
        public required Guid Id { get; init; }
        public required Guid ReaderId { get; set; }
    }
}