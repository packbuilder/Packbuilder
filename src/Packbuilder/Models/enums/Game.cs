namespace Packbuilder.Models.enums
{
    public enum Game
    {
        Minecraft,
    }

    public static class GamePlatformSupport
    {
        public static bool IsSupported(Game game, ModPlatform platform)
        {
            return (game, platform) switch
            {
                (Game.Minecraft, ModPlatform.CurseForge) => true,

                // Example of future integrations
                // (Game.RiskOfRain2, ModPlatform.Thunderstore) => true,
                // (Game.LethalCompany, ModPlatform.Thunderstore) => true,

                _ => false
            };
        }
    }
}