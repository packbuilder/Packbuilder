using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.Dtos.ManifestDtos;
using CurseForge.enums;
using CurseForge.Interfaces;
using Microsoft.AspNetCore.Http;
using Packbuilder.Dto.CurseForgeDtos;

namespace Packbuilder.Services
{
    public partial class CurseForgeManifestService(ICurseForgeApiService curseForgeApiService, ICurseForgeService curseForgeService) : ICurseForgeManifestService
    {
        private const long MaxFileSizeBytes = 512 * 1024;

        private async static Task<IFormFile> VerifyManifestFile(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new Exception("No file provied.");

            string unsafeName = file.FileName;
            string safeName = Path.GetFileName(unsafeName); 

            if (!MyRegex().IsMatch(safeName)) throw new Exception("Invalid filename or extension.");

            if (file.Length > MaxFileSizeBytes) throw new Exception("File exceeds maximum allowed size.");

            return file;
        }

        private static MinecraftModLoader GetPrimaryModLoader(CurseForgeManifestResponseDto manifest)
        {
            ModLoaderManifest primaryModLoader = manifest.MinecraftInfo.ModLoaders.First(m => m.IsPrimary);

            string loaderName = primaryModLoader.Id.Split('-', 2, StringSplitOptions.RemoveEmptyEntries)[0].ToLowerInvariant();

            return loaderName switch
            {
                "neoforge" => MinecraftModLoader.NeoForge,
                "forge"    => MinecraftModLoader.Forge,
                "quilt"    => MinecraftModLoader.Quilt,
                "fabric"   => MinecraftModLoader.Fabric,
                _ => throw new NotSupportedException($"Unsupported mod loader '{primaryModLoader.Id}' in manifest.json")
            };
        }

        public async Task<CurseForgeManifestDto> ParseManifestFile(IFormFile file)
        {
            IFormFile verifiedFile = await VerifyManifestFile(file);

            JsonSerializerOptions jsonOptions = new()
            {
                MaxDepth = 10,
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            using Stream stream = verifiedFile.OpenReadStream();
            
            CurseForgeManifestResponseDto? manifest = await JsonSerializer.DeserializeAsync<CurseForgeManifestResponseDto>(stream, jsonOptions);

            if (manifest == null || manifest.MinecraftInfo == null || manifest.ModFiles.Count == 0)
                throw new Exception("The JSON is valid but is not a recognized CurseForge manifest.");
            
            MinecraftModLoader modLoader = GetPrimaryModLoader(manifest);
            
            CurseForgeManifestDto manifestDto = new()
            {
                Name = manifest.Name,
                GameVersion = manifest.MinecraftInfo.GameVersion,
                ModLoader = modLoader,
                ModFiles = manifest.ModFiles
            };
            
            return manifestDto;
        }

        public async Task<MemoryStream> CreateManifestZipFile(List<string> modReferenceIds, string minecraftVersion, MinecraftModLoader minecraftModLoader, string modpackName, string modpackVersion, string ownerUsername)
        {
            CurseForgeModLoaderSummary? optimalModLoaderVersion = await curseForgeService.GetOptimalModLoaderVersion(minecraftVersion, minecraftModLoader) ?? throw new Exception("Problem with finding optimal mod loader for modpack");

            ModLoaderManifest modLoaderManifest = new()
            {
                Id = optimalModLoaderVersion.Name,
                IsPrimary = true
            };

            List<CurseForgeManifestMod> manifestMods = [];

            foreach (string referenceId in modReferenceIds)
            {
                CurseForgeFile? latestFile = await curseForgeApiService.GetLatestFileAsync(referenceId, minecraftVersion, minecraftModLoader) ?? throw new Exception($"Unable to find compatible mod file for mod: {referenceId} under minecraft version {minecraftVersion} under the mod loader {minecraftModLoader}");

                manifestMods.Add(new()
                {
                    ProjectId = latestFile.ModReferenceId,
                    FileId = latestFile.Id,   
                });
            }

            CurseForgeManifest curseForgeManifest = new(modpackName, minecraftVersion, modLoaderManifest, modpackVersion, ownerUsername, manifestMods);

            string manifestJsonString = JsonSerializer.Serialize(curseForgeManifest, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            MemoryStream zipStream = new();

            using (ZipArchive archive = new(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                ZipArchiveEntry manifestEntry = archive.CreateEntry("manifest.json");

                using (StreamWriter writer = new(manifestEntry.Open(), Encoding.UTF8))
                {
                    await writer.WriteAsync(manifestJsonString);
                }

                archive.CreateEntry("overrides/");
            }

            zipStream.Position = 0;
            
            return zipStream;
        }

        [GeneratedRegex(@"^[\w\-. ]+\.json$")]
        private static partial Regex MyRegex();
    }
}