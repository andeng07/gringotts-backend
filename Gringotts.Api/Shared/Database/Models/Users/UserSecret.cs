namespace Gringotts.Api.Shared.Database.Models.Users
{
    public record UserSecret
    {
        public required int Id { get; init; }
        public required int UserId { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}