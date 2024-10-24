namespace SignInApi.Models
{
    public class ReviewRequestVM
    {
        public int companyID { get; set; }
        public string Operation { get; set; } // "GetReviews", "CreateReviewReply", "UpdateReviewReply"
        public RatingReply RatingReply { get; set; }
    }
}
