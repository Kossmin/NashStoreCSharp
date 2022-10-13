using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public DateTime ImportedDate { get; set; }
        public string ImgUrl { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int Version { get; set; } = 1;

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }

        public virtual List<Rating> Ratings { get; set; }
    }
}
