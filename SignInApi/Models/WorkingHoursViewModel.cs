namespace SignInApi.Models
{
    public class WorkingHoursViewModel
    {


        public string MondayFrom { get; set; }
        public string MondayTo { get; set; }
        public string TuesdayFrom { get; set; }
        public string TuesdayTo { get; set; }
        public string WednesdayFrom { get; set; }
        public string WednesdayTo { get; set; }
        public string ThursdayFrom { get; set; }
        public string ThursdayTo { get; set; }
        public string FridayFrom { get; set; }
        public string FridayTo { get; set; }
        public string SaturdayFrom { get; set; }
        public string SaturdayTo { get; set; }
        public string SundayFrom { get; set; }
        public string SundayTo { get; set; }
        public bool SaturdayHoliday { get; set; }
        public bool SundayHoliday { get; set; }
    }
}
