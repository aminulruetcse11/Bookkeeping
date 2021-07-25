using Bookkeeping.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bookkeeping
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext():
            base("ExpenseIncomeDbContext")
        {

        }

        public DbSet<IncomeExpense> IncomeExpenses { get; set; }
        public DbSet<Head> Heads { get; set; }
        public DbSet<Reconciliation> Reconciliations { get; set; }
    }
}
