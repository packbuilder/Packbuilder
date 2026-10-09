using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.ModpackDtos
{
    public class BookmarkDto
    {
        [JsonPropertyName("userId")] 
        public required int UserId { get; set; }
        [JsonPropertyName("modpackId")]
        public required int ModpackId { get; set; }
        [JsonPropertyName("modpack")]
        public virtual ModpackDto Modpack { get; set; } = null!;

        [SetsRequiredMembers]
        public BookmarkDto(Bookmark bookmark)
        {
            UserId = bookmark.UserId;
            ModpackId = bookmark.ModpackId;

            if(bookmark.Modpack is not null)
            {
                Modpack = new ModpackDto(bookmark.Modpack);
            }
        }
    }
}