namespace ExpensesApp.Models {
    public class DynamicExpense {
        public int Id { get; set; }
        public Payee Payee { get; set; }
        public Vendor Location { get; set; }
        public DateTime Date { get; set; }
        public float Amount { get; set; }
        public string Notes { get; set; }
    }
}
