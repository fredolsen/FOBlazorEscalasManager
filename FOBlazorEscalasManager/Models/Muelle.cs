using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOBlazorEscalasManager.Models
{
    public class Muelle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "decimal(18, 0)")]
        public int ID { get; set; }
        public string SUBPUERTO { get; set; }
        public string SUBPUERTO_DESC { get; set; }
        public string CODIGO_AP { get; set; }
        public string MUELLE_ID { get; set; }
        public string MUELLE_DESC { get; set; }
        public string F6 { get; set; }
        public string BUQUE { get; set; }
        public string TIPOATRAQUE { get; set; }
        public string NORAYS { get; set; }
        public Int16 ELECTRICIDAD { get; set; }


    }
}
