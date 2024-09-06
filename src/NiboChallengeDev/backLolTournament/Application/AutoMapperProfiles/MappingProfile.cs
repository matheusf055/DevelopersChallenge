using AutoMapper;
using LolTournament.Application.DTOs;
using LolTournament.Models;

namespace LolTournament.Application.AutoMapperProfiles
{
    // Configuration class for AutoMapper mappings
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Match, MatchResponseDto>(); // Maps Match entity to MatchResponseDto DTO
            CreateMap<MatchCreateDto, Match>();  // Maps MatchCreateDto DTO to Match entity
            CreateMap<MatchUpdateDto, Match>(); // Maps MatchUpdateDto DTO to Match entity

            CreateMap<Team, TeamResponseDto>(); // Maps Team entity to TeamResponseDto DTO
            CreateMap<TeamCreateDto, Team>(); // Maps TeamCreateDto DTO to Team entity
            CreateMap<TeamUpdateDto, Team>(); // Maps TeamUpdateDto DTO to Team entity

            CreateMap<Tournament, TournamentResponseDto>(); // Maps Tournament entity to TournamentResponseDto DTO
            CreateMap<TournamentCreateDto, Tournament>(); // Maps TournamentCreateDto DTO to Tournament entity
            CreateMap<TournamentUpdateDto, Tournament>(); // Maps TournamentUpdateDto DTO to Tournament entity
        }
    }
}
