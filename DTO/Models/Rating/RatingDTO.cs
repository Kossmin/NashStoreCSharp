using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DTO.Models.Rating
{
    public class RatingDTO
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public RatingStar Star { get; set; }
        public string Comment { get; set; }
    }
}
