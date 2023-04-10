using FOBlazorEscalasManager.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace FOBlazorEscalasManager.Models
{
    public class Capitan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal Id_Capitan { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido.")]
        public string Nombre { get; set; }
        public Int16 Baja { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(2,ErrorMessage = "El campo {0} no puede tener una longitud mayor de 2.")]
        public string Nacionalidad { get; set; }

        public virtual Pais Pais { get; set; }

    }
}
