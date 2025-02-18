using Gringotts.Api.Features.Interactions.DataFilters;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Interactions.Endpoints;

public class GetSessionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
{
        app.MapPost("/sessions/filter/", async (
            [FromBody] GetSessionsRequest request,
            AppDbContext dbContext
        ) =>
        {
            var filter = new SessionsFilter(
                request.UserIds,
                request.ReaderIds,
                request.From,
                request.To
            ).ApplyFilters();
            
            return await EndpointHelpers.GetEntities<Session, GetSessionResponse>(
                dbContext,
                request.Page,
                request.PageSize,
                filter,
                entity =>
                    new GetSessionResponse(
                        entity.Id,
                        entity.LogReaderId,
                        entity.LogUserId,
                        entity.StartDate,
                        entity.EndDate
                    )
            );
        })
        .WithAuthenticationFilter();
    }

    public record GetSessionsRequest(
        int Page,
        int PageSize,
        IEnumerable<Guid>? UserIds,
        IEnumerable<Guid>? ReaderIds,
        DateTime? From,
        DateTime? To
    );

    public record GetSessionResponse(
        Guid Id,
        Guid LogReaderId,
        Guid LogUserId,
        DateTime StartDate,
        DateTime EndDate
    );
}