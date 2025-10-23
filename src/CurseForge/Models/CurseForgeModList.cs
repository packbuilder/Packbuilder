namespace CurseForge.Models;

public class CurseForgeModList
{
    public required List<CurseForgeMod> Mods { get; set; }
    public required CurseForgePagination Pagination { get; set; }
}
