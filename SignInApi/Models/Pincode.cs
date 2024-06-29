namespace SignInApi.Models
{
    public class Pincode
    {
        public int PincodeID { get; set; }
        public int Number { get; set; }
        public int AssemblyID { get; set; }
        public List<Locality> Localities { get; set; }
    }
}
