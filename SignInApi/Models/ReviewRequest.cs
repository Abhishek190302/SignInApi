namespace SignInApi.Models
{
    public class ReviewRequest
    {
        public string Operation { get; set; } // "GetReviews", "CreateReviewReply", "UpdateReviewReply"
        public RatingReply RatingReply { get; set; }
    }
}
