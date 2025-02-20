using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;

namespace Gringotts.Api.Features.Reader.Services;

/// <summary>
/// Service for managing RFID readers, including creation, retrieval, updating, and deletion.
/// </summary>
/// <remarks>
/// The ReaderService provides methods to handle CRUD operations for RFID readers.
/// Readers are associated with a location and identified by a unique access token.
/// </remarks>
public class ReaderService(AppDbContext appDbContext)
{
    /// <summary>
    /// Creates a new RFID reader in the system.
    /// </summary>
    /// <param name="name">The name of the RFID reader.</param>
    /// <param name="location">The unique identifier of the location associated with the reader.</param>
    /// <returns>
    /// A <see cref="TypedOperationResult{TValue}"/> containing the created reader entity if successful,  
    /// or an error if the operation fails.
    /// </returns>
    public async Task<TypedOperationResult<Reader.Models.Reader>> CreateReaderAsync(string name, Guid location)
    {
        var reader = new Reader.Models.Reader
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Name = name,
            LocationId = location
        };

        await appDbContext.Readers.AddAsync(reader);
        await appDbContext.SaveChangesAsync();

        return TypedOperationResult<Reader.Models.Reader>.Success(reader);
    }

    /// <summary>
    /// Retrieves an RFID reader by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the RFID reader.</param>
    /// <returns>
    /// A <see cref="TypedOperationResult{TValue}"/> containing the reader entity if found,  
    /// or an error if the reader does not exist.
    /// </returns>
    public async Task<TypedOperationResult<Reader.Models.Reader>> GetReaderById(Guid id)
    {
        var reader = await appDbContext.Readers.FindAsync(id);

        if (reader == null)
        {
            return new ErrorResponse("Reader.Entities.NotFound", "Reader not found.", ErrorResponse.ErrorType.NotFound);
        }

        return TypedOperationResult<Reader.Models.Reader>.Success(reader);
    }

    /// <summary>
    /// Deletes an RFID reader by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the RFID reader to delete.</param>
    /// <returns>
    /// An <see cref="OperationResult"/> indicating success if the reader was deleted,  
    /// or an error if the reader was not found.
    /// </returns>
    public async Task<OperationResult> DeleteReaderById(Guid id)
    {
        var readerResult = await GetReaderById(id);

        if (!readerResult.IsSuccess)
        {
            return OperationResult.Failure(readerResult.Errors.ToArray());
        }

        appDbContext.Readers.Remove(readerResult.Value!);
        await appDbContext.SaveChangesAsync();

        return OperationResult.Success();
    }

    /// <summary>
    /// Updates the details of an existing RFID reader.
    /// </summary>
    /// <param name="id">The unique identifier of the reader to update.</param>
    /// <param name="name">The new name of the reader.</param>
    /// <param name="location">The new location identifier of the reader.</param>
    /// <returns>
    /// An <see cref="OperationResult"/> indicating success if the reader was updated,  
    /// or an error if the reader was not found.
    /// </returns>
    public async Task<OperationResult> UpdateReader(Guid id, string name, Guid location)
    {
        var readerResult = await GetReaderById(id);

        if (!readerResult.IsSuccess)
        {
            return OperationResult.Failure(readerResult.Errors.ToArray());
        }

        var reader = readerResult.Value!;
        reader.Name = name;
        reader.LocationId = location;

        await appDbContext.SaveChangesAsync();

        return OperationResult.Success();
    }
}
