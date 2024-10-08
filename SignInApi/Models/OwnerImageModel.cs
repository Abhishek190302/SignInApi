﻿namespace SignInApi.Models
{
    public class OwnerImageModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public List<IFormFile> File { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public string MrndMs { get; set; }
    }
}
