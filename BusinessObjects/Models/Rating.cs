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
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public RatingStar Star { get; set; }
        public string Comment { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }

    public enum RatingStar
    {
        VeryUnsatisfied = 1,
        Unsatisfied,
        Neutral,
        Satisfied,
        VerySatisfied
    }
}
