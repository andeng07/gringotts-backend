using FluentValidation;
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
/// Represents the endpoint for updating the location of an RFID reader.
/// </summary>
/// <remarks>
/// This endpoint allows updating the location details (such as building and room name) 
/// of a specific RFID reader identified by its unique ID. 
/// It includes authentication, request validation, and produces appropriate responses:
/// - Success: Returns the updated location details.
/// - Not Found: Returns an error if the specified reader ID does not exist.
/// </remarks>
public class UpdateLocationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("readers/{id:guid}",
                async ([FromBody] UpdateLocationRequest request, Guid id, AppDbContext dbContext) =>
                await EndpointHelpers.UpdateEntity<Location, UpdateLocationResponse>(
                    id, dbContext,
                    updateEntity: location =>
                    {
                        location.BuildingName = request.BuildingName;
                        location.RoomName = request.RoomName;
                    },
                    responseMapper: entity =>
                        new UpdateLocationResponse(
                            entity.Id, entity.BuildingName, entity.RoomName
                        )
                )
            )
            .WithAuthenticationFilter()
            .WithRequestValidation<UpdateLocationRequest>()
            .Produces<UpdateLocationResponse>()
            .Produces<Error>(StatusCodes.Status404NotFound);
    }

    public class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationRequest>
    {
        public UpdateLocationRequestValidator()
        {
            RuleFor(x => x.BuildingName)
                .NotEmpty()
                    .WithMessage("BuildingName is empty")
                    .WithErrorCode(LocationErrorCodes.BuildingNameEmpty)
                .MaximumLength(50)
                    .WithMessage("BuildingName must not exceed 50 characters")
                    .WithErrorCode(LocationErrorCodes.BuildingNameTooLong);

            RuleFor(x => x.RoomName)
                .MaximumLength(50)
                    .WithMessage("RoomName must not exceed 50 characters")
                    .WithErrorCode(LocationErrorCodes.RoomNameTooLong);
        }
    }

    public record UpdateLocationRequest(string BuildingName, string? RoomName);

    public record UpdateLocationResponse(Guid Id, string BuildingName, string? RoomName);
}