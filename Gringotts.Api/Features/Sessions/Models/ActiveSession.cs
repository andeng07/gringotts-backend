using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Sessions.Models;

public record ActiveSession : IEntity 
{
    public Guid Id { get; init; }
    public Guid LogUserId { get; init; }
    public Guid LogReaderId { get; init; }
    public Guid SessionId { get; init; }
}