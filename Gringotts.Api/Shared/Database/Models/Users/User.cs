namespace Gringotts.Api.Shared.Database.Models.Users
{
    public record User
    {
        public required Guid Id { get; init; }
        public required string SchoolId { get; set; }
        public required string FirstName { get; set; }
        public required string? MiddleName { get; set; }
        public required string LastName { get; set; }
    }
}