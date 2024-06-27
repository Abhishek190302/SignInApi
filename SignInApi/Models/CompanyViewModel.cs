using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace SignInApi.Models
{
    public class CompanyViewModel
    {
        public string CompanyName { get; set; }
        public string BusinessCategory { get; set; }
        public string NatureOfBusiness { get; set; }
        public DateTime YearOfEstablishment { get; set; }
        public int NumberOfEmployees { get; set; }
        public string Turnover { get; set; }
        public string GSTNumber { get; set; }
        public string Description { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(CompanyName))
                return false;
            if (string.IsNullOrWhiteSpace(BusinessCategory))
                return false;
            if (string.IsNullOrWhiteSpace(NatureOfBusiness))
                return false;
            if (YearOfEstablishment == default(DateTime))
                return false;
            if (NumberOfEmployees <= 0)
                return false;
            if (string.IsNullOrWhiteSpace(Turnover))
                return false;
            if (string.IsNullOrWhiteSpace(GSTNumber))
                return false;
            if (string.IsNullOrWhiteSpace(Description))
                return false;

            return true;
        }
        
    }
}
