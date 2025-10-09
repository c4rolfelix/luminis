using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System; // Necessário para DateTime

namespace Luminis.Models.ViewModels
{
    public class PsicologoRegisterViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O sobrenome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O sobrenome não pode exceder 100 caracteres.")]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "O CRP é obrigatório.")]
        [StringLength(10, ErrorMessage = "O CRP deve ter 7 dígitos e o formato XX/XXXXX.", MinimumLength = 8)]
        [RegularExpression(@"^\d{2}\/\d{5}$", ErrorMessage = "O CRP deve ter o formato XX/XXXXX (ex: 01/12345).")]
        [Display(Name = "Número do CRP")]
        public string CRP { get; set; }

        // >>> NOVO: CPF (resolve CS1061) <<<
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(14, ErrorMessage = "O CPF deve ter o formato 000.000.000-00", MinimumLength = 14)]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "O CPF deve ter o formato 000.000.000-00.")]
        [Display(Name = "CPF")]
        public string CPF { get; set; }


        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,100}$",
            ErrorMessage = "A senha deve ter pelo menos 8 caracteres, incluindo letra maiúscula, minúscula, número e caractere especial.")]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Senha", ErrorMessage = "A senha e a confirmação de senha não coincidem.")]
        public string ConfirmarSenha { get; set; }

        [Required(ErrorMessage = "O número de WhatsApp é obrigatório.")]
        [Display(Name = "Número de WhatsApp")]
        public string WhatsApp { get; set; }

        [Url(ErrorMessage = "A URL do WhatsApp precisa ser um endereço válido.")]
        [Display(Name = "Link de Agendamento do WhatsApp")]
        public string? WhatsAppUrl { get; set; } // Adicionado para consistência (nulável)

        // >>> NOVO: Data de Nascimento (resolve CS1061) <<<
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }
    }
}
