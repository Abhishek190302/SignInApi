namespace SignInApi.Models
{
    public class SavePincodesModel
    {
        public int ListingID { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int CityID { get; set; }
        public int AssemblyID { get; set; }
        public List<int> SelectedPincodesContainer { get; set; }
    }
}
