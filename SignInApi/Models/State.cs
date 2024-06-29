namespace SignInApi.Models
{
    public class State
    {
        public int StateID { get; set; }
        public string Name { get; set; }
        public int CountryID { get; set; }
        public List<City> Cities { get; set; }
    }
}
