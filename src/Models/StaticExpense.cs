namespace ExpensesApp.Models {
    public class StaticExpense {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float Amount { get; set; }
        public string Notes { get; set; }
    }
}
