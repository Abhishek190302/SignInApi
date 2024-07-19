namespace SignInApi.Models
{
    public class ProfileInfo
    {
        public UserprofileUpdateVM UserProfile { get; set; }
        public bool IsVendor { get; set; }
        public List<Qualification> Qualifications { get; set; }
        public List<Country> Countries { get; set; }
        public List<State> States { get; set; }
        public List<City> Cities { get; set; }
        public List<Assembly> Localities { get; set; }
        public List<Pincode> Pincodes { get; set; }      
        public List<Locality> Areas { get; set; } 
    }
}
