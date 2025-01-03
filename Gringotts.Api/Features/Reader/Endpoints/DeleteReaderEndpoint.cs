using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Endpoints.Helper;
using Gringotts.Api.Shared.Extensions;
using Gringotts.Api.Shared.Results;

namespace Gringotts.Api.Features.Reader.Endpoints;

/// <summary>
/// Endpoint for deleting an existing RFID reader.
/// </summary>
/// <remarks>
/// Deletes the specified RFID reader by ID. 
/// If successful, returns a 200 OK status with the deleted reader details. 
/// If the reader is not found, returns a 404 Not Found error.
/// </remarks>
/// <response code="200">Returns the details of the deleted RFID reader.</response>
/// <response code="404">Returns an error if the RFID reader with the specified ID does not exist.</response>
public class DeleteReaderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("delete/{id:guid}",
                async (Guid id, AppDbContext dbContext) =>
                    await EndpointHelpers.DeleteEntity<Models.Reader, DeleteReaderResponse>(
                        id,
                        dbContext,
                        responseMapper: reader => new DeleteReaderResponse(
                            reader.Id, reader.Name, reader.LocationId
                        )
                    )
            )
            .WithAuthenticationFilter()
            .Produces<DeleteReaderResponse>()
            .Produces<Error>(StatusCodes.Status404NotFound);
    }

    public record DeleteReaderResponse(Guid Id, string ReaderName, Guid Location);
}