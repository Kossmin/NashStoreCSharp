using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Models
{
    public class ViewProductModel
    {
        public Product Product { get; set; }
        public string CategoryName { get; set; }
    }
}
