namespace SignInApi.Models
{
    public class Suggestion
    {
        public int SuggestionID { get; set; }
        public string OwnerGuid { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string SuggestionText { get; set; }
        public string Title { get; set; }        
    }
}
