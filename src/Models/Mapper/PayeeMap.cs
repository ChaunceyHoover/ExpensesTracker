using Dapper.FluentMap.Mapping;

namespace ExpensesApp.Models.Mapper {
    public class PayeeMap : EntityMap<Payee> {
        public PayeeMap() {
            Map(p => p.Id).ToColumn("payee_id");
            Map(p => p.Name).ToColumn("payee_name");
        }
    }
}
