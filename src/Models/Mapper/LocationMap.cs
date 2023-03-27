using Dapper.FluentMap.Mapping;

namespace ExpensesApp.Models.Mapper {
    public class LocationMap : EntityMap<Location> {
        public LocationMap() {
            Map(l => l.Id).ToColumn("location_id");
            Map(l => l.Name).ToColumn("location_name");
        }
    }
}
