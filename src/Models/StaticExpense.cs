namespace ExpensesApp.Models {
    /// <summary>
    /// A model to represent a reoccurring expense
    /// </summary>
    public class StaticExpense {
        /// <summary>
        /// The unique ID for this reoccurring expense
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A human readable, memorable name to represent the reoccurring expense
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The date this expense was issued
        /// </summary>
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// The start of the billing cycle for this expense
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end of the billing cycle for this expense
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The amount in USD for this expense
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// (Optional) Any notes that are relevant to this expense
        /// </summary>
        /// <example>Georgia Power issued credit back on our bill</example>
        public string Notes { get; set; }
    }
}
