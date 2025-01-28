using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Sessions.Models;

public record SessionLog : IEntity
{
    public required Guid Id { get; init; }
    public required string TransactionId { get; init; }
    public required Guid LogReaderId { get; init; }
    public required Guid? LogUserId { get; init; } 
    public required string CardId { get; init; }
    public required DateTime DateTime { get; init; }
    public required Type Type { get; init; }
}

public enum Type : byte
{
    Entry = 1, Exit = 2, Fallback = 3
}