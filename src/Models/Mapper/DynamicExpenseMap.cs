using Dapper.FluentMap.Mapping;

namespace ExpensesApp.Models.Mapper {
    public class DynamicExpenseMap : EntityMap<DynamicExpense> {
        public DynamicExpenseMap() {
            Map(de => de.Id).ToColumn("de_id");
            Map(de => de.Date).ToColumn("date");
            Map(de => de.Amount).ToColumn("amount");
            Map(de => de.Notes).ToColumn("notes");
        }
    }
}
