using Gringotts.Api.Features.Interactions.DataFilters;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Interactions.Endpoints;

public class GetInteractionLogsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/interaction-logs/filter/", async (
            [FromBody] GetInteractionLogsRequest request,
            AppDbContext dbContext
        ) =>
        {
            var filter = new InteractionLogsFilter(
                request.UserIds,
                request.ReaderIds,
                request.From,
                request.To,
                request.InteractionTypes
            ).ApplyFilters();

            return await EndpointHelpers.GetEntities<InteractionLog, GetInteractionLogResponse>(
                dbContext,
                request.Page,
                request.PageSize,
                filter,
                entity =>
                    new GetInteractionLogResponse(
                        entity.Id,
                        entity.LogReaderId,
                        entity.LogUserId,
                        entity.CardId,
                        entity.DateTime,
                        entity.InteractionType
                    )
            );
        })
        .WithAuthenticationFilter();
    }

    public record GetInteractionLogsRequest(
        int Page,
        int PageSize,
        IEnumerable<Guid>? UserIds,
        IEnumerable<Guid>? ReaderIds,
        DateTime? From,
        DateTime? To,
        IEnumerable<InteractionType>? InteractionTypes
    );

    public record GetInteractionLogResponse(
        Guid Id,
        Guid LogReaderId,
        Guid? LogUserId,
        string CardId,
        DateTime DateTime,
        InteractionType InteractionType
    );
}