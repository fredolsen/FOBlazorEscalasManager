using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOBlazorEscalasManager.Models
{
    public class CalendarioCapitan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Buque { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Capitan { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? Al_Mando_Desde { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? Al_Mando_Hasta { get; set; }
        public Int16 Practicaje { get; set; }

    }
}
