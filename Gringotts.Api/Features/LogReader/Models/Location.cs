using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.LogReader.Models
{
    public record Location : IEntity
    {
    public Guid Id { get; init; }
    public required string BuildingName { get; set; }
    public required string? RoomName { get; set; }
    }
}