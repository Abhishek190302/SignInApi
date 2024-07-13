namespace SignInApi.Models
{
    public class AddressVM
    {
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int CityID { get; set; }
        public int AssemblyID { get; set; }
        public int PincodeID { get; set; }
        public int LocalityID { get; set; }
        public string LocalAddress { get; set; }
    }
}
