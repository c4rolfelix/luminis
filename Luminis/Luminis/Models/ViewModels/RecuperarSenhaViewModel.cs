using System.ComponentModel.DataAnnotations;

namespace Luminis.Models.ViewModels
{
    public class RecuperarSenhaViewModel
    {
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo CPF é obrigatório.")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "O CPF deve conter 11 dígitos.")]
        public string CPF { get; set; }

        public bool Validado { get; set; } = false;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Informe a nova senha.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
    ErrorMessage = "A senha deve ter no mínimo 8 caracteres, incluindo maiúscula, minúscula, número e caractere especial.")]
        public string NovaSenha { get; set; }

        [Required(ErrorMessage = "Confirme a nova senha.")]
        [Compare("NovaSenha", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmarSenha { get; set; }

        public string? UserId { get; set; }
    }
}