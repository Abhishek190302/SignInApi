namespace SignInApi.Models
{
    public class AreaCreateRequest
    {
        public string AreaName { get; set; }
        public int PincodeId { get; set; }
        public int LocalityId { get; set; }
    }
}
