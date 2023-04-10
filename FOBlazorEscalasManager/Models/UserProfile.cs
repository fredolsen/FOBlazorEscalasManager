using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FOBlazorEscalasManager.Models
{
    public class UserProfile
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       
        public int UserId { get; set; }
        public string UserName { get; set; }

    }
}
