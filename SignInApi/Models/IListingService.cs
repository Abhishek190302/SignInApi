﻿namespace SignInApi.Models
{
    public interface IListingService
    {

        Task<List<ListingResult>> GetListings(int pageNumber, int pageSize,int? subCategoryid);

        Task<List<ListingResult>> GetListingsid(int subCategoryid, string cityName, string keywords);
        
        Task<List<ListingResult>> GetListingsKeywordlocation(int pageNumber, int pageSize, string keywords);
    }
}