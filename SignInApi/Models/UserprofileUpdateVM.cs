namespace SignInApi.Models
{
    public class UserprofileUpdateVM
    {
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int CityID { get; set; }
        public int AssemblyID { get; set; }
        public int PincodeID { get; set; }
        public int LocalityID { get; set; }
        public string Address { get; set; }
        public bool IsProfileCompleted { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string MaritalStatus { get; set; }
        public int QualificationId { get; set; }
    }
}
