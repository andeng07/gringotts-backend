using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Interactions.Models;

/// <summary>
/// Represents a session entry where a user interacts with an RFID reader.
/// </summary>
/// <remarks>
/// The <see cref="Session"/> entity stores information about user sessions,  
/// tracking the start and end times of user interactions with RFID readers.
/// </remarks>
public record Session : IEntity
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required Guid LogReaderId { get; init; }
    public required Guid LogUserId { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime? EndDate { get; set; }
}