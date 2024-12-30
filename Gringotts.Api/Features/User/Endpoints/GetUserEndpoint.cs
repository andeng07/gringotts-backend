using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Gringotts.Api.Features.Auth.Services;
using Gringotts.Api.Features.User.Services;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.User.Endpoints;

public class GetUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/{id:guid}", HandleAsync)
            .WithAuthenticationFilter()
            .Produces<GetUserResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
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
        var subjectIdString = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(subjectIdString))
        {
            return Results.Unauthorized();
        }

        if (!Guid.TryParse(subjectIdString, out var subjectId))
        {
            return Results.BadRequest(new { Message = "Invalid token subject format." });
        }

        if (subjectId != id)
        {
            return Results.Forbid();
        }

        var user = await service.GetUserByIdAsync(id);
        if (user == null)
        {
            return Results.NotFound(new { Message = "User not found." });
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