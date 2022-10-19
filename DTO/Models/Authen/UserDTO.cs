using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DTO.Models.Authen
{
    public class UserDTO
    {
        public UserDTO()
        {

        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
    }
}
