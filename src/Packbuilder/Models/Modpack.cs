using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace Packbuilder.Models
{
    [Table("modpack")]
    public partial class Modpack : ModelBase
    {
        [MaxLength(20)]
        [Column("name")]
        public required string Name { get; set; }
        [Column("avatar")]
        public string? Avatar { get; set; }
        [Column("slug")]
        public required string Slug { get; set; }
        [Column("user_id")]
        public required int UserId { get; set; }
        public ICollection<Version> Versions { get; set; } = [];

        public User User { get; set; } = null!;

        public static string GenerateSlug(string name)
        {
            string slug = SlugRegex().Replace(name, "");
            slug = slug.Trim().Replace(' ', '-').ToLower();

            return slug;
        }

        [GeneratedRegex(@"[^a-z0-9-]")]
        private static partial Regex SlugRegex();
    }
}