using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DTO.Models.Order
{
    public class AdminGetOrderDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string Status { get; set; }

        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
