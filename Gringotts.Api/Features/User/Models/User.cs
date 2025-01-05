using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.User.Models
{
    public record User : IEntity
    {
        public required Guid Id { get; init; }
        public required string CardId { get; set; }
        public required string SchoolId { get; set; }
        public required string FirstName { get; set; }
        public required string? MiddleName { get; set; }
        public required string LastName { get; set; }
    }
}