﻿using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Endpoints.Helper;
using Gringotts.Api.Shared.Extensions;
using Gringotts.Api.Shared.Results;

namespace Gringotts.Api.Features.User.Endpoints;

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
public class GetUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id:guid}",
                async (Guid id, AppDbContext dbContext) =>
                    await EndpointHelpers.GetEntity<Models.User, GetUserResponse>(
                        id,
                        dbContext,
                        entity => new GetUserResponse(entity.Id, entity.CardId, entity.SchoolId,
                            entity.FirstName, entity.MiddleName, entity.LastName)
                    )
            )
            .WithAuthenticationFilter()
            .WithEntityOwnershipFromRouteFilter("id")
            .Produces<GetUserResponse>()
            .Produces<Error>(StatusCodes.Status404NotFound);
    }

    private record GetUserResponse(
        Guid Id,
        string CardId,
        string SchoolId,
        string FirstName,
        string? MiddleName,
        string LastName
    );

}