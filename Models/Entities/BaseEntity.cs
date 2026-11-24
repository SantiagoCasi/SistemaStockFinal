using System.ComponentModel.DataAnnotations;

namespace SistemaStock.Models.Entities
{
    public class BaseEntity
    {
        [Key]
        public int id { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
