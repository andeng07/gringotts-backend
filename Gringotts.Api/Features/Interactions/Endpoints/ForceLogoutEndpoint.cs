using Gringotts.Api.Features.Interactions.Services;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.Interactions.Endpoints;

public class ForceLogoutEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("logs/{id:guid}/force-logout",
                async (Guid id, SessionService sessionService) => await sessionService.ForceExit(id, DateTime.UtcNow))
            .WithAuthenticationFilter();
    }
}