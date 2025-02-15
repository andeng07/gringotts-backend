using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Features.Interactions.Services;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Interactions.Endpoints;

/// <summary>
/// Endpoint for logging user interactions with an RFID reader.
/// </summary>
/// <remarks>
/// This endpoint records a user's interaction with an RFID reader by processing session log data.  
/// It validates the provided RFID reader ID and user card ID, then stores the log entry in the system.  
/// If the session log is successfully created, the response includes the session details.  
/// If validation fails, an error response is returned.
/// </remarks>
/// <response code="200">Returns the processed session log details upon successful creation.</response>
/// <response code="400">Returns error details if the request validation fails, such as an invalid reader ID or card ID.</response>
public class AddLogSessionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("logs/", HandleAsync)
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
            sessionLog.LogReaderId,
            sessionLog.LogUserId,
            sessionLog.CardId,
            sessionLog.DateTime,
            sessionLog.InteractionType
        );
        
        return Results.Ok(response);
    }

    public record ProcessUserLogRequest(Guid LogReaderId, string CardId, DateTime DateTime);
    public record ProcessUserLogResponse(Guid SessionLogId, Guid LogReaderId, Guid? LogUserId, string CardId, DateTime DateTime, InteractionType InteractionType);
}