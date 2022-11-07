using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Models
{
    public class ViewListDTO<T> where T : class
    {
        public List<T> ModelDatas { get; set; } = new List<T>();
        public int MaxPage { get; set; } = 0;
        public int PageIndex { get; set; } = 0;
    }
}
