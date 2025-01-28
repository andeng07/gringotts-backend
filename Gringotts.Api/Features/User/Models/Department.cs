using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.User.Models;

public record Department : IEntity
{
    public required Guid Id { get; init; }
    public string Name { get; set; }
}