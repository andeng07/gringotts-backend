namespace Gringotts.Api.Features.Statistics.Models
{
    public record UserAnalytics
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; set; }
    }
}