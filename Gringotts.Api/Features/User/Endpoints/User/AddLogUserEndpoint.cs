using FluentValidation;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.User.Endpoints.User;

public class AddLogUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/",
                async ([FromBody] AddLogUserRequest request, AppDbContext dbContext) =>
                await EndpointHelpers.CreateEntity<Models.User, AddLogUserResponse>(
                    new Models.User
                    {
                        Id = Guid.NewGuid(),
                        CardId = request.CardId,
                        SchoolId = request.SchoolId,
                        FirstName = request.FirstName,
                        MiddleName = request.MiddleName,
                        LastName = request.LastName,
                        Affiliation = request.Affiliation,
                        Sex = request.Sex,
                        DepartmentId = request.Department
                    },
                    dbContext,
                    uri: logUser => $"/users/{logUser.Id}",
                    responseMapper: logUser => new AddLogUserResponse(
                        logUser.Id, logUser.CardId, logUser.SchoolId, logUser.FirstName, logUser.MiddleName,
                        logUser.LastName, logUser.Affiliation, logUser.Sex, logUser.DepartmentId
                    )
                )
            )
            .WithAuthenticationFilter()
            .WithRequestValidation<AddLogUserRequest>()
            .WithEntityExistenceFilter<Models.Department, AddLogUserRequest>(request => request.Department)
            .Produces<AddLogUserResponse>()
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    public class AddLogUserRequestValidator : AbstractValidator<AddLogUserRequest>
    {
        public AddLogUserRequestValidator()
        {
            RuleFor(x => x.CardId)
                .NotEmpty()
                .WithMessage("Card ID is required.")
                .WithErrorCode(ValidationErrorCodes.CardIdRequired);

            RuleFor(x => x.SchoolId)
                .NotEmpty()
                .WithMessage("School ID is required.")
                .WithErrorCode(ValidationErrorCodes.SchoolIdRequired);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .WithErrorCode(ValidationErrorCodes.FirstNameRequired);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .WithErrorCode(ValidationErrorCodes.LastNameRequired);

            RuleFor(x => x.Affiliation)
                .GreaterThanOrEqualTo((byte)0)
                .WithMessage("Affiliation must be greater than or equal to 0."); // todo error code

            RuleFor(x => x.Sex)
                .GreaterThanOrEqualTo((byte)0)
                .WithMessage("Sex must be greater than or equal to 0."); // todo error code
        }
    }

    public record AddLogUserRequest(
        string CardId,
        string SchoolId,
        string FirstName,
        string? MiddleName,
        string LastName,
        byte Affiliation,
        byte Sex,
        Guid Department
    );

    public record AddLogUserResponse(
        Guid Id,
        string CardId,
        string SchoolId,
        string FirstName,
        string? MiddleName,
        string LastName,
        byte Affiliation,
        byte Sex,
        Guid Department
    );
}