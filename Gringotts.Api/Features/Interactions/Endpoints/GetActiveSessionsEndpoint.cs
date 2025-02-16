using Gringotts.Api.Features.Interactions.DataFilters;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Interactions.Endpoints;

public class GetActiveSessionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/active-sessions/filter", async (
            [FromBody] GetActiveSessionsRequest request,
            AppDbContext dbContext
        ) =>
        {
            var filter = new ActiveSessionFilter(
                request.UserIds, request.ReaderIds, request.From, request.To
            ).ApplyFilters();

            return await EndpointHelpers.GetEntities<ActiveSession, GetActiveSessionsResponse>(
                dbContext,
                request.Page,
                request.PageSize,
                filter,
                entity =>
                    new GetActiveSessionsResponse(
                        entity.Id,
                        entity.LogReaderId,
                        entity.LogUserId,
                        entity.StartDate
                    )
            );
        });
    }

    public record GetActiveSessionsRequest(
        int Page,
        int PageSize,
        IEnumerable<Guid>? UserIds,
        IEnumerable<Guid>? ReaderIds,
        DateTime? From,
        DateTime? To
    );

    public record GetActiveSessionsResponse(
        Guid Id,
        Guid LogReaderId,
        Guid LogUserId,
        DateTime StartDate
    );
}