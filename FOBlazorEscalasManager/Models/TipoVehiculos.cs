using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FOBlazorEscalasManager.Models
{
    public class TipoVehiculos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        public int IdVehiculo { get; set; }
        public string Puerto { get; set; }
        public string Objeto { get; set; }
        public string Descripcion { get; set; }
        public string TipoUnidadCarga { get; set; }
        public string TipoVehiculo { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? Tarifa { get; set; }
        public string VacioLLeno { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? Tarifa_L8 { get; set; }


    }
}
