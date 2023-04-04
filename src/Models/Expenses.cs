namespace ExpensesApp.Models {
    /// <summary>
    /// Model representing all expenses from a given Payee
    /// </summary>
    public class Expenses {
        /// <summary>
        /// The owner of these expenses
        /// </summary>
        public Payee Payee { get; set; }

        /// <summary>
        /// Amount owed in shared / split dynamic expenses (ex: groceries, food, etc.)
        /// </summary>
        public float DynamicExpenses { get; set; }

        /// <summary>
        /// Amount owed in one time loans (ex: borrowed money)
        /// </summary>
        public float Loans { get; set; }

        /// <summary>
        /// Amount owed in reoccurring expenses (ex: rent, utilities, etc.)
        /// </summary>
        public float StaticExpenses { get; set; }

        /// <summary>
        /// Total amount paid for all expenses
        /// </summary>
        public float Payments { get; set; }

        /// <summary>
        /// A dynamic field that calculates the remaining amount due to pay
        /// </summary>
        /// <remarks>Calculated with the following formula: <code>Dynamic Expenses + Loans + Static Expenses - Payments</code></remarks>
        public float Balance {
            get {
                return DynamicExpenses + Loans + StaticExpenses - Payments;
            }
        }
    }
}
