using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Packbuilder.Dto.CurseForgeDtos;

namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeManifest
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("minecraft")]
        public required MinecraftInfoDto MinecraftInfo { get; set; }
        [JsonPropertyName("manifestType")]
        public required string ManifestType { get; set; }
        [JsonPropertyName("manifestVersion")]
        public required int ManifestVersion { get; set; }
        [JsonPropertyName("version")]
        public required string Version { get; set; }
        [JsonPropertyName("author")]
        public required string Author { get; set; }
        [JsonPropertyName("overrides")]
        public required string Overrides { get; set; }
        [JsonPropertyName("files")]
        public required List<CurseForgeManifestMod> ModFiles { get; set; }

        [SetsRequiredMembers]
        public CurseForgeManifest(string modpackName, string minecraftVersion, ModLoaderManifest modLoaderManifest, string modpackVersion, string ownerUsername, List<CurseForgeManifestMod> manifestModDtos)
        {
            Name = modpackName;
            MinecraftInfo = new()
            {
                GameVersion = minecraftVersion,
                ModLoaders = [modLoaderManifest]
            };
            ManifestType = "minecraftModpack";
            ManifestVersion = 1;
            Overrides = "overrides";
            Version = modpackVersion;
            Author = ownerUsername;
            ModFiles = manifestModDtos;
        }
    }
}