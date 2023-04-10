using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FOBlazorEscalasManager.Models
{
    public class Manifiesto
    {
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
        [Key]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int Orden { get; set; }
        [Key]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Actividad { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string PuertoAnterior { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string PuertoSiguiente { get; set; }
        public int Conocimiento { get; set; }
        public int Partida { get; set; }
        public string TipoObjeto { get; set; }
        public string TipoUnidadCarga { get; set; }
        public string CodigoArancelario { get; set; }
        public int? Cantidad { get; set; }
        public string Matricula { get; set; }
        public int? Tara { get; set; }
        public int? Carga { get; set; }
        public double? Largo { get; set; }
        public string Mercancía { get; set; }
        public string Tipo_CON { get; set; }      
        public string TipoPartida { get; set; }
        public string Documento { get; set; }
        public string Precinto { get; set; }
        public string CNI_Embarque { get; set; }

        public virtual Escala Escala { get; set; }
       

    }
}
