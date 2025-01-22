using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.ManagementAuthentication.Models
{
    public record ManagementUserSecret : IEntity
    {
        public required Guid Id { get; init; }
        public required Guid ManagementUserId { get; init; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}