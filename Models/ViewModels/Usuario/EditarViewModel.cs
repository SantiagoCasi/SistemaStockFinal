using System.ComponentModel.DataAnnotations;

namespace SistemaStock.Models.ViewModels.Usuario
{
    public class EditarViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre{ get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio."), EmailAddress(ErrorMessage = "El email no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un rol")]
        public string Rol { get; set; } = string.Empty;

        public bool Activo { get; set; }
    }
}
