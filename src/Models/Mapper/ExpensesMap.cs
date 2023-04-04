using Dapper.FluentMap.Mapping;

namespace ExpensesApp.Models.Mapper {
    public class ExpensesMap : EntityMap<Expenses> {
        public ExpensesMap() {
            Map(e => e.DynamicExpenses).ToColumn("dynamic_expenses");
            Map(e => e.Loans).ToColumn("loans");
            Map(e => e.StaticExpenses).ToColumn("static_expenses");
            Map(e => e.Payments).ToColumn("payments");
        }
    }
}
