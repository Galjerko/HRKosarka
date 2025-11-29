namespace HRKošarka.UI.Models.UserManagement
{
    public class UserPermissions
    {
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDeactivate { get; set; }
        public bool CanDelete { get; set; }
        public Guid? ManagedClubId { get; set; }
    }
}
