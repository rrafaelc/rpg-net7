using AutoMapper;
using rpg.Dtos.Character;
using rpg.Models;
using rpg.Repositories.CharacterRepository;

namespace rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        public ICharacterRepository _characterRepository;

        public CharacterService(IMapper mapper, ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<CharacterResponseDto>> AddCharacter(AddCharacterRequestDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<CharacterResponseDto>();

            try
            {
                var character = _mapper.Map<Character>(newCharacter);
                var characterCreated = await _characterRepository.AddCharacter(character);
                serviceResponse.Data = _mapper.Map<CharacterResponseDto>(characterCreated);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<CharacterResponseDto>>> GetAllCharacters()
        {
            var dbCharacters = await _characterRepository.FindCharacters();
            var serviceResponse = new ServiceResponse<List<CharacterResponseDto>>
            {
                Data = dbCharacters.Select(x => _mapper.Map<CharacterResponseDto>(x)).ToList()
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<CharacterResponseDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<CharacterResponseDto>();
            var dbCharacter = await _characterRepository.FindCharacterById(id);

            if (dbCharacter is not null)
                serviceResponse.Data = _mapper.Map<CharacterResponseDto>(dbCharacter);
            else
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Character not found";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<CharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<CharacterResponseDto>();
            try
            {
                if (updatedCharacter == null)
                    throw new Exception("Invalid input: updatedCharacter is null");

                var character = await _characterRepository.FindCharacterById(updatedCharacter.Id)
                ?? throw new Exception($"Character with id '{updatedCharacter.Id}' not found");
                character.Name = updatedCharacter.Name ?? character.Name;
                character.HitPoints = updatedCharacter.HitPoints ?? character.HitPoints;
                character.Strength = updatedCharacter.Strength ?? character.Strength;
                character.Defense = updatedCharacter.Defense ?? character.Defense;
                character.Intelligence = updatedCharacter.Intelligence ?? character.Intelligence;
                character.Class = updatedCharacter.Class ?? character.Class;
                serviceResponse.Data = _mapper.Map<CharacterResponseDto>(character);

                await _characterRepository.UpdateCharacter(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task DeleteCharacter(int id)
        {
            var dbCharacter = await _characterRepository.FindCharacterById(id)
            ?? throw new Exception($"Character with id '{id}' not found"); ;
            await _characterRepository.DeleteCharacter(dbCharacter);
        }
    }
}
