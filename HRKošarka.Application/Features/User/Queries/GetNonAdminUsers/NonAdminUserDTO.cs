namespace HRKošarka.Application.Features.User.Queries.GetNonAdminUsers
{
    public class NonAdminUserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Role { get; set; } = string.Empty;
        public Guid? ManagedClubId { get; set; }
        public string? ManagedClubName { get; set; }
    }
}
