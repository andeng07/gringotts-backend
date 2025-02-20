using Gringotts.Api.Features.Reader.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.Reader.Endpoints.Locations;

/// <summary>
/// Represents the endpoint for deleting the location details of an RFID reader.
/// </summary>
/// <remarks>
/// This endpoint deletes the location details (such as building and room name) 
/// of a specific RFID reader identified by its unique ID. 
/// It includes authentication and produces appropriate responses:
/// - Success: Returns the details of the deleted location.
/// - Not Found: Returns an error if the specified reader ID does not exist.
/// </remarks>
public class DeleteLocationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/locations/{id:guid}", async (Guid id, AppDbContext dbContext) =>
                await EndpointHelpers.DeleteEntity<Location, DeleteLocationResponse>(
                    id,
                    dbContext,
                    responseMapper: entity =>
                        new DeleteLocationResponse(entity.Id, entity.BuildingName, entity.RoomName))
            )
            .WithAuthenticationFilter()
            .Produces<DeleteLocationResponse>()
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    public record DeleteLocationResponse(Guid Id, string BuildingName, string? RoomName);
}