using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DTO.Models.Statistic
{
    public class MonthlyIncomeStatisticDTO
    {
        public Dictionary<int, decimal> MonthlyIncome { get; set; }
    }
}
