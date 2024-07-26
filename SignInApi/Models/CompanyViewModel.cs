using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace SignInApi.Models
{
    public class CompanyViewModel
    {
        // Company Details Model
        public string CompanyName { get; set; }
        public string BusinessCategory { get; set; }
        public string NatureOfBusiness { get; set; }
        public DateTime YearOfEstablishment { get; set; }
        public int NumberOfEmployees { get; set; }
        public string Turnover { get; set; }
        public string GSTNumber { get; set; }
        public string Description { get; set; }
    }
}
