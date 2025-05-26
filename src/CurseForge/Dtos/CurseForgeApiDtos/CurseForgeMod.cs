namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeMod
    {
        public required string ReferenceId { get; set; }
        public required string Name { get; set; }
        public required string Summary { get; set; }
        public required int DownloadCount { get; set; }
        public required string DateModified { get; set; }
        public required string Slug { get; set; }
        public required string WebsiteLink { get; set; }
        public required int ClassId { get; set; }
        public string? LogoUrl { get; set; }
        public List<CurseForgeAuthor> Authors { get; set; } = [];

        public static CurseForgeMod FromSummary(CurseForgeModSummary modSummary)
        {
            return new CurseForgeMod
            {
                ReferenceId = modSummary.Id.ToString(),
                Name = modSummary.Name,
                Summary = modSummary.Summary,
                DownloadCount = modSummary.DownloadCount,
                DateModified = modSummary.DateModified,
                Slug = modSummary.Slug,
                WebsiteLink = modSummary.Links.WebsiteUrl,
                LogoUrl = modSummary.Logo?.Url,
                Authors = modSummary.Authors,
                ClassId = modSummary.ClassId
            };
        }
    }

}