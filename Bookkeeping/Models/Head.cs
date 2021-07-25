using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Bookkeeping.EnumData.Enum;

namespace Bookkeeping.Models
{
    public class Head
    {
        [Key]
        public int HeadID { get; set; }
        [StringLength(300)]
        public string Name { get; set; }
        public HeadType Type { get; set; }
    }
}
