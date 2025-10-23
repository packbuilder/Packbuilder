using CurseForge.Models;

namespace CurseForge.Dtos;

public class CurseForgeModListDto
{
    public required List<CurseForgeModDto> Mods { get; set; }
    public required CurseForgePagination Pagination { get; set; }
}
