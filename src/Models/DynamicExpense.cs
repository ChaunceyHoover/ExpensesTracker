namespace ExpensesApp.Models {
    /// <summary>
    /// A model representing lent money for a one-off expense that doesn't happen on a consistent basis
    /// </summary>
    public class DynamicExpense {
        /// <summary>
        /// A unique, internal ID for this expense
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The person who paid for this expense
        /// </summary>
        public Payee Payee { get; set; }

        /// <summary>
        /// The location this expense was made at
        /// </summary>
        public Vendor Location { get; set; }

        /// <summary>
        /// The date this expense was made at
        /// </summary>
        /// <remarks>The `Time` component of this value is irrelevant - .NET or Dapper doesn't support `DateOnly` yet</remarks>
        public DateTime Date { get; set; }

        /// <summary>
        /// The amount spent on this purchase in USD
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// (Optional) Any notes about this transaction
        /// </summary>
        public string Notes { get; set; }
    }
}
