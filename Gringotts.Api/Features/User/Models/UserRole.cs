namespace Gringotts.Api.Features.User.Models
{
    public record UserRole
    {
        public Guid UserId { get; init; }
        public Guid RoleId { get; set; }
    }
}