using Packbuilder.Models.CurseForgeApiResponse;

namespace Packbuilder.Dto.CurseForgeDtos
{
    public class CurseForgeModListDto
    {
        public required List<CurseForgeModDto> Mods { get; set; }
        public required CurseForgePagination Pagination { get; set; }
    }
}