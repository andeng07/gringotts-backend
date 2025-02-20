using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Client.Models;

/// <summary>
/// Represents a client entity.
/// </summary>
/// <remarks>
/// This record stores client details, including their unique identifier, creation date, 
/// and name information. The <see cref="Id"/> is immutable, while the name fields 
/// can be updated as needed.
/// </remarks>
public record Client : IEntity
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string FirstName { get; set; }
    public required string? MiddleName { get; set; }
    public required string LastName { get; set; }
}