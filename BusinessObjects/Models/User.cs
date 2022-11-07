using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.BusinessObjects.Models
{
    public class User : IdentityUser
    {
        public bool IsBanned { get; set; } = false;

        public virtual List<Order> Orders { get; set; }

        public virtual List<Rating> Ratings { get; set; }
    }
}
