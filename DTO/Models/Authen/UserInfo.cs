using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DTO.Models.Authen
{
    public class UserInfo
    {
        public string Email { get; set; }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public bool IsBanned { get; set; }
    }
}
