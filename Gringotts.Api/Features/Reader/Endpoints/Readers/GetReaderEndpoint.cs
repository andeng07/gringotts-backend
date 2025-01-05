using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.Reader.Endpoints.Readers;

/// <summary>
/// Endpoint for retrieving details of an RFID reader by its ID.
/// </summary>
/// <remarks>
/// Fetches the RFID reader details using the provided ID. 
/// If successful, returns a 200 OK status with the reader's details. 
/// If the reader is not found, returns a 404 Not Found error.
/// </remarks>
/// <response code="200">Returns the details of the specified RFID reader.</response>
/// <response code="404">Returns an error if the RFID reader with the specified ID does not exist.</response>
public class GetReaderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/readers/{id:guid}",
                async (Guid id, AppDbContext dbContext) =>
                    await EndpointHelpers.GetEntity<Models.Reader, GetReaderResponse>(
                        id,
                        dbContext,
                        responseMapper: reader => new GetReaderResponse(
                            reader.Id, reader.Name, reader.LocationId
                        )
                    )
            )
            .WithAuthenticationFilter()
            .Produces<GetReaderResponse>()
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    public record GetReaderResponse(
        Guid Id,
        string Name,
        Guid Location
    );
}