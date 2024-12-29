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
            .WithAuthenticationFilter();
    }

    private record GetUserRequest(Guid Id);

    private record GetUserResponse(
        Guid Id,
        string CardId,
        string SchoolId,
        string FirstName,
        string? MiddleName,
        string LastName
    );

    // TODO: Figure out how to validate ownership
    private async Task<IResult> HandleAsync(HttpContext httpContext, [FromRoute] Guid id, UserService service)
    {
        var user = await service.GetUserByIdAsync(id);

        if (user == null)
        {
            return Results.NotFound();
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