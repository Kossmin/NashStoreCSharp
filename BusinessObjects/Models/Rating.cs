using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class Rating
    {
        public string UserID { get; set; }
        public int ProductID { get; set; }
        public RatingStar Star { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }

    public enum RatingStar
    {
        VeryUnsatisfied,
        Unsatisfied,
        Neutral,
        Satisfied,
        VerySatisfied
    }
}
