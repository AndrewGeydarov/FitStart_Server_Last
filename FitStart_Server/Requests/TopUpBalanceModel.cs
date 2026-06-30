namespace FitStart_Server.Requests
{
    public class TopUpBalanceModel
    {
        public int UserID { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
    }
}
