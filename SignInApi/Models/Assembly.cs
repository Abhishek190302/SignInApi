namespace SignInApi.Models
{
    public class Assembly
    {
        public int AssemblyID { get; set; }
        public string Name { get; set; }
        public int CityID { get; set; }
        public List<Pincode> Pincodes { get; set; }
    }
}
