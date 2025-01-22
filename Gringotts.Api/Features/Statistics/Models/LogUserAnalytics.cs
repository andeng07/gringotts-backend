using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Statistics.Models
{
    public record LogUserAnalytics : IEntity
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; set; }
    }
}