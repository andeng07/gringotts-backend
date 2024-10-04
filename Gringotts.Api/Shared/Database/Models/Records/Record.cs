namespace Gringotts.Api.Shared.Database.Models.Records
{
    public class Record
    {
        public required Guid Id { get; init; }
        public required Guid ReaderId { get; init; }
        public required Guid UserId { get; init; }
        public required DateTime Time { get; init; }
        public required ActionType ActionType { get; init; }
    }
}