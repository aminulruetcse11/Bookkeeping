using Bookkeeping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Bookkeeping.EnumData.Enum;

namespace Bookkeeping.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController()
        {
            _dbContext = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public JsonResult GetIncomeExpense(int Year)
        {
            var incomeExp = GetExIn(Year);
            var reconIncome = GetReconciliationIncome(Year);
            var reconExpense = GetReconciliationExpense(Year);

            var reconciliationVM = new ReconciliationVM();
            reconciliationVM.HeadName = "Reconciliation Result";
            decimal Income = 0m,Cost=0m;
            for (int j = 1; j <= 12; j++)
            {
                Income = 0m;
                Cost=0m;
                foreach (var item in reconIncome)
                {
                    Income += item.ReconInEXList.Sum(o => (o.Month == j) ? o.Income : 0m);
                }
                foreach (var item in reconExpense)
                {
                    Cost += item.ReconInEXList.Sum(o => (o.Month == j) ? o.Income : 0m);
                }
                reconciliationVM.ReconInEXList.Add(new ExInViewModel { Income = Income-Cost, Month = j });
            }
            reconExpense.Add(reconciliationVM);

            return Json(new { inex = incomeExp, reconIncome, reconExpense }, JsonRequestBehavior.AllowGet);

        }


        private List<ExInViewModel> GetExIn(int Year)
        {
            List<ExInViewModel> exInViewModels = new List<ExInViewModel>();
            ExInViewModel exInView = null;

            var incomeExp = (from ie in _dbContext.IncomeExpenses
                             where ie.Date.Year == Year
                             group ie by new { ie.Date.Month, ie.Type } into g
                             select new
                             {
                                 Amount = g.Sum(i => i.Amount),
                                 Type = g.Key.Type,
                                 Month = g.Key.Month
                             }).OrderBy(i => i.Month).ToList();

            decimal CumulativeIncome = 0m, CumuExpense = 0m;
            for (int i = 1; i <= 12; i++)
            {
                exInView = new ExInViewModel();
                exInView.Income = incomeExp.Sum(o => (o.Type == HeadType.Income && o.Month == i) ? o.Amount : 0m);
                CumulativeIncome += exInView.Income;
                exInView.CumulativeIncome = CumulativeIncome;

                exInView.Cost = incomeExp.Sum(o => (o.Type == HeadType.Expense && o.Month == i) ? o.Amount : 0m);
                CumuExpense += exInView.Cost;
                exInView.CumulativeCost = CumuExpense;

                exInView.Month = i;
                exInView.Result = exInView.Income - exInView.Cost;
                exInViewModels.Add(exInView);
            }

            return exInViewModels;
        }


        private List<ReconciliationVM> GetReconciliationIncome(int Year)
        {
            List<ReconciliationVM> reconList = new List<ReconciliationVM>();
            ReconciliationVM reconciliationVM = null;
            var heads = _dbContext.Heads.Where(i => i.Type == HeadType.Income);
            var reconciliations = (from h in heads
                                   join rec in _dbContext.Reconciliations on h.HeadID equals rec.HeadID into lj
                                   from rec in lj.DefaultIfEmpty()
                                   where rec.Date.Year == Year
                                   select new
                                   {
                                       h.HeadID,
                                       RecID = rec != null ? rec.RecID : 0,
                                       HeadName = h.Name,
                                       Month = rec != null ? rec.Date.Month : 1,
                                       Amount = rec != null ? rec.Amount : 0m,
                                       h.Type
                                   }).ToList();

            var groupData = (from r in reconciliations
                             group r by new
                             {
                                 r.HeadID,
                                 r.HeadName,
                                 r.Month,
                                 r.Type
                             } into g
                             select new
                             {
                                 g.Key.HeadName,
                                 g.Key.HeadID,
                                 g.Key.Month,
                                 g.Key.Type,
                                 Amount = g.Sum(i => i.Amount)
                             }).ToList();
            decimal Income = 0m;
            foreach (var item in heads)
            {
                reconciliationVM = new ReconciliationVM();
                reconciliationVM.HeadID = item.HeadID;
                reconciliationVM.HeadName = item.Name;
                for (int i = 1; i <= 12; i++)
                {
                    Income = groupData.Sum(o => (o.Month == i && o.HeadID == item.HeadID) ? o.Amount : 0m);
                    reconciliationVM.ReconInEXList.Add(new ExInViewModel { Income = Income, Month = i });
                }
                reconList.Add(reconciliationVM);
            }


            return reconList;

        }

        private List<ReconciliationVM> GetReconciliationExpense(int Year)
        {
            List<ReconciliationVM> reconList = new List<ReconciliationVM>();
            ReconciliationVM reconciliationVM = null;
            var reconciliations = (from h in _dbContext.Heads
                                   join rec in _dbContext.Reconciliations on h.HeadID equals rec.HeadID into lj
                                   from rec in lj.DefaultIfEmpty()
                                   where rec.Date.Year == Year && h.Type == HeadType.Expense
                                   select new
                                   {
                                       h.HeadID,
                                       RecID = rec != null ? rec.RecID : 0,
                                       HeadName = h.Name,
                                       Month = rec != null ? rec.Date.Month : 1,
                                       Amount = rec != null ? rec.Amount : 0m,
                                       h.Type
                                   }).ToList();

            var groupData = (from r in reconciliations
                             group r by new
                             {
                                 r.HeadID,
                                 r.HeadName,
                                 r.Month,
                                 r.Type
                             } into g
                             select new
                             {
                                 g.Key.HeadName,
                                 g.Key.HeadID,
                                 g.Key.Month,
                                 g.Key.Type,
                                 Amount = g.Sum(i => i.Amount)
                             }).ToList();
            decimal Income = 0m;
            foreach (var item in groupData)
            {
                reconciliationVM = new ReconciliationVM();
                reconciliationVM.HeadID = item.HeadID;
                reconciliationVM.HeadName = item.HeadName;
                for (int i = 1; i <= 12; i++)
                {
                    Income = groupData.Sum(o => (o.Month == i && o.HeadID == item.HeadID) ? o.Amount : 0m);
                    reconciliationVM.ReconInEXList.Add(new ExInViewModel { Income = Income, Month = i });
                }
                reconList.Add(reconciliationVM);
            }

            return reconList;

        }
    }
}