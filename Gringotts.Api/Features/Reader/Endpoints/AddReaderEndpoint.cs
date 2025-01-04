using FluentValidation;
using Gringotts.Api.Features.Authentication.Services;
using Gringotts.Api.Features.Reader.Models;
using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Endpoints.Helper;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Extensions;
using Gringotts.Api.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Reader.Endpoints;

/// <summary>
/// Endpoint for adding a new RFID reader to the system.
/// </summary>
/// <remarks>
/// This endpoint allows registering a new RFID reader by providing its name and associated location ID. 
/// If the reader is successfully created, it returns a 201 Created status with the reader details. 
/// If the reader creation fails, it returns a 500 Internal Server Error with error details.
/// </remarks>
/// <response code="201">Returns the created reader details when the reader is successfully added.</response>
/// <response code="500">Returns error details if the reader creation fails, such as an invalid location or other internal errors.</response>
public class AddReaderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("readers",
                async ([FromBody] AddReaderRequest request, AppDbContext context, JwtService jwtService) =>
                {
                    var newId = Guid.NewGuid();
                    
                    await EndpointHelpers.CreateEntity<Models.Reader, AddReaderResponse>(
                        new Models.Reader
                        {
                            Id = newId,
                            Name = request.Name,
                            LocationId = request.Location,
                            AccessToken = jwtService.GenerateToken(newId)
                        },
                        context,
                        uri: reader => $"readers/{reader.Id}",
                        responseMapper: reader => new AddReaderResponse(reader.Id, reader.Name, reader.LocationId)
                    );
                }
            )
            .WithRequestValidation<AddReaderRequest>()
            .WithEntityExistenceFilter<Location, AddReaderRequest>(request => request.Location)
            .WithAuthenticationFilter()
            .Produces<AddReaderResponse>(StatusCodes.Status201Created)
            .Produces<Error>(StatusCodes.Status404NotFound)
            .WithGroupName("Readers");
    }

    public class RegisterReaderRequestValidator : AbstractValidator<AddReaderRequest>
    {
        public RegisterReaderRequestValidator()
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

    public record AddReaderRequest(string Name, Guid Location);

    public record AddReaderResponse(Guid Id, string Name, Guid LocationId);
}