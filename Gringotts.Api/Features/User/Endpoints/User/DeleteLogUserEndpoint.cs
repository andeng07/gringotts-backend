using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.User.Endpoints.User;

/// <summary>
/// Endpoint for deleting a user.
/// </summary>
/// <remarks>
/// This endpoint allows the deletion of a user by their unique identifier.
/// The request must be authenticated, and the user must have the appropriate permissions.
/// </remarks>
/// <response code="200">Returns the deleted user's information if the operation is successful.</response>
/// <response code="404">Returns a not found error if the user does not exist.</response>
public class DeleteLogUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("users/{id:guid}",
                async (Guid id, AppDbContext dbContext) =>
                    await EndpointHelpers.DeleteEntity<Models.User, DeleteLogUserResponse>(
                        id,
                        dbContext,
                        entity => new DeleteLogUserResponse(
                            entity.Id, entity.AccessExpiry, entity.CardId, entity.SchoolId, entity.FirstName,
                            entity.MiddleName, entity.LastName, entity.Affiliation, entity.Sex, entity.DepartmentId
                        )
                    )
            )
            .WithAuthenticationFilter()
            .Produces<DeleteLogUserResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    public record DeleteLogUserResponse(
        Guid Id,
        DateTime AccessExpiry,
        string CardId,
        string SchoolId,
        string FirstName,
        string? MiddleName,
        string LastName,
        byte Affiliation,
        byte Sex,
        Guid? Department
    );
}