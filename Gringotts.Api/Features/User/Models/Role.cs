using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.User.Models
{
    public record Role : IEntity
    {
        public required Guid Id { get; init; }
        public required string Name { get; set; }
        public required int Permission { get; set; }
    }
}