namespace Gringotts.Api.Features.User.Models
{
    public record Role
    {
        public required Guid Id { get; init; }
        public required string Name { get; set; }
        public required int Permission { get; set; }
    }
}