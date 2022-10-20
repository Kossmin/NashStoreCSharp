using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DTO.Models.Product
{
    public class ProductDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string ImgUrl { get; set; } = "";
    }
}
