using Gringotts.Api.Shared.Database.Models.Records;

namespace Gringotts.Api.Shared.Database.Models.Users
{
    public record User
    {
        public required Guid Id { get; init; }
        public required string SchoolId { get; set; }
        public required string FirstName { get; set; }
        public required string? MiddleName { get; set; } = null;
        public required string LastName { get; set; }
    }
}