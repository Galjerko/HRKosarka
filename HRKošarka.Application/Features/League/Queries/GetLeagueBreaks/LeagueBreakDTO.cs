namespace HRKošarka.Application.Features.League.Queries.GetLeagueBreaks
{
    public class LeagueBreakDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
