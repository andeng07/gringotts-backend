using Gringotts.Api.Features.Sessions.Services;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Type = Gringotts.Api.Features.Sessions.Models.Type;

namespace Gringotts.Api.Features.Sessions.Endpoints;

public class AddLogSessionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("log-sessions/", HandleAsync)
            .WithAuthenticationFilter()
            .Produces<ProcessUserLogResponse>()
            .Produces<List<ErrorResponse>>(StatusCodes.Status400BadRequest);
    }

    public async Task<IResult> HandleAsync([FromBody] ProcessUserLogRequest request, SessionService sessionService)
    {
        var processSessionLogResult = await sessionService.ProcessSessionLog(
            request.LogReaderId,
            request.CardId,
            request.DateTime
        );

        if (!processSessionLogResult.IsSuccess)
        {
            return Results.BadRequest(processSessionLogResult.Errors);
        }
        
        var sessionLog = processSessionLogResult.Value!;
        
        var response = new ProcessUserLogResponse(
            sessionLog.Id,
            sessionLog.TransactionId,
            sessionLog.LogReaderId,
            sessionLog.LogUserId,
            sessionLog.CardId,
            sessionLog.DateTime,
            sessionLog.Type
        );
        
        return Results.Ok(response);
    }

    public record ProcessUserLogRequest(Guid LogReaderId, string CardId, DateTime DateTime);
    public record ProcessUserLogResponse(Guid SessionLogId, string TransactionId, Guid LogReaderId, Guid? LogUserId, string CardId, DateTime DateTime, Type Type);
}