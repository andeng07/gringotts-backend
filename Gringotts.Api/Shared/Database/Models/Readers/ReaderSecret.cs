namespace Gringotts.Api.Shared.Database.Models.Readers
{
    public record ReaderSecret
    {
        public required Guid Id { get; init; }
        public required Guid ReaderId { get; set; }
        public required string AccessToken { get; set; }
    }
}