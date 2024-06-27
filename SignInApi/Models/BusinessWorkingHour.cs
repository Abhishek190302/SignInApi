using Microsoft.VisualBasic;

namespace SignInApi.Models
{
    public class BusinessWorkingHour
    {
        public static string Open = "Open";
        public static string Closed = "Closed Now";

        public bool IsBusinessOpen { get; set; }
        public string IsBusinessOpenText
        {
            get
            {
                return IsBusinessOpen ? Open : Closed;
            }
            set
            {

            }
            
        }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public string OpenDay { get; set; }
    }
}
