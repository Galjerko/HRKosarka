using AutoMapper;
using HRKošarka.Application.Features.Player.Commands.CreatePlayer;
using HRKošarka.Application.Features.Player.Commands.UpdatePlayer;
using HRKošarka.Application.Features.Player.Queries.GetAllPlayers;
using HRKošarka.Application.Features.Player.Queries.GetPlayerDetails;
using HRKošarka.Domain;

namespace HRKošarka.Application.MappingProfiles
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            CreateMap<Player, PlayerDTO>();
            CreateMap<Player, PlayerDetailsDTO>();
            CreateMap<CreatePlayerCommand, Player>();
            CreateMap<UpdatePlayerCommand, Player>();
        }
    }
}
