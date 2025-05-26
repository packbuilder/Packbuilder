namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeModList
    {
        public required List<CurseForgeMod> Mods { get; set; }
        public CurseForgePagination? Pagination { get; set; }
    }
}