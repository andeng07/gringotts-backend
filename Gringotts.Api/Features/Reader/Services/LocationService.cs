using Gringotts.Api.Features.Reader.Models;
using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.Reader.Services;

/// <summary>
/// Service for managing locations within the system.
/// </summary>
/// <param name="dbContext">The database context used to interact with the location data.</param>
public class LocationService(AppDbContext dbContext)
{
    /// <summary>
    /// Creates a new location with the specified building name and room name.
    /// </summary>
    /// <param name="buildingName">The name of the building.</param>
    /// <param name="roomName">The name of the room.</param>
    /// <returns>A result indicating the success or failure of the location creation.</returns>
    public async Task<TypedResult<Location>> CreateLocation(string buildingName, string? roomName)
    {
        var locationExists = await dbContext.Locations.AnyAsync(
            x => x.BuildingName == buildingName && x.RoomName == roomName
        );

        if (locationExists)
            return new Error(LocationErrorCodes.LocationAlreadyExists,
                $"The location with building name \"{buildingName}\" and room name \"{roomName}\" already exists",
                Error.ErrorType.Conflict);

        var location = new Location
        {
            Id = Guid.NewGuid(),
            BuildingName = buildingName,
            RoomName = roomName
        };

        await dbContext.Locations.AddAsync(location);
        await dbContext.SaveChangesAsync();

        return location;
    }

    /// <summary>
    /// Retrieves a location by its unique identifier.
    /// </summary>
    /// <param name="guid">The unique identifier of the location.</param>
    /// <returns>A result containing the location if found, or an error if not found.</returns>
    public async Task<TypedResult<Location>> GetLocation(Guid guid)
    {
        var location = await dbContext.Locations.FirstOrDefaultAsync(x => x.Id == guid);

        if (location == null) return new Error(LocationErrorCodes.LocationNotFound,
            $"The location with id \"{guid}\" does not exist",
            Error.ErrorType.NotFound);

        return location;
    }

    /// <summary>
    /// Retrieves a location by its building and room name.
    /// </summary>
    /// <param name="buildingName">The name of the building.</param>
    /// <param name="roomName">The name of the room.</param>
    /// <returns>A result containing the location if found, or an error if not found.</returns>
    public async Task<TypedResult<Location>> GetLocation(string buildingName, string? roomName)
    {
        var location = await dbContext.Locations.FirstOrDefaultAsync(x =>
            string.Equals(x.BuildingName, buildingName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.RoomName, roomName, StringComparison.OrdinalIgnoreCase)
        );

        if (location == null) return new Error(LocationErrorCodes.LocationNotFound, 
            $"The location with building name \"{buildingName}\" and room name \"{roomName}\" does not exist",
            Error.ErrorType.NotFound);

        return location;
    }
}