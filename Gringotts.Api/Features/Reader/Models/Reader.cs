namespace Gringotts.Api.Features.Reader.Models
{
    public record Reader
    {
        public required Guid Id { get; init; }
        public required string Name { get; set; }
        public required Guid LocationId { get; set; }
    }
}