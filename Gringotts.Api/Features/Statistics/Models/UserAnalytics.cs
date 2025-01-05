using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Statistics.Models
{
    public record UserAnalytics : IEntity
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; set; }
    }
}