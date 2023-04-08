using Dapper.FluentMap.Mapping;

namespace ExpensesApp.Models.Mapper {
    public class VendorMap : EntityMap<Vendor> {
        public VendorMap() {
            Map(l => l.Id).ToColumn("vendor_id");
            Map(l => l.Name).ToColumn("vendor_name");
        }
    }
}
