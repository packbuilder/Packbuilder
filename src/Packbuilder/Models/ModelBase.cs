using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Packbuilder.Models
{
    public class ModelBase
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public void Bump()
        {
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}