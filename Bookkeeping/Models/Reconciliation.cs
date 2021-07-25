using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Bookkeeping.EnumData.Enum;

namespace Bookkeeping.Models
{
    public class Reconciliation
    {
        [Key]
        public int RecID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int HeadID { get; set; }
        public virtual Head Head { get; set; }
    }
}
