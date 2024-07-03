namespace SignInApi.Models
{
    public class PaymentMode
    {
        public int ListingID { get; set; }
        public string OwnerGuid { get; set; }
        public string IPAddress { get; set; }
        public bool Cash { get; set; }
        public bool Cheque { get; set; }
        public bool RtgsNeft { get; set; }
        public bool DebitCard { get; set; }
        public bool CreditCard { get; set; }
        public bool NetBanking { get; set; }
        public bool PayTM { get; set; }
        public bool PhonePay { get; set; }
        public bool Paypal { get; set; }
    }
}
