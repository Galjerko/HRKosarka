namespace HRKošarka.Application.Features.Season.Queries.GetAllSeasons
{
    public class SeasonDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
    }
}