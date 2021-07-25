using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Bookkeeping.EnumData.Enum;

namespace Bookkeeping.Models
{
    public class IncomeExpense
    {
        [Key]
        public int IEID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public HeadType Type { get; set; }
    }
}
