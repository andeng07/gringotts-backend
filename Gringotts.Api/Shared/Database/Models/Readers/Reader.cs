namespace Gringotts.Api.Shared.Database.Models.Readers
{
    public record Reader
    {
        public required int Id { get; init; }
        public required string Name { get; set; }
        public required Guid LocationId { get; set; }
    }
}