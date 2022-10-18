using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Models
{
    public class ViewListDTO<T> where T : class
    {
        public List<T> ModelDatas { get; set; }
        public int MaxPage { get; set; }
        public int PageIndex { get; set; }
    }
}
