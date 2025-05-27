using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Packbuilder.Models
{
    [Table("users")]
    public class User : ModelBase
    {
        [MaxLength(20)]
        [Column("name")]
        public required string Name { get; set; }
        [Column("email")]
        public string? Email { get; set; }
        [Column("avatar")]
        public required string Avatar { get; set; }
        [Column("password_digest")]
        [JsonIgnore]
        [Required]
        public string PasswordDigest { get; set; } = "";

        public void SetPassword(IPasswordHasher<User> passwordHasher, string password)
        {
            this.PasswordDigest = passwordHasher.HashPassword(this, password);
        }

        public PasswordVerificationResult VerifyPassword(IPasswordHasher<User> passwordHasher, string password)
        {
            return passwordHasher.VerifyHashedPassword(this, this.PasswordDigest, password);
        }
    }
}