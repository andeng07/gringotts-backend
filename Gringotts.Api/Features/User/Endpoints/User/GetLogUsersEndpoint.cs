using Gringotts.Api.Features.User.DataFilters;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Pagination;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.User.Endpoints.User;

public class GetLogUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/filter", async ([FromBody] GetLogUsersRequest request, AppDbContext dbContext) =>
                {
                    var dataFilter = new LogUserFilter(request.Expired, request.NameSearchTerm, request.CardIdSearchTerm,
                        request.SchoolIdSearchTerm, request.Affiliations, request.Sexes,
                        request.Departments).ApplyFilters();

                    return await EndpointHelpers.GetEntities<Models.User, GetLogUserEndpoint.GetLogUserResponse>(
                        dbContext,
                        request.Page,
                        request.PageSize,
                        entityQuery: dataFilter,
                        responseMapper: c =>
                            new GetLogUserEndpoint.GetLogUserResponse(
                                c.Id,
                                c.AccessExpiry,
                                c.CardId,
                                c.SchoolId,
                                c.FirstName,
                                c.MiddleName,
                                c.LastName,
                                c.Affiliation,
                                c.Sex,
                                c.DepartmentId
                            )
                    );
                }
            )
            //.WithAuthenticationFilter()
            .Produces<PaginatedResult<GetLogUserEndpoint.GetLogUserResponse>>();
    }

    public record GetLogUsersRequest(
        int Page,
        int PageSize,
        bool? Expired,
        string? NameSearchTerm,
        string? CardIdSearchTerm,
        string? SchoolIdSearchTerm,
        IEnumerable<byte>? Affiliations,
        IEnumerable<byte>? Sexes,
        IEnumerable<Guid>? Departments
    );
}