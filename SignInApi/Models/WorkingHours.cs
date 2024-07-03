namespace SignInApi.Models
{
    public class WorkingHours
    {
        public int ListingID { get; set; }
        public string OwnerGuid { get; set; }
        public string IPAddress { get; set; }
        public DateTime MondayFrom { get; set; }
        public DateTime MondayTo { get; set; }
        public DateTime TuesdayFrom { get; set; }
        public DateTime TuesdayTo { get; set; }
        public DateTime WednesdayFrom { get; set; }
        public DateTime WednesdayTo { get; set; }
        public DateTime ThursdayFrom { get; set; }
        public DateTime ThursdayTo { get; set; }
        public DateTime FridayFrom { get; set; }
        public DateTime FridayTo { get; set; }
        public DateTime SaturdayFrom { get; set; }
        public DateTime SaturdayTo { get; set; }
        public DateTime SundayFrom { get; set; }
        public DateTime SundayTo { get; set; }
        public bool SaturdayHoliday { get; set; }
        public bool SundayHoliday { get; set; }
    }
}
