namespace HRKošarka.Application.Features.User.Queries.GetInactiveUsers
{
    public class InactiveUserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
    }
}
