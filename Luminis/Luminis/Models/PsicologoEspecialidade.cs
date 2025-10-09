using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Luminis.Models
{
    public class PsicologoEspecialidade
    {
        [Required]
        [Column(Order = 0)] // Remova [Key]
        public int PsicologosId { get; set; }

        [Required]
        [Column(Order = 1)] // Remova [Key]
        public int EspecialidadesId { get; set; }
    }
}