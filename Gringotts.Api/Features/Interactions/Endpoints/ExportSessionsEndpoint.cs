using Gringotts.Api.Features.Interactions.Services;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Interactions.Endpoints;

public class ExportSessionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/sessions/export/", 
            async (AppDbContext dbContext, [FromBody] ExportSessionsRequest request) => 
                await new SessionsExporter(dbContext).ExportAsync(request))
            .WithAuthenticationFilter();
    }

    public record ExportSessionsRequest(
        IEnumerable<Guid>? UserIds,
        IEnumerable<Guid>? ReaderIds,
        DateTime? From,
        DateTime? To
    );
}