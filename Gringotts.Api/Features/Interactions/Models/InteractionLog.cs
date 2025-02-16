using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Interactions.Models;

/// <summary>
/// Represents an interaction log entry when a user interacts with an RFID reader.
/// </summary>
/// <remarks>
/// The <see cref="InteractionLog"/> entity stores details about RFID interactions,  
/// including the reader, user, card ID, and transaction type.
/// </remarks>
public record InteractionLog : IEntity
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required Guid LogReaderId { get; init; }
    public required Guid? LogUserId { get; init; } 
    public required string CardId { get; init; }
    public required DateTime DateTime { get; init; }
    public required InteractionType InteractionType { get; init; }
}

public enum InteractionType : byte
{
    Entry = 1, Exit = 2, Unauthorized = 3, Fallback = 4
}