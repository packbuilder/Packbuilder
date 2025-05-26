namespace CurseForge.Dtos.Create
{
    public class CreateModListDto
    {
        public required List<string> ModIds { get; set; }
        public required bool FilterPcOnly { get; set; }
    }
}