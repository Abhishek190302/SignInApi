namespace SignInApi.Models
{
    public interface ICategoryServices
    {
        Task<CategoryResponse> GetCategoriesForWebDevelopment();
        Task<CategoryResponse> GetCategoriesForNetting();
        Task<CategoryResponse> GetCategoriesForPrinting();
        Task<CategoryResponse> GetCategoriesForPackcaging();
        Task<CategoryResponse> GetCategoriesForArtModuling();
        Task<CategoryResponse> GetCategoriesForComputeriseCutting();
        Task<CategoryResponse> GetCategoriesForMetalBending();
        Task<CategoryResponse> GetCategoriesForPump();
        Task<CategoryResponse> GetCategoriesForSolarPanel();
        Task<CategoryResponse> GetCategoriesForTankCleaning();
        Task<CategoryResponse> GetCategoriesForPackersMovers();
        Task<CategoryResponse> GetCategoriesForGardening();
        Task<CategoryResponse> GetCategoriesForLegalAdvisory();
        Task<CategoryResponse> GetCategoriesForSecurity();
        Task<CategoryResponse> GetCategoriesForAutomation();
        Task<CategoryResponse> GetCategoriesForRealEastate();
        Task<CategoryResponse> GetCategoriesForPestControl();
        Task<CategoryResponse> GetCategoriesForHouseKeping();
        Task<CategoryResponse> GetCategoriesForTransport();
        Task<CategoryResponse> GetCategoriesForAdvertisingAgency();
        Task<CategoryResponse> GetCategoriesForWaterSuppliers();
        Task<CategoryResponse> GetCategoriesForPolishCoating();
        Task<CategoryResponse> GetCategoriesForToursTravels();
        Task<CategoryResponse> GetCategoriesForCourier();
        Task<CategoryResponse> GetCategoriesForLeafing();
        Task<CategoryResponse> GetCategoriesForConsultants();
        Task<CategoryResponse> GetCategoriesForSurveyors();
        Task<CategoryResponse> GetCategoriesForEngineers();
    }
}