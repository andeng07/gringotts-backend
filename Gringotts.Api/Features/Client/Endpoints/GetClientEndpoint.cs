using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.Client.Endpoints;

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