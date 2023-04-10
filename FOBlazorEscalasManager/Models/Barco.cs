using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOBlazorEscalasManager.Models
{
    public class Barco
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal ID_Barco { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

        public virtual List<Escala> Escala { get; set; }


    }
}
