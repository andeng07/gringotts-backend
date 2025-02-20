using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Features.Reader.Services;
using Gringotts.Api.Features.User.Services;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.Interactions.Services;

public class SessionService(AppDbContext dbContext, ReaderService readerService, UserService userService)
{
    public async Task<TypedOperationResult<InteractionLog>> ProcessSessionLog(Guid logReaderId, string cardId, DateTime date)
    {
        // Validate Reader
        var getReaderResult = await readerService.GetReaderById(logReaderId);
        if (!getReaderResult.IsSuccess) return getReaderResult.Errors;

        var reader = getReaderResult.Value!;
        
        // Validate User
        var getUserResult = await userService.GetLogUserByCardIdAsync(cardId);
        if (!getUserResult.IsSuccess)
        {
            return await AddInteractionLog(reader.Id, null, cardId, date, InteractionType.Fallback);
        }

        var user = getUserResult.Value!;
        var userActiveSession = await GetActiveSession(user.Id);
        
        // TODO: Logical bug: Cannot Exit when Access expires while being in an Active Session
        var isAccessExpired = DateTime.UtcNow <= user.AccessExpiry;
        if (isAccessExpired)
        {
            return await AddInteractionLog(reader.Id, user.Id, cardId, date, InteractionType.Unauthorized);
        }
        
        // No active session, process entry
        if (userActiveSession == null)
        {
            return await ProcessEntry(reader.Id, user.Id, cardId, date);
        }

        // Different reader, process exit and new entry
        if (userActiveSession.LogReaderId != logReaderId)
        {
            return await HandleSessionTransition(userActiveSession, logReaderId, user.Id, cardId, date);
        }

        // Same reader, process exit
        return await ProcessExit(userActiveSession, cardId, date);
    }

    private async Task<TypedOperationResult<InteractionLog>> HandleSessionTransition(
        ActiveSession activeSession, Guid newReaderId, Guid userId, string cardId, DateTime date)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await ProcessExit(activeSession, cardId, date);
            var result = await ProcessEntry(newReaderId, userId, cardId, date);

            await transaction.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return TypedOperationResult<InteractionLog>.Failure(new ErrorResponse(
                "SESSION_TRANSITION_FAILED", ex.Message, ErrorResponse.ErrorType.Conflict));
        }
    }

    public async Task<TypedOperationResult<InteractionLog>> ProcessEntry(Guid readerId, Guid userId, string cardId, DateTime dateTime)
    {
        // Ensure no duplicate active session
        var existingActiveSession = await GetActiveSession(userId);
        if (existingActiveSession != null)
        {
            return TypedOperationResult<InteractionLog>.Failure(new ErrorResponse(
                "USER_ALREADY_HAS_ACTIVE_SESSION", "User already has an active session.",
                ErrorResponse.ErrorType.Conflict));
        }

        await AddActiveSession(userId, readerId, dateTime);

        var log = await AddInteractionLog(readerId, userId, cardId, dateTime, InteractionType.Entry);
        return TypedOperationResult<InteractionLog>.Success(log);
    }

    public async Task<TypedOperationResult<InteractionLog>> ProcessExit(ActiveSession? activeSession, string cardId, DateTime dateTime)
    {
        if (activeSession == null)
        {
            return TypedOperationResult<InteractionLog>.Failure(new ErrorResponse(
                "ACTIVE_SESSION_NOT_FOUND", "Active session not found.", ErrorResponse.ErrorType.NotFound));
        }

        await RemoveActiveSession(activeSession.Id);

        var session = await AddSession(activeSession.LogReaderId, activeSession.LogUserId, activeSession.StartDate, dateTime);
        
        await dbContext.SaveChangesAsync();
        await RemoveActiveSession(activeSession.Id);

        var log = await AddInteractionLog(session.LogReaderId, activeSession.LogUserId, cardId, dateTime, InteractionType.Exit);
        return TypedOperationResult<InteractionLog>.Success(log);
    }

    public async Task<ActiveSession?> GetActiveSession(Guid logUserId)
    {
        return await dbContext.ActiveSessions.FirstOrDefaultAsync(activeSession =>
            activeSession.LogUserId == logUserId);
    }

    public async Task<ActiveSession> AddActiveSession(Guid logUserId, Guid logReaderId, DateTime startDate)
    {
        var activeSession = new ActiveSession
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            LogUserId = logUserId,
            LogReaderId = logReaderId,
            StartDate = startDate
        };

        await dbContext.ActiveSessions.AddAsync(activeSession);
        await dbContext.SaveChangesAsync();
        return activeSession;
    }

    public async Task RemoveActiveSession(Guid activeSessionId)
    {
        var activeSession = await dbContext.ActiveSessions.FindAsync(activeSessionId);
        if (activeSession != null)
        {
            dbContext.ActiveSessions.Remove(activeSession);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<Session?> GetSession(Guid sessionId)
    {
        return await dbContext.Sessions.FindAsync(sessionId);
    }

    public async Task<Session> AddSession(Guid logReaderId, Guid logUserId, DateTime startDate, DateTime endDate)
    {
        var session = new Session
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            LogReaderId = logReaderId,
            LogUserId = logUserId,
            StartDate = startDate,
            EndDate = endDate
        };

        await dbContext.Sessions.AddAsync(session);
        await dbContext.SaveChangesAsync();
        return session;
    }

    public async Task<InteractionLog> AddInteractionLog(Guid logReaderId, Guid? logUserId, string cardId, DateTime dateTime, InteractionType sessionInteractionType)
    {
        var sessionLog = new InteractionLog
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            LogReaderId = logReaderId,
            LogUserId = logUserId,
            CardId = cardId,
            DateTime = dateTime,
            InteractionType = sessionInteractionType
        };

        await dbContext.InteractionLogs.AddAsync(sessionLog);
        await dbContext.SaveChangesAsync();
        return sessionLog;
    }
}
