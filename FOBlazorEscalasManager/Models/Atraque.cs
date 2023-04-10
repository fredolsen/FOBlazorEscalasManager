using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FOBlazorEscalasManager.Models
{
    public class Atraque
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal ID_Atraque { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal ID { get; set; }
        public int N_Atraque { get; set; }
        public Int16 Praticaje { get; set; }
        [Required(ErrorMessage = "El campo muelle es requerido.")]
        public string Muelle_ID { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? Ocupacíon { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? Desocupacíon { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        [Required(ErrorMessage = "El campo tipo atraque es requerido.")]
        public string TipoAtraque { get; set; }
        public Int16? Pasajeros { get; set; }
        [StringLength(3)]
        public string Estado { get; set; }
        public string Norays { get; set; }
        public Int16 OperacionD { get; set; }
        public Int16 OperacionE { get; set; }
        [Required(ErrorMessage = "El campo calado de entrada es requerido.")]
        public Double? Calado_LLegada { get; set; }
        [Required(ErrorMessage = "El campo calado de salida es requerido.")]
        public Double? Calado_Salida { get; set; }
        [Required(ErrorMessage = "El campo tipo operación es requerido.")]
        public string TipoActividad { get; set; }
        public Int16 Electricidad { get; set; }


        //public virtual Escala Escala { get; set; }



    }
}
