namespace ExpensesApp.Models {
    /// <summary>
    /// A model representing someone who lent money
    /// </summary>
    public class Payee {
        /// <summary>
        /// A unique, internal identifier for the lender
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The lender's name
        /// </summary>
        public string Name { get; set; }
    }
}
