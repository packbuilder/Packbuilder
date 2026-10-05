using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.Update;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class ModpackService(PackbuilderContext context) : IModpackService
    {
        public async Task CreateModpackAsync(CreateModpackDto body, int userId)
        {
            User user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId) ?? throw new Exception("Could not create modpack because user does not exist");

            List<Modpack> userModpacks = await context.Modpacks.Where(m => m.UserId == user.Id).ToListAsync();

            if(userModpacks.Count == 20) throw new Exception("User has already created the maximum amount of modpacks.");

            if(userModpacks.Find(m => m.Name == body.Name) is not null) throw new Exception($"This user has already made a modpack with the name {body.Name}");

            Modpack modpack = Modpack.CreateModpack(body.Name, user, body.ImageType, body.ImageValue, body.Game);

            ModpackVersion modpackVersion = modpack.CreateVersionFromMods([], body.GameVersion, body.ModLoader); 

            context.Modpacks.Add(modpack);
            context.Versions.Add(modpackVersion);

            await context.SaveChangesAsync();

            return;
        }

        public async Task UpdateModpackAsync(UpdateModpackDto body, int modpackId)
        {
            Modpack? modpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Id == modpackId)
                ?? throw new Exception($"Could not update modpack with id {modpackId} because it doesn't exist.");
            
            if(body.Name is not null && body.Name != null) modpack.Name = body.Name;

            if(body.ImageValue is not null && body.ImageValue != null)
            {
                modpack.ImageValue = body.ImageValue;
            }

            if(body.ImageType is { } imageType)
            {
                modpack.ImageType = imageType;    
            }

            context.Modpacks.Update(modpack);
            await context.SaveChangesAsync();

            return;
        }

        public async Task DeleteModpackAsync(int modpackId)
        {
            Modpack? modpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Id == modpackId)
                ?? throw new Exception($"Could not delete modpack with id {modpackId} because it doesn't exist.");
            
            context.RemoveRange(modpack);
            await context.SaveChangesAsync();

            return;
        }

    }
}