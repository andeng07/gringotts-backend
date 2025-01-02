using FluentValidation;
using Gringotts.Api.Features.Reader.Models;
using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Endpoints.Helper;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Reader.Endpoints;

/// <summary>
/// Endpoint for adding a new location to the system.
/// </summary>
/// <remarks>
/// This endpoint allows adding a location by providing the building name and room name. 
/// If the location is successfully created, it returns a 201 Created status with the location details. 
/// If the request is invalid or the location creation fails, it returns a 400 Bad Request with error details.
/// </remarks>
/// <response code="201">Returns the created location details when the location is successfully added.</response>
/// <response code="400">Returns error details if the location creation fails, such as missing or invalid fields.</response>
public class AddLocationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("locations",
                async ([FromBody] AddLocationRequest request, AppDbContext dbContext) =>
                await EndpointHelpers.CreateEntity<Location, AddLocationResponse>(
                    new Location
                        { Id = Guid.NewGuid(), BuildingName = request.BuildingName, RoomName = request.RoomName },
                    dbContext,
                    entity => $"locations/{entity.Id}",
                    location => new AddLocationResponse(location.Id, location.BuildingName, location.RoomName)
                )
            )
            .WithRequestValidation<AddLocationRequest>()
            .WithAuthenticationFilter()
            .Produces<AddLocationResponse>();
    }

    public class AddLocationRequestValidator : AbstractValidator<AddLocationRequest>
    {
        public AddLocationRequestValidator()
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

    public record AddLocationRequest(string BuildingName, string? RoomName);

    public record AddLocationResponse(Guid Id, string BuildingName, string? RoomName);
}