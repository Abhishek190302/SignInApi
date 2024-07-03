namespace SignInApi.Models
{
    public class PaymentModeViewModel
    {
        public bool Cash { get; set; }
        public bool Cheque { get; set; }
        public bool RtgsNeft { get; set; }
        public bool DebitCard { get; set; }
        public bool CreditCard { get; set; }
        public bool NetBanking { get; set; }
        public bool SelectAll { get; set; }
    }
}
