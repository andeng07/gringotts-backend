using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;

namespace Gringotts.Api.Features.LogReader.Services;

public class ReaderService(AppDbContext appDbContext)
{
    public async Task<TypedOperationResult<LogReader.Models.LogReader>> CreateReaderAsync(string name, Guid location)
    {
        var reader = new LogReader.Models.LogReader
        {
            Id = Guid.NewGuid(),
            Name = name,
            LocationId = location,
            AccessToken = Guid.NewGuid().ToString()
        };

        await appDbContext.Readers.AddAsync(reader);
        await appDbContext.SaveChangesAsync();

        return TypedOperationResult<LogReader.Models.LogReader>.Success(reader);
    }

    public async Task<TypedOperationResult<LogReader.Models.LogReader>> GetReaderById(Guid id)
    {
        var reader = await appDbContext.Readers.FindAsync(id);

        if (reader == null)
        {
            return new ErrorResponse("LogReader.Entities.NotFound", "LogReader not found.", ErrorResponse.ErrorType.NotFound);
        }

        return TypedOperationResult<LogReader.Models.LogReader>.Success(reader);
    }

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