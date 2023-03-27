namespace ExpensesApp.Models {
    public class Loan {
        public int Id { get; set; }
        public string Name { get; set; }
        public Payee Payee { get; set; }
        public float Amount { get; set; }
        public string Notes { get; set; }
    }
}
