using System.Data.SqlClient;

namespace SignInApi.Models
{
    public class UserProfileVM
    {
        private string imgUrl;
        public string ImgUrl
        {
            get
            {
                return string.IsNullOrEmpty(imgUrl) ? "resources/img/Asset 33.png" : (imgUrl + "?DummyId=" + DateTime.Now.Ticks);
            }
            set { imgUrl = value; }
        }

        public string imgText
        {
            get
            {
                return isVendor ? "VENDOR" : "USER";
            }
        }

        public bool isVendor { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public Stream file { get; set; }

    }
}
