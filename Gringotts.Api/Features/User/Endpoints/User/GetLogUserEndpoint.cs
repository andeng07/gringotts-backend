using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.User.Endpoints.User;

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
                (Guid id, AppDbContext dbContext) => EndpointHelpers.GetEntity<Models.User, GetLogUserResponse>(id,
                    dbContext,
                    responseMapper: user => new GetLogUserResponse(user.Id, user.AccessExpiry, user.CardId,
                        user.SchoolId, user.FirstName,
                        user.MiddleName, user.LastName, user.Affiliation, user.Sex, user.DepartmentId)
                )
            )
            .WithAuthenticationFilter();
    }

    public record GetLogUserResponse(
        Guid Id,
        DateTime AccessExpiry,
        string CardId,
        string SchoolId,
        string FirstName,
        string? MiddleName,
        string LastName,
        byte Affiliation,
        byte Sex,
        Guid? DepartmentId
    );
}