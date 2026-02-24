using MediatR;

namespace HRKošarka.Application.Features.Season.Commands.UpdateSeason
{
    public class UpdateSeasonCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}