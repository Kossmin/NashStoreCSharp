using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.BusinessObjects.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public OrderStatus Status { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }
    }

    public enum OrderStatus
    {
        Ordering,
        Done,
        Delivering,
        Delivered,
        Pending,
        Canceled
    }
}
