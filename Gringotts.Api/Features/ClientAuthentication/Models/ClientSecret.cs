using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.ClientAuthentication.Models;

/// <summary>
/// Represents a client's authentication credentials.
/// </summary>
/// <remarks>
/// This entity stores the client's login details, including a username and password.
/// It is linked to a specific management user for authentication purposes.
/// </remarks>
public record ClientSecret : IEntity
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required Guid ClientId { get; init; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}