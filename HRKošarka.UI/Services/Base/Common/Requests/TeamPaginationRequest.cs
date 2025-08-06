namespace HRKošarka.UI.Services.Base.Common.Requests
{
    public class TeamPaginationRequest : PaginationRequest
    {
        public Guid? AgeCategoryId { get; set; }
        public Gender? Gender { get; set; }
        public bool? IsActive { get; set; }
    }
}
