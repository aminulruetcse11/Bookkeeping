using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookkeeping
{
    public class BookkeepingViewModel
    {
        public BookkeepingViewModel()
        {
            ExpenseList = new List<ExInViewModel>();
            IncomeList = new List<ExInViewModel>();
            ReconList = new List<ReconciliationVM>();
        }

        public List<ExInViewModel> ExpenseList { get; set; }
        public List<ExInViewModel> IncomeList { get; set; }
        public List<ReconciliationVM> ReconList { get; set; }
    }

    public class ExInViewModel
    {
        public int HeadID { get; set; }
        public string HeadName { get; set; }
        public int Month { get; set; }
        public decimal Income{ get; set; }
        public decimal CumulativeIncome { get; set; }
        public decimal Cost { get; set; }
        public decimal CumulativeCost { get; set; }
        public decimal Result { get; set; }
    }

    public class ReconciliationVM
    {
        public ReconciliationVM()
        {
            ReconInEXList = new List<ExInViewModel>();
        }
        public int? RECID { get; set; }
        public int HeadID { get; set; }
        public string HeadName { get; set; }
        public List<ExInViewModel> ReconInEXList{ get; set; }
    }

    public class ReconIncomeExpense
    {

    }
}