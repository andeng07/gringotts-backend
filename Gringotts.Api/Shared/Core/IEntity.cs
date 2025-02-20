namespace Gringotts.Api.Shared.Core;

public interface IEntity
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}