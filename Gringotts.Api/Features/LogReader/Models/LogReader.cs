using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.LogReader.Models
{
    public record LogReader : IEntity
    {
        public required Guid Id { get; init; }
        public required string Name { get; set; }
        public required Guid LocationId { get; set; }
        public required string AccessToken { get; set; }
    }
}