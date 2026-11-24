using System.ComponentModel.DataAnnotations;

namespace SistemaStock.Models.ViewModels.Usuario
{
    public class ResetearPasswordViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Requerido"), StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NuevaPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Requerido")]
        [DataType(DataType.Password)]
        [Compare(nameof(NuevaPassword), ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}
