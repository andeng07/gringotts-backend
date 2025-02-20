using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.User.Models;

public record Department : IEntity
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Name { get; set; }
} 