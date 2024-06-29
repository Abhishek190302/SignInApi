namespace SignInApi.Models
{
    public class Country
    {
        public int CountryID { get; set; }
        public string Name { get; set; }
        public List<State> States { get; set; }
    }
}
