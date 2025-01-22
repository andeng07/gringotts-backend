using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.LogUser.Endpoints;

/// <summary>
/// Endpoint for retrieving user information.
/// </summary>
/// <remarks>
/// This endpoint allows users to retrieve their personal information based on their user ID.
/// The request must be authenticated, and the user must be authorized to access their own data.
/// </remarks>
/// <response code="200">Returns the user's information if found.</response>
/// <response code="401">Returns an unauthorized error if the user is not authenticated.</response>
/// <response code="403">Returns a forbidden error if the user attempts to access another user's data.</response>
/// <response code="404">Returns a not found error if the user is not found.</response>
public class GetLogUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{id:guid}",
                async (Guid id, AppDbContext dbContext) =>
                    await EndpointHelpers.GetEntity<Models.LogUser, GetLogUserResponse>(
                        id,
                        dbContext,
                        logUser => new GetLogUserResponse(
                            logUser.Id,
                            logUser.CardId,
                            logUser.SchoolId,
                            logUser.FirstName,
                            logUser.MiddleName,
                            logUser.LastName,
                            logUser.Sex
                        )
                    )
            )
            .WithAuthenticationFilter()
            .WithEntityOwnershipFromRouteFilter("id")
            .Produces<GetLogUserResponse>()
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    private record GetLogUserResponse(
        Guid Id,
        string CardId,
        string SchoolId,
        string FirstName,
        string? MiddleName,
        string LastName,
        byte Sex
    );
}