using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Models.enums;
using Packbuilder.Validators;

namespace Packbuilder.Models
{
    [Table("users")]
    [Index(nameof(Name), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User : ModelBase
    {
        [Required]
        [Name]
        [Column("name")]
        public required string Name { get; set; }
        [Required]
        [EmailAddress]
        [Column("email")]
        public required string Email { get; set; }
        [Required]
        [Column("image_type")]
        public required ImageType ImageType { get; set; }
        [Required]
        [Column("image_value")]
        public required string ImageValue { get; set; }
        [Column("email_verified")]
        public bool EmailVerified { get; set; } = false;
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