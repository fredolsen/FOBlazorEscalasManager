using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FOBlazorEscalasManager.Models
{
    public class Error
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       
        public int ERR_NUMSECUENCIAL { get; set; }
        public string Puerto { get; set; }
        public int NumeroEscala { get; set; }
        public int AnnoEscala { get; set; }
        public string Actividad { get; set; }
        public string ReferenciaFosa { get; set; }
        public string NumError { get; set; }
        public string TipoLoc { get; set; }
        public string NumPartida { get; set; }
        public string Descripcion { get; set; }
        public DateTime FUM { get; set; }

        public virtual Escala Escala { get; set; }





    }
}
