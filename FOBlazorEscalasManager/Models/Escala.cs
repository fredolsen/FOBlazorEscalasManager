using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Composition.Convention;

namespace FOBlazorEscalasManager.Models
{
    public class Escala
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Buque { get; set; }
        [Key]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Puerto { get; set; }
        [Key]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int NumeroEscala { get; set; }
        [Key]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int AnnoEscala { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string PuertoAnterior { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string PuertoSiguiente { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? FechaAtraque { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        //public DateTime? HoraAtraque { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        //[Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? HoraLLegada { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? FechaDesatraque { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? HoraDesatraque { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Muelle { get; set; }
        [Required(ErrorMessage = "El campo CapitanSalida es requerido.")]
        public string Capitan { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string CapitanEntrada { get; set; }
        public Int16? Enviada { get; set; }
        public string Estado { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? ID_Atraque { get; set; }
        public int? N_Atraques { get; set; }
        public Int16? Practicaje { get; set; }
        public string Cod_Servicio { get; set; }
        public string DDSS { get; set; }
        public string MMCC { get; set; }
        public Int16? IMDG_Carga { get; set; }
        public Int16? IMDG_Descarga { get; set; }
        public Int16? IMDG_Transito { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? FechaSalidaPtoAnterior { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? HoraSalidaPtoAnterior { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? FechaLLegadaPtoSiguiente { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public DateTime? HoraLLegadaPtoSiguiente { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);




        public virtual List<Manifiesto> Manifiesto { get; set; }
        public virtual List<Error> Error { get; set; }
        public virtual Puerto PuertoOperacion { get; set; }  
        public virtual Barco Barco { get; set; }
    }
}
