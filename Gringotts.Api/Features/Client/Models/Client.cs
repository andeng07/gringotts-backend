using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Client.Models;

public record Client : IEntity
{
    public required Guid Id { get; init; }
    public required string FirstName { get; set; }
    public required string? MiddleName { get; set; }
    public required string LastName { get; set; }
}