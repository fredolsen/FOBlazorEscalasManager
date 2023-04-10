using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOBlazorEscalasManager.Models
{
    public class EstadoEscala
    {
        public string EST_CCOEstado { get; set; }
        public string EST_Descripcion { get; set; }
    }
}
