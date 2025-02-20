using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Reader.Models;

/// <summary>
/// Represents an RFID reader registered in the system.
/// </summary>
/// <remarks>
/// An RFID reader is a device that scans RFID-embedded cards to log interactions.  
/// Each reader is uniquely identified and associated with a specific location.
/// </remarks>
public record Reader : IEntity
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Name { get; set; }
    public required Guid? LocationId { get; set; }
}