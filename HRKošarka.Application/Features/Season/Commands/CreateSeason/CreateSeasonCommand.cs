using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Season.Commands.CreateSeason
{
    public class CreateSeasonCommand : IRequest<CommandResponse<Guid>>
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}