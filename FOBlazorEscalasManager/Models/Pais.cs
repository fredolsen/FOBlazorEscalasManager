using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FOBlazorEscalasManager.Models
{
    public class Pais
    {

        [Key]       
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

        public virtual List<Capitan> Capitan { get; set; }

    }
}
