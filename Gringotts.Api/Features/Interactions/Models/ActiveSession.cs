using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Interactions.Models;

/// <summary>
/// Represents an active session where a user interacts with an RFID reader.
/// </summary>
/// <remarks>
/// The <see cref="ActiveSession"/> entity stores information about ongoing sessions,  
/// linking a user to an RFID reader through a session identifier.
/// </remarks>
public record ActiveSession : IEntity 
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required Guid LogUserId { get; init; }
    public required Guid LogReaderId { get; init; }
    public required Guid SessionId { get; init; }
}