namespace Gringotts.Api.Shared.Database.Models.Readers
{
    public record Location
    {
        public required Guid Id { get; init; }
        public required string BuildingName { get; set; }
        public required string RoomName { get; set; }
    }
}