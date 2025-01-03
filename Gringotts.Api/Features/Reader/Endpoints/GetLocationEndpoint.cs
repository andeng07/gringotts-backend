using Gringotts.Api.Features.Reader.Models;
using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Endpoints.Helper;
using Gringotts.Api.Shared.Extensions;
using Gringotts.Api.Shared.Results;

namespace Gringotts.Api.Features.Reader.Endpoints;

/// <summary>
/// Represents the endpoint for retrieving the location details of an RFID reader.
/// </summary>
/// <remarks>
/// This endpoint fetches the location details (such as building and room name) 
/// of a specific RFID reader identified by its unique ID. 
/// It includes authentication and produces appropriate responses:
/// - Success: Returns the location details of the specified RFID reader.
/// - Not Found: Returns an error if the specified reader ID does not exist.
/// </remarks>
public class GetLocationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("locations/{id:guid}", async (Guid id, AppDbContext dbContext) => await
                EndpointHelpers.GetEntity<Location, GetLocationResponse>(
                    id,
                    dbContext,
                    responseMapper: location =>
                        new GetLocationResponse(location.Id, location.BuildingName, location.RoomName)
                )
            )
            .WithAuthenticationFilter()
            .Produces<GetLocationResponse>()
            .Produces<Error>(StatusCodes.Status404NotFound);
    }

    public record GetLocationResponse(Guid Id, string BuildingName, string? RoomName);
}