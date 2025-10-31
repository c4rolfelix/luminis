using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Luminis.Models
{
    public class Plano
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; } // "Plano Mensal"
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Preco { get; set; } // 50.00
        public string Periodo { get; set; } // "mês", "trimestre", "ano"
        public string Descricao { get; set; }
        public string Recursos { get; set; } // "Perfil público;Busca por especialidade"
        public bool Destaque { get; set; } = false; // Se é o plano "Mais Popular"
    }
}