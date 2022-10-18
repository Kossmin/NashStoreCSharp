using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Models.Authen
{
    public class Token
    {
        public string TokenString { get; set; }
        public DateTime Expiration { get; set; }
        public List<string> Roles { get; set; }
        public string UserId { get; set; }
    }
}
