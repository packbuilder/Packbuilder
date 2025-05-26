using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Packbuilder.Dto.Create;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.ModpackDtos
{
    public class SuggestionDto : SuggestionPartialDto
    {
        [JsonPropertyName("modpack")] public virtual ModpackDto? ModpackDto { get; set; } = null!;
        [JsonPropertyName("user")] public virtual User User { get; set; } = null!;
        [JsonPropertyName("modifications")] public ICollection<ModificationDto> ModificationDtos { get; set; } = [];

        [SetsRequiredMembers]
        public SuggestionDto(Suggestion suggestion)
        {
            Id = suggestion.Id;
            ModpackId = suggestion.ModpackId;
            UserId = suggestion.UserId;
            State = suggestion.State;
            GameVersion = suggestion.GameVersion;
            ModLoader = suggestion.ModLoader;
            User = suggestion.User;
            VersionId = suggestion.VersionId;
            VersionIteration = suggestion.VersionIteration;
            Memo = suggestion.Memo;

            if(suggestion.Modpack is not null)
            {    
                ModpackDto = new ModpackDto(suggestion.Modpack)
                {
                    Id = suggestion.Modpack.Id,
                    Name = suggestion.Modpack.Name,
                    Slug = suggestion.Modpack.Slug,
                    UserId = suggestion.Modpack.UserId
                };
            }
            
            ModificationDtos = [.. suggestion.Modifications.Select(m => new ModificationDto(m))];
        }
    }
}