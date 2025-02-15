using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.Client.Endpoints;

/// <summary>
/// Handles the deletion of a client by their unique identifier.
/// </summary>
/// <remarks>
/// This endpoint allows authenticated users to delete a client using their GUID.
/// If the client exists, they are removed from the database, and their details 
/// are returned in the response. If the client does not exist, a 404 Not Found 
/// response is returned.
/// </remarks>
/// <response code="200">Returns the deleted client's details.</response>
/// <response code="404">If no client with the specified ID exists.</response>
public class DeleteClientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("clients/{id:guid}",
            async (Guid id, AppDbContext dbContext) =>
                await EndpointHelpers.DeleteEntity<Models.Client, DeleteClientResponse>(
                    id, dbContext, responseMapper: client =>
                        new DeleteClientResponse(client.Id, client.FirstName, client.MiddleName, client.LastName)
                )
        )
        .WithAuthenticationFilter()
        .Produces<DeleteClientResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }

    public record DeleteClientResponse(Guid Id, string FirstName, string? MiddleName, string LastName);
}