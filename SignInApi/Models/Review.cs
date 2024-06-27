namespace SignInApi.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public int Ratings { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public string RatingReplyMessage { get; set; }
    }
}
