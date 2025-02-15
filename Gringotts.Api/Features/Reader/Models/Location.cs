using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Reader.Models;

/// <summary>
/// Represents a physical location where an RFID reader is placed.
/// </summary>
public record Location : IEntity
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string BuildingName { get; set; }
    public required string? RoomName { get; set; }
}