namespace HRKošarka.Application.Models.Identity
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string Role { get; set; } = string.Empty;
        public bool IsLockedOut { get; set; }
        public Guid? ManagedClubId { get; set; }
        public string? ManagedClubName { get; set; }
    }
}
