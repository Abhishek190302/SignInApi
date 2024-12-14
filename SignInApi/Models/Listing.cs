namespace SignInApi.Models
{
    public class Listing
    {
        public int Listingid { get; set; }
        public string OwnerGuid { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public string IPAddress { get; set; }
        public int Status { get; set; }
        public string CompanyName { get; set; }
        public string BusinessCategory { get; set; }
        public string NatureOfBusiness { get; set; }
        public DateTime? YearOfEstablishment { get; set; }
        public int? NumberOfEmployees { get; set; }
        public string Turnover { get; set; }
        public string GSTNumber { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Designation { get; set; }
        public string ListingURL { get; set; }
        public bool ApprovedOrRejectedBy { get; set; }
        public bool Rejected { get; set; }
        public int Steps { get; set; }
        public Guid Id { get; set; }
        public bool ClaimedListing { get; set; }
        public bool SelfCreated { get; set; }
    }
}
