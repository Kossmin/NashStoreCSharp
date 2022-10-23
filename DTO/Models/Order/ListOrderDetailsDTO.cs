using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DTO.Models.Order
{
    public class ListOrderDetailsDTO
    {
        public List<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
    }
}
