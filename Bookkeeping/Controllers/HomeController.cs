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
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            List<Reconciliation> reconciliations = new List<Reconciliation>();
            Reconciliation reconciliation = null;
            try
            {

                var Incomes = formCollection.AllKeys.Where(k => k.Contains("Income"));
                int HeadID = 0, counter = 1, month = 0, year = 0;
                decimal amount = 0m;
                year = Convert.ToInt32(formCollection["ddYear"]);
                foreach (var item in Incomes)
                {
                    if (item.Contains("Income_Head"))
                    {
                        HeadID = Convert.ToInt32(formCollection[item]);
                        counter = 1;
                        continue;
                    }
                    else
                        amount = Convert.ToDecimal(formCollection[item]);


                    reconciliation = new Reconciliation();

                    reconciliation.HeadID = HeadID;
                    if (counter == 12)
                        month = counter;
                    else
                        month = counter % 12;


                    reconciliation.Date = new DateTime(year, month, 1, 0, 0, 0);
                    reconciliation.Amount = amount;
                    if (amount != 0)
                        reconciliations.Add(reconciliation);
                    counter++;
                }

                var expense = formCollection.AllKeys.Where(k => k.Contains("Expense"));
                HeadID = 0;
                counter = 1;
                month = 0;
                amount = 0m;
                foreach (var item in expense)
                {
                    if (item.Contains("Expense_Head"))
                    {
                        HeadID = Convert.ToInt32(formCollection[item]);
                        counter = 1;
                        continue;
                    }
                    else
                        amount = Convert.ToDecimal(formCollection[item]);


                    reconciliation = new Reconciliation();

                    reconciliation.HeadID = HeadID;
                    if (counter == 12)
                        month = counter;
                    else
                        month = counter % 12;


                    reconciliation.Date = new DateTime(year, month, 1, 0, 0, 0);
                    reconciliation.Amount = amount;
                    if (amount != 0)
                        reconciliations.Add(reconciliation);
                    counter++;
                }
                if (reconciliations.Count() > 0)
                {
                    decimal newAmount = 0m;
                    Reconciliation oldrecon = null;
                    var olddata = _dbContext.Reconciliations.Where(i => i.Date.Year == year);
                    foreach (var item in reconciliations)
                    {
                        if (olddata.Any(i => i.Date == item.Date && i.HeadID == item.HeadID))
                        {
                            oldrecon = olddata.FirstOrDefault(i => i.Date == item.Date && i.HeadID == item.HeadID);
                            oldrecon.Amount = item.Amount;
                        }
                        else
                        {
                            _dbContext.Reconciliations.Add(item);
                        }
                    }

                    if (_dbContext.SaveChanges() > 0)
                        ViewBag.msg = "Save successfull";
                    else
                        ViewBag.msg = "Save failed.";
                }

            }
            catch (Exception ex)
            {
                ViewBag.msg = "Save failed." + ex.Message;
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetIncomeCost(int Year)
        {
            var incomeExp = GetCostAndIncome(Year);
            var reconIncome = GetReconciliationData(Year, HeadType.Income);
            var reconExpense = GetReconciliationData(Year, HeadType.Expense);
            ReconciliationVM reconciliationResult = CalculateReconciliationResult(reconIncome, reconExpense);
            reconExpense.Add(reconciliationResult);

            return Json(new { inex = incomeExp, reconIncome, reconExpense }, JsonRequestBehavior.AllowGet);

        }

        private ReconciliationVM CalculateReconciliationResult(List<ReconciliationVM> reconIncome, List<ReconciliationVM> reconExpense)
        {
            var reconciliationVM = new ReconciliationVM();
            reconciliationVM.HeadName = "Reconciliation Result";
            decimal Income = 0m, Cost = 0m;
            for (int j = 1; j <= 12; j++)
            {

                Income = 0m;
                Cost = 0m;
                foreach (var item in reconIncome)
                {
                    Income += item.ReconInEXList.Sum(o => (o.Month == j) ? o.Income : 0m);
                }
                foreach (var item in reconExpense)
                {
                    Cost += item.ReconInEXList.Sum(o => (o.Month == j) ? o.Income : 0m);
                }
                reconciliationVM.ReconInEXList.Add(new ExInViewModel { Income = Income - Cost, Month = j });
            }

            return reconciliationVM;
        }

        private List<ExInViewModel> GetCostAndIncome(int Year)
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

        private List<ReconciliationVM> GetReconciliationData(int Year, HeadType type)
        {
            List<ReconciliationVM> reconList = new List<ReconciliationVM>();
            ReconciliationVM reconciliationVM = null;
            var heads = _dbContext.Heads.Where(i => i.Type == type);
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
    }
}