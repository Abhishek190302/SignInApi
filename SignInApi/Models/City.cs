namespace SignInApi.Models
{
    public class City
    {
        public int CityID { get; set; }
        public string Name { get; set; }
        public int StateID { get; set; }
        public List<Assembly> Assemblies { get; set; }
    }
}
