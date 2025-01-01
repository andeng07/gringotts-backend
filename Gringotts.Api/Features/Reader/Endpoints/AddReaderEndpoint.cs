using FluentValidation;
using Gringotts.Api.Features.Reader.Models;
using Gringotts.Api.Features.Reader.Services;
using Gringotts.Api.Shared.Endpoints;
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
        app.MapPost("readers", HandleAsync)
            .WithRequestValidation<AddReaderRequest>()
            .WithEntityExistenceFilter<Location, AddReaderRequest>(x => x.Location)
            .WithAuthenticationFilter()
            .Produces<AddReaderResponse>(StatusCodes.Status201Created)
            .Produces<List<Error>>(StatusCodes.Status500InternalServerError);
    }

    public async Task<IResult> HandleAsync([FromBody] AddReaderRequest request, ReaderService service)
    {
        var createReaderResult = await service.CreateReaderAsync(
            request.Name, request.Location);

        if (!createReaderResult.IsSuccess)
        {
            return Results.Json(createReaderResult.Errors, statusCode: StatusCodes.Status500InternalServerError);
        }
        
        var reader = createReaderResult.Value!;

        var response = new AddReaderResponse(reader.Id, reader.Name, reader.LocationId);
        
        return Results.Created($"readers/{reader.Id}", response);
    }
    
    public class RegisterReaderRequestValidator : AbstractValidator<AddReaderRequest>
    {
        public RegisterReaderRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Name is required").WithErrorCode(ReaderErrorCodes.NameRequired)
                .NotEmpty().WithMessage("Name is required").WithErrorCode(ReaderErrorCodes.NameRequired)
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters").WithErrorCode(ReaderErrorCodes.NameTooLong);
        }
    }
    
    public record AddReaderRequest(string Name, Guid Location);

    public record AddReaderResponse(Guid Id, string Name, Guid LocationId);
}