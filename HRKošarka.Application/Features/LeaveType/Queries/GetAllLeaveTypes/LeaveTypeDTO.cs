namespace HRKošarka.Application.Features.LeaveType.Queries.GetAllLeaveTypes
{
    public class LeaveTypeDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DefaultDays { get; set; }
    }
}
