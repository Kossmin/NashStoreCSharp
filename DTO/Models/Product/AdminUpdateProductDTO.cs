using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DTO.Models.Product
{
    public class AdminUpdateProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime ImportedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int Version { get; set; }
        public IEnumerable<IFormFile>? Imgs { get; set; } = null;
    }
}
