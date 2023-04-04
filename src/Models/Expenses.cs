namespace ExpensesApp.Models {
    public class Expenses {
        public Payee Payee { get; set; }
        public float DynamicExpenses { get; set; }
        public float Loans { get; set; }
        public float StaticExpenses { get; set; }
        public float Payments { get; set; }
        public float Balance { get; set; }
    }
}
