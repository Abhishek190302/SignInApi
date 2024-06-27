using System;

namespace SignInApi.Models
{
    public interface ICategoryService
    {
        Task GetCategoriesForIndexPageAsync(IndexVM indexVM);
    }
}