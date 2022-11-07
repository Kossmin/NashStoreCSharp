using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DTO.Models.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public DateTime ImportedDate { get; set; }
        public List<string> ImgUrls { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int Version { get; set; } = 1;
        public string CategoryName { get; set; }
    }
}
