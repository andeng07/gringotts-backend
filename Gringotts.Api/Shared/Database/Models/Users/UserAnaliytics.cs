namespace Gringotts.Api.Shared.Database.Models.Users
{
    public record UserAnalytics
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; set; }
    }
}