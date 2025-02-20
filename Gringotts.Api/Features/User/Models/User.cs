using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.User.Models
{
    public record User : IEntity
    {
        public required Guid Id { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required DateTime AccessExpiry { get; set; }
        public required string CardId { get; set; }
        public required string SchoolId { get; set; }
        public required string FirstName { get; set; }
        public required string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required byte Affiliation { get; set; }
        public required byte Sex { get; set; }
        public required Guid? DepartmentId { get; init; }
    }
    
    public enum Sex : byte
    {
        Male = 0, Female = 1
    }

    public enum Affiliation : byte
    {
        Student = 0, Faculty = 1, Staff = 2, Administrator = 3
    }
}