using Dapper.FluentMap.Mapping;

namespace ExpensesApp.Models.Mapper {
    public class StaticExpenseMapper : EntityMap<StaticExpense> {
        public StaticExpenseMapper() {
            Map(se => se.Id).ToColumn("se_id");
            Map(se => se.Name).ToColumn("se_name");
            Map(se => se.IssueDate).ToColumn("issue_date");
            Map(se => se.StartDate).ToColumn("start_date");
            Map(se => se.EndDate).ToColumn("end_date");
            Map(se => se.Amount).ToColumn("amount");
            Map(se => se.Notes).ToColumn("notes");
        }
    }
}
