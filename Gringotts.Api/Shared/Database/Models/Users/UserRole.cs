namespace Gringotts.Api.Shared.Database.Models.Users
{
    public record UserRole
    {
        public Guid UserId { get; init; }
        public Guid RoleId { get; set; }
    }
}