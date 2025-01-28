using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Sessions.Models;

public record Session : IEntity
{
    public required Guid Id { get; init; }
    public required Guid LogReaderId { get; init; }
    public required Guid LogUserId { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime? EndDate { get; set; }
}