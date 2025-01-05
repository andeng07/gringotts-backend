using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Statistics.Models
{
    public record ReaderAnalytics : IEntity
    {
        public required Guid Id { get; init; }
        public required Guid ReaderId { get; set; }
    }
}