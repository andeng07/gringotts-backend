using Gringotts.Api.Shared.Models;

namespace Gringotts.Api.Features.Auth.Models
{
    public record UserSecret : IEntity
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; init; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}