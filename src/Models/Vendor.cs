namespace ExpensesApp.Models {
    /// <summary>
    /// A model representing a vendor in which a dynamic expense took place at
    /// </summary>
    public class Vendor {
        /// <summary>
        /// This vendor's unique, internal ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The vendor's official name
        /// </summary>
        public string Name { get; set; }
    }
}
