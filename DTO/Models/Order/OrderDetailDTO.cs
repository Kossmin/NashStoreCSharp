using DTO.Models;
using NashPhaseOne.DTO.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DTO.Models.Order
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public ProductDetailDTO Product{ get; set; }
    }
}
