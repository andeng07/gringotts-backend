using FluentValidation;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Reader.Endpoints.Readers;

/// <summary>
/// Endpoint for updating details of an existing RFID reader.
/// </summary>
/// <remarks>
/// Updates the name and location of the specified RFID reader by its ID. 
/// If successful, returns a 200 OK status with the updated reader details. 
/// If the reader is not found, returns a 404 Not Found error.
/// </remarks>
/// <response code="200">Returns the updated details of the RFID reader.</response>
/// <response code="404">Returns an error if the RFID reader with the specified ID does not exist.</response>
public class UpdateReaderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/readers/{id:guid}",
                async ([FromBody] UpdateReaderRequest request, Guid id, AppDbContext dbContext) =>
                await EndpointHelpers.UpdateEntity<Models.Reader, UpdateReaderResponse>(
                    id,
                    dbContext,
                    updateEntity: reader =>
                    {
                        reader.Name = request.Name;
                        reader.LocationId = request.Location;
                    },
                    responseMapper: reader => 
                        new UpdateReaderResponse(reader.Id, reader.Name, reader.LocationId)
                )
            )
            .WithAuthenticationFilter()
            .WithRequestValidation<UpdateReaderRequest>()
            .WithEntityExistenceFromRouteFilter<Models.Reader>("id")
            .Produces<UpdateReaderResponse>()
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    public class UpdateReaderRequestValidator : AbstractValidator<UpdateReaderRequest>
    {
        public UpdateReaderRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage("Name is required")
                    .WithErrorCode(ReaderErrorCodes.NameRequired)
                .MaximumLength(50)
                    .WithMessage("Name must not exceed 50 characters")
                    .WithErrorCode(ReaderErrorCodes.NameTooLong);
        }
    }

    public record UpdateReaderRequest(string Name, Guid Location);

    public record UpdateReaderResponse(Guid Id, string Name, Guid? Location);
}