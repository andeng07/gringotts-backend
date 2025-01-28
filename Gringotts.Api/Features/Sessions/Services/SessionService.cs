using Gringotts.Api.Features.Reader.Services;
using Gringotts.Api.Features.Sessions.Models;
using Gringotts.Api.Features.User.Services;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;
using Type = Gringotts.Api.Features.Sessions.Models.Type;

namespace Gringotts.Api.Features.Sessions.Services;

public class SessionService(AppDbContext dbContext, ReaderService readerService, UserService userService)
{
    public async Task<TypedOperationResult<SessionLog>> ProcessSessionLog(Guid logReaderId, string cardId,
        DateTime date)
    {
        // Validate Reader
        var getReaderResult = await readerService.GetReaderById(logReaderId);
        if (!getReaderResult.IsSuccess) return getReaderResult.Errors;

        var reader = getReaderResult.Value!;

        // Validate User
        var getUserResult = await userService.GetLogUserByCardIdAsync(cardId);
        if (!getUserResult.IsSuccess)
        {
            return await AddSessionLog(reader.Id, null, cardId, date, Type.Fallback);
        }

        var user = getUserResult.Value!;
        var userActiveSession = await GetActiveSession(user.Id);

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

    private async Task<TypedOperationResult<SessionLog>> HandleSessionTransition(ActiveSession activeSession,
        Guid newReaderId, Guid userId, string cardId, DateTime date)
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
            return TypedOperationResult<SessionLog>.Failure(new ErrorResponse("", ex.Message,
                ErrorResponse.ErrorType.Conflict));
        }
    }

    public async Task<SessionLog> ProcessEntry(Guid readerId, Guid userId, string cardId, DateTime dateTime)
    {
        // Ensure no duplicate active session
        var existingActiveSession = await GetActiveSession(userId);
        if (existingActiveSession != null)
        {
            throw new InvalidOperationException("User already has an active session.");
        }

        var session = await AddSession(readerId, userId, dateTime);
        await AddActiveSession(userId, readerId, session.Id);

        return await AddSessionLog(readerId, userId, cardId, dateTime, Type.Entry);
    }

    public async Task<SessionLog> ProcessExit(ActiveSession activeSession, string cardId, DateTime dateTime)
    {
        if (activeSession == null) throw new ArgumentException("Active session not found.", nameof(activeSession));

        var session = await GetSession(activeSession.SessionId);
        if (session == null)
        {
            await RemoveActiveSession(activeSession.Id);
            throw new ArgumentException("Session reference from active session does not exist.", nameof(activeSession));
        }

        session.EndDate = dateTime;
        await dbContext.SaveChangesAsync();

        await RemoveActiveSession(activeSession.Id);
        return await AddSessionLog(session.LogReaderId, activeSession.LogUserId, cardId, dateTime, Type.Exit);
    }

    public async Task<ActiveSession?> GetActiveSession(Guid logUserId)
    {
        return await dbContext.ActiveSessions.FirstOrDefaultAsync(activeSession =>
            activeSession.LogUserId == logUserId);
    }

    public async Task<ActiveSession> AddActiveSession(Guid logUserId, Guid logReaderId, Guid sessionId)
    {
        var activeSession = new ActiveSession
        {
            Id = Guid.NewGuid(),
            LogUserId = logUserId,
            LogReaderId = logReaderId,
            SessionId = sessionId
        };

        await dbContext.ActiveSessions.AddAsync(activeSession);
        await dbContext.SaveChangesAsync();

        return activeSession;
    }

    public async Task RemoveActiveSession(Guid activeSessionId)
    {
        var activeSession = await dbContext.ActiveSessions.FindAsync(activeSessionId);

        if (activeSession == null) return;

        dbContext.ActiveSessions.Remove(activeSession);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Session?> GetSession(Guid sessionId)
    {
        return await dbContext.Sessions.FindAsync(sessionId);
    }

    public async Task<Session> AddSession(Guid logReaderId, Guid logUserId, DateTime startDate)
    {
        var session = new Session
        {
            Id = Guid.NewGuid(),
            LogReaderId = logReaderId,
            LogUserId = logUserId,
            StartDate = startDate,
            EndDate = null
        };

        await dbContext.Sessions.AddAsync(session);
        await dbContext.SaveChangesAsync();

        return session;
    }

    public async Task<SessionLog> AddSessionLog(Guid logReaderId, Guid? logUserId, string cardId, DateTime dateTime,
        Type sessionType)
    {
        var sessionLog = new SessionLog
        {
            Id = Guid.NewGuid(),
            TransactionId = Guid.NewGuid().ToString(), // Generate unique transaction ID
            LogReaderId = logReaderId,
            LogUserId = logUserId,
            CardId = cardId,
            DateTime = dateTime,
            Type = sessionType
        };

        await dbContext.SessionLogs.AddAsync(sessionLog);
        await dbContext.SaveChangesAsync();

        return sessionLog;
    }
}