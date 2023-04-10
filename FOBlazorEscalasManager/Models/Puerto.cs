using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FOBlazorEscalasManager.Models
{
    public class Puerto
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public Int16 Autonomico { get; set; }

        public virtual List<Escala> Escala { get; set; }

    }
}
