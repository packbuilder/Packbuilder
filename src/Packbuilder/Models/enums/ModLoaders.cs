using CurseForge.enums;

namespace Packbuilder.Models.enums
{
    public enum ModLoader
    {
        Any,
        Forge,
        Cauldron,
        LiteLoader,
        Fabric,
        Quilt,
        NeoForge,
    }

    public static class MinecraftModLoaderMapper 
    {
        public static MinecraftModLoader TryConvertToMinecraftModLoader(ModLoader modLoader, out bool isSupported)
        {
            MinecraftModLoader minecraftModLoader = modLoader switch
            {
                ModLoader.Forge      => MinecraftModLoader.Forge,
                ModLoader.Cauldron   => MinecraftModLoader.Cauldron,
                ModLoader.LiteLoader => MinecraftModLoader.LiteLoader,
                ModLoader.NeoForge   => MinecraftModLoader.NeoForge,
                ModLoader.Fabric    => MinecraftModLoader.Fabric,
                ModLoader.Quilt     => MinecraftModLoader.Quilt,
                _                   => MinecraftModLoader.None
            };

            isSupported = minecraftModLoader != MinecraftModLoader.None;

            return minecraftModLoader;
        }

        public static ModLoader TryConvertFromMinecraftModLoader(MinecraftModLoader minecraftModLoader, out bool isSupported)
        {
            ModLoader modLoader = minecraftModLoader switch
            {
                MinecraftModLoader.Forge      => ModLoader.Forge,
                MinecraftModLoader.Cauldron   => ModLoader.Cauldron,
                MinecraftModLoader.LiteLoader => ModLoader.LiteLoader,
                MinecraftModLoader.NeoForge   => ModLoader.NeoForge,
                MinecraftModLoader.Fabric    => ModLoader.Fabric,
                MinecraftModLoader.Quilt     => ModLoader.Quilt,
                _                   => ModLoader.Any
            };

            isSupported = modLoader != ModLoader.Any;

            return modLoader;
        }
    } 
}