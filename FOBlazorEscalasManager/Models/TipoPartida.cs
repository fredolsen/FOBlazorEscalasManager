using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FOBlazorEscalasManager.Models
{
    public class TipoPartida
    {
        [Key]
        public string Tipo { get; set; }
        public string Descripcion { get; set; }

    }
}
