using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Endpoints.Helper;
using Gringotts.Api.Shared.Extensions;

namespace Gringotts.Api.Features.User.Endpoints;

/// <summary>
/// Endpoint for deleting a user.
/// </summary>
/// <remarks>
/// This endpoint allows the deletion of a user by their unique identifier.
/// The request must be authenticated, and the user must have the appropriate permissions.
/// </remarks>
/// <response code="200">Returns the deleted user's information if the operation is successful.</response>
/// <response code="404">Returns a not found error if the user does not exist.</response>
public class DeleteUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("users/{id:guid}",
                (Guid id, AppDbContext dbContext) =>
                    EndpointHelpers.DeleteEntity<Models.User, DeleteUserResponse>(id, dbContext, entity =>
                        new DeleteUserResponse(
                            entity.Id, entity.CardId, entity.SchoolId, entity.FirstName, entity.MiddleName,
                            entity.LastName)
                    )
            )
            .WithAuthenticationFilter()
            .Produces<DeleteUserResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    public record DeleteUserResponse(
        Guid Guid,
        string CardId,
        string SchoolId,
        string FirstName,
        string? MiddleName,
        string LastName
    );
}