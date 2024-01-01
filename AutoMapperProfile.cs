using AutoMapper;
using rpg.Dtos.Character;
using rpg.Dtos.Skill;
using rpg.Dtos.Weapon;
using rpg.Models;

namespace rpg
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, CharacterResponseDto>();
            CreateMap<AddCharacterRequestDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<AddWeaponRequestDto, Weapon>();
            CreateMap<Skill, SkillResponseDto>();
        }
    }
}
