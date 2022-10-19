using NashPhaseOne.DTO.Models.Authen;
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
        public UserDTO UserInfo { get; set; }
    }
}
