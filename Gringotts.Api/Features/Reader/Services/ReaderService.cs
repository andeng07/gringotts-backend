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
}
