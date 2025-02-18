using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.Client.Endpoints;

/// <summary>
/// Retrieves client details by their unique identifier.
/// </summary>
/// <remarks>
/// This endpoint allows authenticated users to fetch information about a client 
/// using their GUID. If the client exists, their details are returned. If the client 
/// is not found, a 404 Not Found response is returned.
/// </remarks>
/// <response code="200">Returns the client's details.</response>
/// <response code="404">If no client with the specified ID exists.</response>

public class GetClientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/clients/{id:guid}",
                async (Guid id, AppDbContext dbContext) =>
                    await EndpointHelpers.GetEntity<Models.Client, GetClientResponse>(
                        id, dbContext, responseMapper: client =>
                            new GetClientResponse(client.Id, client.FirstName, client.MiddleName, client.LastName)
                    )
            )
            .WithAuthenticationFilter()
            .Produces<GetClientResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    public record GetClientResponse(Guid Id, string FirstName, string? MiddleName, string LastName);
}