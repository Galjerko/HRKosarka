using MediatR;

namespace HRKošarka.Application.Features.Season.Commands.DeleteSeason
{
    public record DeleteSeasonCommand(Guid Id) : IRequest<Unit>;
}