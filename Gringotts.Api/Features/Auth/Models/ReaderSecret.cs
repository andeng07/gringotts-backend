using Gringotts.Api.Shared.Models;

namespace Gringotts.Api.Features.Auth.Models
{
    public record ReaderSecret : IEntity
    {
        public required Guid Id { get; init; }
        public required Guid ReaderId { get; set; }
        public required string AccessToken { get; set; }
    }
}