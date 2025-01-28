using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.Client.Endpoints;

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