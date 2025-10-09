using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;

namespace Luminis.Models.ViewModels
{
    public class PsicologoEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O sobrenome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O sobrenome não pode exceder 100 caracteres.")]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "O número de CRP é obrigatório.")]
        [StringLength(10, ErrorMessage = "O CRP deve ter 7 dígitos e o formato XX/XXXXX.", MinimumLength = 8)]
        [RegularExpression(@"^\d{2}\/\d{5}$", ErrorMessage = "O CRP deve ter o formato XX/XXXXX (ex: 01/12345).")]
        [Display(Name = "Número do CRP")]
        public string CRP { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; }

        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [StringLength(1500, ErrorMessage = "A biografia não pode exceder 1500 caracteres.")]
        [DataType(DataType.MultilineText)]
        public string? Biografia { get; set; }

        [Display(Name = "URL da Foto de Perfil")]
        public string? FotoUrl { get; set; }

        [Display(Name = "Arquivo da Foto de Perfil")]
        public IFormFile? ProfileImageFile { get; set; }

        [Required(ErrorMessage = "O número de WhatsApp é obrigatório.")]
        [Display(Name = "Número de WhatsApp")]
        public string WhatsApp { get; set; }

        [Url(ErrorMessage = "O link para agendamento precisa ser uma URL válida (ex: https://wa.me/...).")]
        [Display(Name = "Link para Agendamento via WhatsApp")]
        public string? WhatsAppUrl { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [Display(Name = "Especialidades")]
        public List<int>? EspecialidadesSelecionadasIds { get; set; }
    }
}