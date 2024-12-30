using System.Security.Claims;
using Gringotts.Api.Features.Auth.Services;
using Gringotts.Api.Features.User.Services;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Extensions;
using Gringotts.Api.Shared.Results;
using Microsoft.AspNetCore.Mvc;

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
        app.MapGet("/users/{id:guid}", HandleAsync)
            .WithAuthenticationFilter()
            .Produces<GetUserResponse>()
            .Produces<List<Error>>(StatusCodes.Status401Unauthorized)
            .Produces<List<Error>>(StatusCodes.Status403Forbidden)
            .Produces<List<Error>>(StatusCodes.Status404NotFound);
    }

    private record GetUserResponse(
        Guid Id,
        string CardId,
        string SchoolId,
        string FirstName,
        string? MiddleName,
        string LastName
    );

    private async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromRoute] Guid id,
        UserService service,
        JwtService jwtService)
    {
        var subjectIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Unauthorized if there's no subject in the JWT
        if (string.IsNullOrEmpty(subjectIdString))
        {
            var errors = new List<Error>
            {
                new(
                    AuthErrorCodes.TokenNotFound,
                    "Authentication token not found in the request.",
                    Error.ErrorType.Validation
                )
            };

            return Results.Json(errors, statusCode: StatusCodes.Status401Unauthorized);
        }

        // Invalid token subject format
        if (!Guid.TryParse(subjectIdString, out var subjectId))
        {
            return Results.BadRequest(new List<Error>
            {
                new(
                    AuthErrorCodes.InvalidTokenFormat,
                    "Invalid token subject format.",
                    Error.ErrorType.Validation
                )
            });
        }

        // Forbidden if the user tries to access another user's data
        if (subjectId != id)
        {
            var errors = new List<Error>
            {
                new(
                    AuthErrorCodes.AccessDenied,
                    "You are not authorized to access this user's data.",
                    Error.ErrorType.AccessUnauthorized
                )
            };
            return Results.Json(errors, statusCode: StatusCodes.Status403Forbidden);
        }

        var user = await service.GetUserByIdAsync(id);
        if (user == null)
        {
            var errors = new List<Error>
            {
                new(UserErrorCodes.UserNotFound, "User not found.", Error.ErrorType.NotFound)
            };
            return Results.NotFound(errors);
        }

        var response = new GetUserResponse(
            user.Id,
            user.CardId,
            user.SchoolId,
            user.FirstName,
            user.MiddleName,
            user.LastName
        );

        return Results.Ok(response);
    }
}