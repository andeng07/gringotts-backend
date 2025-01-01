using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Results;

namespace Gringotts.Api.Features.Reader.Services;

public class ReaderService(AppDbContext appDbContext)
{
    public async Task<TypedResult<Models.Reader>> CreateReaderAsync(string name, Guid location)
    {
        var reader = new Models.Reader
        {
            Id = Guid.NewGuid(),
            Name = name,
            LocationId = location,
            AccessToken = Guid.NewGuid().ToString()
        };

        await appDbContext.Readers.AddAsync(reader);
        await appDbContext.SaveChangesAsync();

        return TypedResult<Models.Reader>.Success(reader);
    }

    public async Task<TypedResult<Models.Reader>> GetReaderById(Guid id)
    {
        var reader = await appDbContext.Readers.FindAsync(id);

        if (reader == null)
        {
            return new Error("Reader.Entities.NotFound", "Reader not found.", Error.ErrorType.NotFound);
        }

        return TypedResult<Models.Reader>.Success(reader);
    }

    public async Task<Result> DeleteReaderById(Guid id)
    {
        var readerResult = await GetReaderById(id);

        if (!readerResult.IsSuccess)
        {
            return Result.Failure(readerResult.Errors.ToArray());
        }

        appDbContext.Readers.Remove(readerResult.Value!);
        await appDbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateReader(Guid id, string name, Guid location)
    {
        var readerResult = await GetReaderById(id);

        if (!readerResult.IsSuccess)
        {
            return Result.Failure(readerResult.Errors.ToArray());
        }

        var reader = readerResult.Value!;
        reader.Name = name;
        reader.LocationId = location;

        await appDbContext.SaveChangesAsync();

        return Result.Success();
    }
}