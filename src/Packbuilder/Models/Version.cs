using System.ComponentModel.DataAnnotations.Schema;

namespace Packbuilder.Models
{
    [Table("versions")]
    public class Version : BaseModel
    {
        [Column("modpacks")]
        public required Modpack Modpack { get; set; }
        [Column("iterations")]
        public required int Iteration { get; set; }
    }
}