namespace Gringotts.Api.Shared.Database.Models.Users
{
    public record Role
    {
        public required Guid Id { get; init; }
        public required string Name { get; set; }
        public required int Permission { get; set; }
    }
}