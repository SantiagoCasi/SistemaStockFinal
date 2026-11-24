using System.ComponentModel.DataAnnotations;

namespace SistemaStock.Models.ViewModels
{
    public class ProveedorViewModel
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio"), StringLength(120,
        ErrorMessage = "Máximo 120 caracteres")]
        [Display(Name = "Nombre / Razón Social")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Contacto")]
        public string? Contacto { get; set; }

        [StringLength(30, ErrorMessage = "Máximo 30 caracteres")]
        public string? Documento { get; set; }

        [EmailAddress(ErrorMessage = "El email no es válido"), StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string? Email { get; set; }

        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "El teléfono no es válido"), StringLength(30, ErrorMessage = "Máximo 30 caracteres")]
        public string? Telefono { get; set; }

        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [StringLength(300, ErrorMessage = "Máximo 300 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Actualización")]
        public DateTime? UpdatedAt { get; set; }
    }
}
