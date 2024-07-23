using System.Data.SqlClient;
using System;

namespace SignInApi.Models
{
    public class CategoryService: ICategoryService
    {
        private readonly string _connectionString;

        public CategoryService(IConfiguration configuration)
        {           
            _connectionString = configuration.GetConnectionString("MimCategories");
        }


        public async Task GetCategoriesForIndexPageAsync(IndexVM indexVM)
        {
            indexVM.PremiumCategories = new List<CategoryRequest>();
            indexVM.Repairs = new List<CategoryRequest>();
            indexVM.Services = new List<CategoryRequest>();
            indexVM.Contractors = new List<CategoryRequest>();
            indexVM.Dealers = new List<CategoryRequest>();
            indexVM.Manufacturers = new List<CategoryRequest>();
            indexVM.Labours = new List<CategoryRequest>();
            indexVM.RentalServices = new List<CategoryRequest>();
            indexVM.LaborContractors = new List<CategoryRequest>();
            indexVM.Wholesalers = new List<CategoryRequest>();
            indexVM.Distributors = new List<CategoryRequest>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();


                #region PremiumCategories

                // Fetch PremiumCategories and there Subcategories
                await FetchCategoriesAsync(conn, "select * from [cat].[SecondCategory] where FirstCategoryID='39'", indexVM.PremiumCategories);

                //// Collect SecondCategory IDs
                //var secondCategoryIds = string.Join(",", indexVM.PremiumCategories.Select(c => $"'{c.SecondCategoryID}'"));

                //if (!string.IsNullOrEmpty(secondCategoryIds))
                //{
                //    // Fetch ThirdCategories with multiple SecondCategory IDs
                //    var thirdCategories = new List<CategoryRequest>();
                //    await FetchCategoriesAsync(conn, $"select * from [cat].[ThirdCategory] where SecondCategoryID in ({secondCategoryIds})", thirdCategories);

                //    // Map third categories to their respective second categories
                //    foreach (var secondCategory in indexVM.PremiumCategories)
                //    {
                //        secondCategory.ThirdCategories = thirdCategories.Where(tc => tc.SecondCategoryID == secondCategory.SecondCategoryID).ToList();

                //        // Collect ThirdCategory IDs for FourthCategory fetch
                //        var thirdCategoryIds = string.Join(",", secondCategory.ThirdCategories.Select(tc => $"'{tc.ThirdCategoryID}'"));

                //        if (!string.IsNullOrEmpty(thirdCategoryIds))
                //        {
                //            // Fetch FourthCategories with multiple ThirdCategory IDs
                //            var fourthCategories = new List<CategoryRequest>();
                //            await FetchCategoriesAsync(conn, $"select * from [cat].[FourthCategory] where ThirdCategoryID in ({thirdCategoryIds})", fourthCategories);

                //            // Map fourth categories to their respective third categories
                //            foreach (var thirdCategory in secondCategory.ThirdCategories)
                //            {
                //                thirdCategory.FourthCategories = fourthCategories.Where(fc => fc.ThirdCategoryID == thirdCategory.ThirdCategoryID).ToList();

                //                // Collect FourthCategory IDs for FifthCategory fetch
                //                var fourthCategoryIds = string.Join(",", thirdCategory.FourthCategories.Select(fc => $"'{fc.FourthCategoryID}'"));

                //                if (!string.IsNullOrEmpty(fourthCategoryIds))
                //                {
                //                    // Fetch FifthCategories with multiple FourthCategory IDs
                //                    var fifthCategories = new List<CategoryRequest>();
                //                    await FetchCategoriesAsync(conn, $"select * from [cat].[FifthCategory] where FourthCategoryID in ({fourthCategoryIds})", fifthCategories);

                //                    // Map fifth categories to their respective fourth categories
                //                    foreach (var fourthCategory in thirdCategory.FourthCategories)
                //                    {
                //                        fourthCategory.FifthCategories = fifthCategories.Where(fc => fc.FourthCategoryID == fourthCategory.FourthCategoryID).ToList();

                //                        // Collect FifthCategory IDs for SixthCategory fetch
                //                        var fifthCategoryIds = string.Join(",", fourthCategory.FifthCategories.Select(fc => $"'{fc.FifthCategoryID}'"));

                //                        if (!string.IsNullOrEmpty(fifthCategoryIds))
                //                        {
                //                            // Fetch SixthCategories with multiple FifthCategory IDs
                //                            var sixthCategories = new List<CategoryRequest>();
                //                            await FetchCategoriesAsync(conn, $"select * from [cat].[SixthCategory] where FifthCategoryID in ({fifthCategoryIds})", sixthCategories);

                //                            // Map sixth categories to their respective fifth categories
                //                            foreach (var fifthCategory in fourthCategory.FifthCategories)
                //                            {
                //                                fifthCategory.SixthCategories = sixthCategories.Where(sc => sc.FifthCategoryID == fifthCategory.FifthCategoryID).ToList();
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion


                #region RepairsCategories

                // Fetch Repairs
                await FetchCategoriesAsync(conn, "select * from [cat].[SecondCategory] where FirstCategoryID='40'", indexVM.Repairs);

                // Collect SecondCategory IDs
                //var secondCategoryIdrepairs = string.Join(",", indexVM.Repairs.Select(c => $"'{c.SecondCategoryID}'"));

                //if (!string.IsNullOrEmpty(secondCategoryIdrepairs))
                //{
                //    // Fetch ThirdCategories with multiple SecondCategory IDs
                //    var thirdCategories = new List<CategoryRequest>();
                //    await FetchCategoriesAsync(conn, $"select * from [cat].[ThirdCategory] where SecondCategoryID in ({secondCategoryIdrepairs})", thirdCategories);

                //    // Map third categories to their respective second categories
                //    foreach (var secondCategory in indexVM.Repairs)
                //    {
                //        secondCategory.ThirdCategories = thirdCategories.Where(tc => tc.SecondCategoryID == secondCategory.SecondCategoryID).ToList();

                //        // Collect ThirdCategory IDs for FourthCategory fetch
                //        var thirdCategoryIds = string.Join(",", secondCategory.ThirdCategories.Select(tc => $"'{tc.ThirdCategoryID}'"));

                //        if (!string.IsNullOrEmpty(thirdCategoryIds))
                //        {
                //            // Fetch FourthCategories with multiple ThirdCategory IDs
                //            var fourthCategories = new List<CategoryRequest>();
                //            await FetchCategoriesAsync(conn, $"select * from [cat].[FourthCategory] where ThirdCategoryID in ({thirdCategoryIds})", fourthCategories);

                //            // Map fourth categories to their respective third categories
                //            foreach (var thirdCategory in secondCategory.ThirdCategories)
                //            {
                //                thirdCategory.FourthCategories = fourthCategories.Where(fc => fc.ThirdCategoryID == thirdCategory.ThirdCategoryID).ToList();

                //                // Collect FourthCategory IDs for FifthCategory fetch
                //                var fourthCategoryIds = string.Join(",", thirdCategory.FourthCategories.Select(fc => $"'{fc.FourthCategoryID}'"));

                //                if (!string.IsNullOrEmpty(fourthCategoryIds))
                //                {
                //                    // Fetch FifthCategories with multiple FourthCategory IDs
                //                    var fifthCategories = new List<CategoryRequest>();
                //                    await FetchCategoriesAsync(conn, $"select * from [cat].[FifthCategory] where FourthCategoryID in ({fourthCategoryIds})", fifthCategories);

                //                    // Map fifth categories to their respective fourth categories
                //                    foreach (var fourthCategory in thirdCategory.FourthCategories)
                //                    {
                //                        fourthCategory.FifthCategories = fifthCategories.Where(fc => fc.FourthCategoryID == fourthCategory.FourthCategoryID).ToList();

                //                        // Collect FifthCategory IDs for SixthCategory fetch
                //                        var fifthCategoryIds = string.Join(",", fourthCategory.FifthCategories.Select(fc => $"'{fc.FifthCategoryID}'"));

                //                        if (!string.IsNullOrEmpty(fifthCategoryIds))
                //                        {
                //                            // Fetch SixthCategories with multiple FifthCategory IDs
                //                            var sixthCategories = new List<CategoryRequest>();
                //                            await FetchCategoriesAsync(conn, $"select * from [cat].[SixthCategory] where FifthCategoryID in ({fifthCategoryIds})", sixthCategories);

                //                            // Map sixth categories to their respective fifth categories
                //                            foreach (var fifthCategory in fourthCategory.FifthCategories)
                //                            {
                //                                fifthCategory.SixthCategories = sixthCategories.Where(sc => sc.FifthCategoryID == fifthCategory.FifthCategoryID).ToList();
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion


                #region ServicesCategories

                // Fetch Services
                await FetchCategoriesAsync(conn, "select * from [cat].[SecondCategory] where FirstCategoryID='41'", indexVM.Services);
                // Collect SecondCategory IDs
                var secondCategoryIdservices = string.Join(",", indexVM.Services.Select(c => $"'{c.SecondCategoryID}'"));

                if (!string.IsNullOrEmpty(secondCategoryIdservices))
                {
                    // Fetch ThirdCategories with multiple SecondCategory IDs
                    var thirdCategories = new List<CategoryRequest>();
                    await FetchCategoriesAsync(conn, $"select * from [cat].[ThirdCategory] where SecondCategoryID in ({secondCategoryIdservices})", thirdCategories);

                    // Map third categories to their respective second categories
                    foreach (var secondCategory in indexVM.Services)
                    {
                        secondCategory.ThirdCategories = thirdCategories.Where(tc => tc.SecondCategoryID == secondCategory.SecondCategoryID).ToList();

                        // Collect ThirdCategory IDs for FourthCategory fetch
                        var thirdCategoryIds = string.Join(",", secondCategory.ThirdCategories.Select(tc => $"'{tc.ThirdCategoryID}'"));

                        if (!string.IsNullOrEmpty(thirdCategoryIds))
                        {
                            // Fetch FourthCategories with multiple ThirdCategory IDs
                            var fourthCategories = new List<CategoryRequest>();
                            await FetchCategoriesAsync(conn, $"select * from [cat].[FourthCategory] where ThirdCategoryID in ({thirdCategoryIds})", fourthCategories);

                            // Map fourth categories to their respective third categories
                            foreach (var thirdCategory in secondCategory.ThirdCategories)
                            {
                                thirdCategory.FourthCategories = fourthCategories.Where(fc => fc.ThirdCategoryID == thirdCategory.ThirdCategoryID).ToList();

                                // Collect FourthCategory IDs for FifthCategory fetch
                                var fourthCategoryIds = string.Join(",", thirdCategory.FourthCategories.Select(fc => $"'{fc.FourthCategoryID}'"));

                                if (!string.IsNullOrEmpty(fourthCategoryIds))
                                {
                                    // Fetch FifthCategories with multiple FourthCategory IDs
                                    var fifthCategories = new List<CategoryRequest>();
                                    await FetchCategoriesAsync(conn, $"select * from [cat].[FifthCategory] where FourthCategoryID in ({fourthCategoryIds})", fifthCategories);

                                    // Map fifth categories to their respective fourth categories
                                    foreach (var fourthCategory in thirdCategory.FourthCategories)
                                    {
                                        fourthCategory.FifthCategories = fifthCategories.Where(fc => fc.FourthCategoryID == fourthCategory.FourthCategoryID).ToList();

                                        // Collect FifthCategory IDs for SixthCategory fetch
                                        var fifthCategoryIds = string.Join(",", fourthCategory.FifthCategories.Select(fc => $"'{fc.FifthCategoryID}'"));

                                        if (!string.IsNullOrEmpty(fifthCategoryIds))
                                        {
                                            // Fetch SixthCategories with multiple FifthCategory IDs
                                            var sixthCategories = new List<CategoryRequest>();
                                            await FetchCategoriesAsync(conn, $"select * from [cat].[SixthCategory] where FifthCategoryID in ({fifthCategoryIds})", sixthCategories);

                                            // Map sixth categories to their respective fifth categories
                                            foreach (var fifthCategory in fourthCategory.FifthCategories)
                                            {
                                                fifthCategory.SixthCategories = sixthCategories.Where(sc => sc.FifthCategoryID == fifthCategory.FifthCategoryID).ToList();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion


                #region ContractorsCategories

                // Fetch Contractors
                await FetchCategoriesAsync(conn, "select * from [cat].[SecondCategory] where FirstCategoryID='43'", indexVM.Contractors);

                // Collect SecondCategory IDs
                var secondCategoryIdcontractor = string.Join(",", indexVM.Contractors.Select(c => $"'{c.SecondCategoryID}'"));

                if (!string.IsNullOrEmpty(secondCategoryIdcontractor))
                {
                    // Fetch ThirdCategories with multiple SecondCategory IDs
                    var thirdCategories = new List<CategoryRequest>();
                    await FetchCategoriesAsync(conn, $"select * from [cat].[ThirdCategory] where SecondCategoryID in ({secondCategoryIdcontractor})", thirdCategories);

                    // Map third categories to their respective second categories
                    foreach (var secondCategory in indexVM.Contractors)
                    {
                        secondCategory.ThirdCategories = thirdCategories.Where(tc => tc.SecondCategoryID == secondCategory.SecondCategoryID).ToList();

                        // Collect ThirdCategory IDs for FourthCategory fetch
                        var thirdCategoryIds = string.Join(",", secondCategory.ThirdCategories.Select(tc => $"'{tc.ThirdCategoryID}'"));

                        if (!string.IsNullOrEmpty(thirdCategoryIds))
                        {
                            // Fetch FourthCategories with multiple ThirdCategory IDs
                            var fourthCategories = new List<CategoryRequest>();
                            await FetchCategoriesAsync(conn, $"select * from [cat].[FourthCategory] where ThirdCategoryID in ({thirdCategoryIds})", fourthCategories);

                            // Map fourth categories to their respective third categories
                            foreach (var thirdCategory in secondCategory.ThirdCategories)
                            {
                                thirdCategory.FourthCategories = fourthCategories.Where(fc => fc.ThirdCategoryID == thirdCategory.ThirdCategoryID).ToList();

                                // Collect FourthCategory IDs for FifthCategory fetch
                                var fourthCategoryIds = string.Join(",", thirdCategory.FourthCategories.Select(fc => $"'{fc.FourthCategoryID}'"));

                                if (!string.IsNullOrEmpty(fourthCategoryIds))
                                {
                                    // Fetch FifthCategories with multiple FourthCategory IDs
                                    var fifthCategories = new List<CategoryRequest>();
                                    await FetchCategoriesAsync(conn, $"select * from [cat].[FifthCategory] where FourthCategoryID in ({fourthCategoryIds})", fifthCategories);

                                    // Map fifth categories to their respective fourth categories
                                    foreach (var fourthCategory in thirdCategory.FourthCategories)
                                    {
                                        fourthCategory.FifthCategories = fifthCategories.Where(fc => fc.FourthCategoryID == fourthCategory.FourthCategoryID).ToList();

                                        // Collect FifthCategory IDs for SixthCategory fetch
                                        var fifthCategoryIds = string.Join(",", fourthCategory.FifthCategories.Select(fc => $"'{fc.FifthCategoryID}'"));

                                        if (!string.IsNullOrEmpty(fifthCategoryIds))
                                        {
                                            // Fetch SixthCategories with multiple FifthCategory IDs
                                            var sixthCategories = new List<CategoryRequest>();
                                            await FetchCategoriesAsync(conn, $"select * from [cat].[SixthCategory] where FifthCategoryID in ({fifthCategoryIds})", sixthCategories);

                                            // Map sixth categories to their respective fifth categories
                                            foreach (var fifthCategory in fourthCategory.FifthCategories)
                                            {
                                                fifthCategory.SixthCategories = sixthCategories.Where(sc => sc.FifthCategoryID == fifthCategory.FifthCategoryID).ToList();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion


                #region DealersCategories

                // Fetch Dealers
                await FetchCategoriesAsync(conn, "select * from [cat].[SecondCategory] where FirstCategoryID='44'", indexVM.Dealers);

                // Collect SecondCategory IDs
                var secondCategoryIddealers = string.Join(",", indexVM.Dealers.Select(c => $"'{c.SecondCategoryID}'"));

                if (!string.IsNullOrEmpty(secondCategoryIddealers))
                {
                    // Fetch ThirdCategories with multiple SecondCategory IDs
                    var thirdCategories = new List<CategoryRequest>();
                    await FetchCategoriesAsync(conn, $"select * from [cat].[ThirdCategory] where SecondCategoryID in ({secondCategoryIddealers})", thirdCategories);

                    // Map third categories to their respective second categories
                    foreach (var secondCategory in indexVM.Dealers)
                    {
                        secondCategory.ThirdCategories = thirdCategories.Where(tc => tc.SecondCategoryID == secondCategory.SecondCategoryID).ToList();

                        // Collect ThirdCategory IDs for FourthCategory fetch
                        var thirdCategoryIds = string.Join(",", secondCategory.ThirdCategories.Select(tc => $"'{tc.ThirdCategoryID}'"));

                        if (!string.IsNullOrEmpty(thirdCategoryIds))
                        {
                            // Fetch FourthCategories with multiple ThirdCategory IDs
                            var fourthCategories = new List<CategoryRequest>();
                            await FetchCategoriesAsync(conn, $"select * from [cat].[FourthCategory] where ThirdCategoryID in ({thirdCategoryIds})", fourthCategories);

                            // Map fourth categories to their respective third categories
                            foreach (var thirdCategory in secondCategory.ThirdCategories)
                            {
                                thirdCategory.FourthCategories = fourthCategories.Where(fc => fc.ThirdCategoryID == thirdCategory.ThirdCategoryID).ToList();

                                // Collect FourthCategory IDs for FifthCategory fetch
                                var fourthCategoryIds = string.Join(",", thirdCategory.FourthCategories.Select(fc => $"'{fc.FourthCategoryID}'"));

                                if (!string.IsNullOrEmpty(fourthCategoryIds))
                                {
                                    // Fetch FifthCategories with multiple FourthCategory IDs
                                    var fifthCategories = new List<CategoryRequest>();
                                    await FetchCategoriesAsync(conn, $"select * from [cat].[FifthCategory] where FourthCategoryID in ({fourthCategoryIds})", fifthCategories);

                                    // Map fifth categories to their respective fourth categories
                                    foreach (var fourthCategory in thirdCategory.FourthCategories)
                                    {
                                        fourthCategory.FifthCategories = fifthCategories.Where(fc => fc.FourthCategoryID == fourthCategory.FourthCategoryID).ToList();

                                        // Collect FifthCategory IDs for SixthCategory fetch
                                        var fifthCategoryIds = string.Join(",", fourthCategory.FifthCategories.Select(fc => $"'{fc.FifthCategoryID}'"));

                                        if (!string.IsNullOrEmpty(fifthCategoryIds))
                                        {
                                            // Fetch SixthCategories with multiple FifthCategory IDs
                                            var sixthCategories = new List<CategoryRequest>();
                                            await FetchCategoriesAsync(conn, $"select * from [cat].[SixthCategory] where FifthCategoryID in ({fifthCategoryIds})", sixthCategories);

                                            // Map sixth categories to their respective fifth categories
                                            foreach (var fifthCategory in fourthCategory.FifthCategories)
                                            {
                                                fifthCategory.SixthCategories = sixthCategories.Where(sc => sc.FifthCategoryID == fifthCategory.FifthCategoryID).ToList();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                #endregion


                #region ManufacturersCategories

                // Fetch Manufacturers
                await FetchCategoriesAsync(conn, "select * from [cat].[SecondCategory] where FirstCategoryID='45'", indexVM.Manufacturers);

                // Collect SecondCategory IDs
                //var secondCategoryIdmanufacturers = string.Join(",", indexVM.Manufacturers.Select(c => $"'{c.SecondCategoryID}'"));

                //if (!string.IsNullOrEmpty(secondCategoryIdmanufacturers))
                //{
                //    // Fetch ThirdCategories with multiple SecondCategory IDs
                //    var thirdCategories = new List<CategoryRequest>();
                //    await FetchCategoriesAsync(conn, $"select * from [cat].[ThirdCategory] where SecondCategoryID in ({secondCategoryIdmanufacturers})", thirdCategories);

                //    // Map third categories to their respective second categories
                //    foreach (var secondCategory in indexVM.Manufacturers)
                //    {
                //        secondCategory.ThirdCategories = thirdCategories.Where(tc => tc.SecondCategoryID == secondCategory.SecondCategoryID).ToList();

                //        // Collect ThirdCategory IDs for FourthCategory fetch
                //        var thirdCategoryIds = string.Join(",", secondCategory.ThirdCategories.Select(tc => $"'{tc.ThirdCategoryID}'"));

                //        if (!string.IsNullOrEmpty(thirdCategoryIds))
                //        {
                //            // Fetch FourthCategories with multiple ThirdCategory IDs
                //            var fourthCategories = new List<CategoryRequest>();
                //            await FetchCategoriesAsync(conn, $"select * from [cat].[FourthCategory] where ThirdCategoryID in ({thirdCategoryIds})", fourthCategories);

                //            // Map fourth categories to their respective third categories
                //            foreach (var thirdCategory in secondCategory.ThirdCategories)
                //            {
                //                thirdCategory.FourthCategories = fourthCategories.Where(fc => fc.ThirdCategoryID == thirdCategory.ThirdCategoryID).ToList();

                //                // Collect FourthCategory IDs for FifthCategory fetch
                //                var fourthCategoryIds = string.Join(",", thirdCategory.FourthCategories.Select(fc => $"'{fc.FourthCategoryID}'"));

                //                if (!string.IsNullOrEmpty(fourthCategoryIds))
                //                {
                //                    // Fetch FifthCategories with multiple FourthCategory IDs
                //                    var fifthCategories = new List<CategoryRequest>();
                //                    await FetchCategoriesAsync(conn, $"select * from [cat].[FifthCategory] where FourthCategoryID in ({fourthCategoryIds})", fifthCategories);

                //                    // Map fifth categories to their respective fourth categories
                //                    foreach (var fourthCategory in thirdCategory.FourthCategories)
                //                    {
                //                        fourthCategory.FifthCategories = fifthCategories.Where(fc => fc.FourthCategoryID == fourthCategory.FourthCategoryID).ToList();

                //                        // Collect FifthCategory IDs for SixthCategory fetch
                //                        var fifthCategoryIds = string.Join(",", fourthCategory.FifthCategories.Select(fc => $"'{fc.FifthCategoryID}'"));

                //                        if (!string.IsNullOrEmpty(fifthCategoryIds))
                //                        {
                //                            // Fetch SixthCategories with multiple FifthCategory IDs
                //                            var sixthCategories = new List<CategoryRequest>();
                //                            await FetchCategoriesAsync(conn, $"select * from [cat].[SixthCategory] where FifthCategoryID in ({fifthCategoryIds})", sixthCategories);

                //                            // Map sixth categories to their respective fifth categories
                //                            foreach (var fifthCategory in fourthCategory.FifthCategories)
                //                            {
                //                                fifthCategory.SixthCategories = sixthCategories.Where(sc => sc.FifthCategoryID == fifthCategory.FifthCategoryID).ToList();
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion


                #region LaboursCategories

                // Fetch Labours
                await FetchCategoriesAsync(conn, "select * from [cat].[SecondCategory] where FirstCategoryID='46'", indexVM.Labours);

                // Collect SecondCategory IDs
                //var secondCategoryIdlabours = string.Join(",", indexVM.Labours.Select(c => $"'{c.SecondCategoryID}'"));

                //if (!string.IsNullOrEmpty(secondCategoryIdlabours))
                //{
                //    // Fetch ThirdCategories with multiple SecondCategory IDs
                //    var thirdCategories = new List<CategoryRequest>();
                //    await FetchCategoriesAsync(conn, $"select * from [cat].[ThirdCategory] where SecondCategoryID in ({secondCategoryIdlabours})", thirdCategories);

                //    // Map third categories to their respective second categories
                //    foreach (var secondCategory in indexVM.Labours)
                //    {
                //        secondCategory.ThirdCategories = thirdCategories.Where(tc => tc.SecondCategoryID == secondCategory.SecondCategoryID).ToList();

                //        // Collect ThirdCategory IDs for FourthCategory fetch
                //        var thirdCategoryIds = string.Join(",", secondCategory.ThirdCategories.Select(tc => $"'{tc.ThirdCategoryID}'"));

                //        if (!string.IsNullOrEmpty(thirdCategoryIds))
                //        {
                //            // Fetch FourthCategories with multiple ThirdCategory IDs
                //            var fourthCategories = new List<CategoryRequest>();
                //            await FetchCategoriesAsync(conn, $"select * from [cat].[FourthCategory] where ThirdCategoryID in ({thirdCategoryIds})", fourthCategories);

                //            // Map fourth categories to their respective third categories
                //            foreach (var thirdCategory in secondCategory.ThirdCategories)
                //            {
                //                thirdCategory.FourthCategories = fourthCategories.Where(fc => fc.ThirdCategoryID == thirdCategory.ThirdCategoryID).ToList();

                //                // Collect FourthCategory IDs for FifthCategory fetch
                //                var fourthCategoryIds = string.Join(",", thirdCategory.FourthCategories.Select(fc => $"'{fc.FourthCategoryID}'"));

                //                if (!string.IsNullOrEmpty(fourthCategoryIds))
                //                {
                //                    // Fetch FifthCategories with multiple FourthCategory IDs
                //                    var fifthCategories = new List<CategoryRequest>();
                //                    await FetchCategoriesAsync(conn, $"select * from [cat].[FifthCategory] where FourthCategoryID in ({fourthCategoryIds})", fifthCategories);

                //                    // Map fifth categories to their respective fourth categories
                //                    foreach (var fourthCategory in thirdCategory.FourthCategories)
                //                    {
                //                        fourthCategory.FifthCategories = fifthCategories.Where(fc => fc.FourthCategoryID == fourthCategory.FourthCategoryID).ToList();

                //                        // Collect FifthCategory IDs for SixthCategory fetch
                //                        var fifthCategoryIds = string.Join(",", fourthCategory.FifthCategories.Select(fc => $"'{fc.FifthCategoryID}'"));

                //                        if (!string.IsNullOrEmpty(fifthCategoryIds))
                //                        {
                //                            // Fetch SixthCategories with multiple FifthCategory IDs
                //                            var sixthCategories = new List<CategoryRequest>();
                //                            await FetchCategoriesAsync(conn, $"select * from [cat].[SixthCategory] where FifthCategoryID in ({fifthCategoryIds})", sixthCategories);

                //                            // Map sixth categories to their respective fifth categories
                //                            foreach (var fifthCategory in fourthCategory.FifthCategories)
                //                            {
                //                                fifthCategory.SixthCategories = sixthCategories.Where(sc => sc.FifthCategoryID == fifthCategory.FifthCategoryID).ToList();
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion


                #region RentalServicesCategories

                // Fetch RentalServices
                await FetchCategoriesAsync(conn, "select * from [cat].[SecondCategory] where FirstCategoryID='47'", indexVM.RentalServices);

                // Collect SecondCategory IDs
                //var secondCategoryIdRentalServices = string.Join(",", indexVM.RentalServices.Select(c => $"'{c.SecondCategoryID}'"));

                //if (!string.IsNullOrEmpty(secondCategoryIdRentalServices))
                //{
                //    // Fetch ThirdCategories with multiple SecondCategory IDs
                //    var thirdCategories = new List<CategoryRequest>();
                //    await FetchCategoriesAsync(conn, $"select * from [cat].[ThirdCategory] where SecondCategoryID in ({secondCategoryIdRentalServices})", thirdCategories);

                //    // Map third categories to their respective second categories
                //    foreach (var secondCategory in indexVM.RentalServices)
                //    {
                //        secondCategory.ThirdCategories = thirdCategories.Where(tc => tc.SecondCategoryID == secondCategory.SecondCategoryID).ToList();

                //        // Collect ThirdCategory IDs for FourthCategory fetch
                //        var thirdCategoryIds = string.Join(",", secondCategory.ThirdCategories.Select(tc => $"'{tc.ThirdCategoryID}'"));

                //        if (!string.IsNullOrEmpty(thirdCategoryIds))
                //        {
                //            // Fetch FourthCategories with multiple ThirdCategory IDs
                //            var fourthCategories = new List<CategoryRequest>();
                //            await FetchCategoriesAsync(conn, $"select * from [cat].[FourthCategory] where ThirdCategoryID in ({thirdCategoryIds})", fourthCategories);

                //            // Map fourth categories to their respective third categories
                //            foreach (var thirdCategory in secondCategory.ThirdCategories)
                //            {
                //                thirdCategory.FourthCategories = fourthCategories.Where(fc => fc.ThirdCategoryID == thirdCategory.ThirdCategoryID).ToList();

                //                // Collect FourthCategory IDs for FifthCategory fetch
                //                var fourthCategoryIds = string.Join(",", thirdCategory.FourthCategories.Select(fc => $"'{fc.FourthCategoryID}'"));

                //                if (!string.IsNullOrEmpty(fourthCategoryIds))
                //                {
                //                    // Fetch FifthCategories with multiple FourthCategory IDs
                //                    var fifthCategories = new List<CategoryRequest>();
                //                    await FetchCategoriesAsync(conn, $"select * from [cat].[FifthCategory] where FourthCategoryID in ({fourthCategoryIds})", fifthCategories);

                //                    // Map fifth categories to their respective fourth categories
                //                    foreach (var fourthCategory in thirdCategory.FourthCategories)
                //                    {
                //                        fourthCategory.FifthCategories = fifthCategories.Where(fc => fc.FourthCategoryID == fourthCategory.FourthCategoryID).ToList();

                //                        // Collect FifthCategory IDs for SixthCategory fetch
                //                        var fifthCategoryIds = string.Join(",", fourthCategory.FifthCategories.Select(fc => $"'{fc.FifthCategoryID}'"));

                //                        if (!string.IsNullOrEmpty(fifthCategoryIds))
                //                        {
                //                            // Fetch SixthCategories with multiple FifthCategory IDs
                //                            var sixthCategories = new List<CategoryRequest>();
                //                            await FetchCategoriesAsync(conn, $"select * from [cat].[SixthCategory] where FifthCategoryID in ({fifthCategoryIds})", sixthCategories);

                //                            // Map sixth categories to their respective fifth categories
                //                            foreach (var fifthCategory in fourthCategory.FifthCategories)
                //                            {
                //                                fifthCategory.SixthCategories = sixthCategories.Where(sc => sc.FifthCategoryID == fifthCategory.FifthCategoryID).ToList();
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion


                #region LaborContractorsCategories

                // Fetch LaborContractors
                await FetchCategoriesAsync(conn, "select * from [cat].[SecondCategory] where FirstCategoryID='48'", indexVM.LaborContractors);

                // Collect SecondCategory IDs
                //var secondCategoryIdlabourcontractor = string.Join(",", indexVM.LaborContractors.Select(c => $"'{c.SecondCategoryID}'"));

                //if (!string.IsNullOrEmpty(secondCategoryIdlabourcontractor))
                //{
                //    // Fetch ThirdCategories with multiple SecondCategory IDs
                //    var thirdCategories = new List<CategoryRequest>();
                //    await FetchCategoriesAsync(conn, $"select * from [cat].[ThirdCategory] where SecondCategoryID in ({secondCategoryIdlabourcontractor})", thirdCategories);

                //    // Map third categories to their respective second categories
                //    foreach (var secondCategory in indexVM.LaborContractors)
                //    {
                //        secondCategory.ThirdCategories = thirdCategories.Where(tc => tc.SecondCategoryID == secondCategory.SecondCategoryID).ToList();

                //        // Collect ThirdCategory IDs for FourthCategory fetch
                //        var thirdCategoryIds = string.Join(",", secondCategory.ThirdCategories.Select(tc => $"'{tc.ThirdCategoryID}'"));

                //        if (!string.IsNullOrEmpty(thirdCategoryIds))
                //        {
                //            // Fetch FourthCategories with multiple ThirdCategory IDs
                //            var fourthCategories = new List<CategoryRequest>();
                //            await FetchCategoriesAsync(conn, $"select * from [cat].[FourthCategory] where ThirdCategoryID in ({thirdCategoryIds})", fourthCategories);

                //            // Map fourth categories to their respective third categories
                //            foreach (var thirdCategory in secondCategory.ThirdCategories)
                //            {
                //                thirdCategory.FourthCategories = fourthCategories.Where(fc => fc.ThirdCategoryID == thirdCategory.ThirdCategoryID).ToList();

                //                // Collect FourthCategory IDs for FifthCategory fetch
                //                var fourthCategoryIds = string.Join(",", thirdCategory.FourthCategories.Select(fc => $"'{fc.FourthCategoryID}'"));

                //                if (!string.IsNullOrEmpty(fourthCategoryIds))
                //                {
                //                    // Fetch FifthCategories with multiple FourthCategory IDs
                //                    var fifthCategories = new List<CategoryRequest>();
                //                    await FetchCategoriesAsync(conn, $"select * from [cat].[FifthCategory] where FourthCategoryID in ({fourthCategoryIds})", fifthCategories);

                //                    // Map fifth categories to their respective fourth categories
                //                    foreach (var fourthCategory in thirdCategory.FourthCategories)
                //                    {
                //                        fourthCategory.FifthCategories = fifthCategories.Where(fc => fc.FourthCategoryID == fourthCategory.FourthCategoryID).ToList();

                //                        // Collect FifthCategory IDs for SixthCategory fetch
                //                        var fifthCategoryIds = string.Join(",", fourthCategory.FifthCategories.Select(fc => $"'{fc.FifthCategoryID}'"));

                //                        if (!string.IsNullOrEmpty(fifthCategoryIds))
                //                        {
                //                            // Fetch SixthCategories with multiple FifthCategory IDs
                //                            var sixthCategories = new List<CategoryRequest>();
                //                            await FetchCategoriesAsync(conn, $"select * from [cat].[SixthCategory] where FifthCategoryID in ({fifthCategoryIds})", sixthCategories);

                //                            // Map sixth categories to their respective fifth categories
                //                            foreach (var fifthCategory in fourthCategory.FifthCategories)
                //                            {
                //                                fifthCategory.SixthCategories = sixthCategories.Where(sc => sc.FifthCategoryID == fifthCategory.FifthCategoryID).ToList();
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion


                #region WholesalersCategories

                // Fetch Wholesalers
                await FetchCategoriesAsync(conn, "select * from [cat].[SecondCategory] where FirstCategoryID='53'", indexVM.Wholesalers);

                // Collect SecondCategory IDs
                //var secondCategoryIdwholesalers = string.Join(",", indexVM.Wholesalers.Select(c => $"'{c.SecondCategoryID}'"));

                //if (!string.IsNullOrEmpty(secondCategoryIdwholesalers))
                //{
                //    // Fetch ThirdCategories with multiple SecondCategory IDs
                //    var thirdCategories = new List<CategoryRequest>();
                //    await FetchCategoriesAsync(conn, $"select * from [cat].[ThirdCategory] where SecondCategoryID in ({secondCategoryIdwholesalers})", thirdCategories);

                //    // Map third categories to their respective second categories
                //    foreach (var secondCategory in indexVM.Wholesalers)
                //    {
                //        secondCategory.ThirdCategories = thirdCategories.Where(tc => tc.SecondCategoryID == secondCategory.SecondCategoryID).ToList();

                //        // Collect ThirdCategory IDs for FourthCategory fetch
                //        var thirdCategoryIds = string.Join(",", secondCategory.ThirdCategories.Select(tc => $"'{tc.ThirdCategoryID}'"));

                //        if (!string.IsNullOrEmpty(thirdCategoryIds))
                //        {
                //            // Fetch FourthCategories with multiple ThirdCategory IDs
                //            var fourthCategories = new List<CategoryRequest>();
                //            await FetchCategoriesAsync(conn, $"select * from [cat].[FourthCategory] where ThirdCategoryID in ({thirdCategoryIds})", fourthCategories);

                //            // Map fourth categories to their respective third categories
                //            foreach (var thirdCategory in secondCategory.ThirdCategories)
                //            {
                //                thirdCategory.FourthCategories = fourthCategories.Where(fc => fc.ThirdCategoryID == thirdCategory.ThirdCategoryID).ToList();

                //                // Collect FourthCategory IDs for FifthCategory fetch
                //                var fourthCategoryIds = string.Join(",", thirdCategory.FourthCategories.Select(fc => $"'{fc.FourthCategoryID}'"));

                //                if (!string.IsNullOrEmpty(fourthCategoryIds))
                //                {
                //                    // Fetch FifthCategories with multiple FourthCategory IDs
                //                    var fifthCategories = new List<CategoryRequest>();
                //                    await FetchCategoriesAsync(conn, $"select * from [cat].[FifthCategory] where FourthCategoryID in ({fourthCategoryIds})", fifthCategories);

                //                    // Map fifth categories to their respective fourth categories
                //                    foreach (var fourthCategory in thirdCategory.FourthCategories)
                //                    {
                //                        fourthCategory.FifthCategories = fifthCategories.Where(fc => fc.FourthCategoryID == fourthCategory.FourthCategoryID).ToList();

                //                        // Collect FifthCategory IDs for SixthCategory fetch
                //                        var fifthCategoryIds = string.Join(",", fourthCategory.FifthCategories.Select(fc => $"'{fc.FifthCategoryID}'"));

                //                        if (!string.IsNullOrEmpty(fifthCategoryIds))
                //                        {
                //                            // Fetch SixthCategories with multiple FifthCategory IDs
                //                            var sixthCategories = new List<CategoryRequest>();
                //                            await FetchCategoriesAsync(conn, $"select * from [cat].[SixthCategory] where FifthCategoryID in ({fifthCategoryIds})", sixthCategories);

                //                            // Map sixth categories to their respective fifth categories
                //                            foreach (var fifthCategory in fourthCategory.FifthCategories)
                //                            {
                //                                fifthCategory.SixthCategories = sixthCategories.Where(sc => sc.FifthCategoryID == fifthCategory.FifthCategoryID).ToList();
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion


                #region DistributorsCategories

                // Fetch Distributors
                await FetchCategoriesAsync(conn, "select * from [cat].[SecondCategory] where FirstCategoryID='54'", indexVM.Distributors);
                
                // Collect SecondCategory IDs
                //var secondCategoryIddistributors = string.Join(",", indexVM.Distributors.Select(c => $"'{c.SecondCategoryID}'"));

                //if (!string.IsNullOrEmpty(secondCategoryIddistributors))
                //{
                //    // Fetch ThirdCategories with multiple SecondCategory IDs
                //    var thirdCategories = new List<CategoryRequest>();
                //    await FetchCategoriesAsync(conn, $"select * from [cat].[ThirdCategory] where SecondCategoryID in ({secondCategoryIddistributors})", thirdCategories);

                //    // Map third categories to their respective second categories
                //    foreach (var secondCategory in indexVM.Distributors)
                //    {
                //        secondCategory.ThirdCategories = thirdCategories.Where(tc => tc.SecondCategoryID == secondCategory.SecondCategoryID).ToList();

                //        // Collect ThirdCategory IDs for FourthCategory fetch
                //        var thirdCategoryIds = string.Join(",", secondCategory.ThirdCategories.Select(tc => $"'{tc.ThirdCategoryID}'"));

                //        if (!string.IsNullOrEmpty(thirdCategoryIds))
                //        {
                //            // Fetch FourthCategories with multiple ThirdCategory IDs
                //            var fourthCategories = new List<CategoryRequest>();
                //            await FetchCategoriesAsync(conn, $"select * from [cat].[FourthCategory] where ThirdCategoryID in ({thirdCategoryIds})", fourthCategories);

                //            // Map fourth categories to their respective third categories
                //            foreach (var thirdCategory in secondCategory.ThirdCategories)
                //            {
                //                thirdCategory.FourthCategories = fourthCategories.Where(fc => fc.ThirdCategoryID == thirdCategory.ThirdCategoryID).ToList();

                //                // Collect FourthCategory IDs for FifthCategory fetch
                //                var fourthCategoryIds = string.Join(",", thirdCategory.FourthCategories.Select(fc => $"'{fc.FourthCategoryID}'"));

                //                if (!string.IsNullOrEmpty(fourthCategoryIds))
                //                {
                //                    // Fetch FifthCategories with multiple FourthCategory IDs
                //                    var fifthCategories = new List<CategoryRequest>();
                //                    await FetchCategoriesAsync(conn, $"select * from [cat].[FifthCategory] where FourthCategoryID in ({fourthCategoryIds})", fifthCategories);

                //                    // Map fifth categories to their respective fourth categories
                //                    foreach (var fourthCategory in thirdCategory.FourthCategories)
                //                    {
                //                        fourthCategory.FifthCategories = fifthCategories.Where(fc => fc.FourthCategoryID == fourthCategory.FourthCategoryID).ToList();

                //                        // Collect FifthCategory IDs for SixthCategory fetch
                //                        var fifthCategoryIds = string.Join(",", fourthCategory.FifthCategories.Select(fc => $"'{fc.FifthCategoryID}'"));

                //                        if (!string.IsNullOrEmpty(fifthCategoryIds))
                //                        {
                //                            // Fetch SixthCategories with multiple FifthCategory IDs
                //                            var sixthCategories = new List<CategoryRequest>();
                //                            await FetchCategoriesAsync(conn, $"select * from [cat].[SixthCategory] where FifthCategoryID in ({fifthCategoryIds})", sixthCategories);

                //                            // Map sixth categories to their respective fifth categories
                //                            foreach (var fifthCategory in fourthCategory.FifthCategories)
                //                            {
                //                                fifthCategory.SixthCategories = sixthCategories.Where(sc => sc.FifthCategoryID == fifthCategory.FifthCategoryID).ToList();
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion
            }
        }


        private async Task FetchCategoriesAsync(SqlConnection conn, string query, List<CategoryRequest> categoryList)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        categoryList.Add(new CategoryRequest
                        {
                            FirstCategoryID = reader.GetColumnValueOrNull("FirstCategoryID"),
                            ImageURL = reader.GetColumnValueOrNull("URL"),
                            Name = reader.GetColumnValueOrNull("Name"),
                            SearchKeywordName = reader.GetColumnValueOrNull("SearchKeywordName"),
                            SortOrder = reader.GetColumnValueOrNull("SortOrder"),
                            SecondCategoryID = reader.GetColumnValueOrNull("SecondCategoryID"),
                            ThirdCategoryID = reader.GetColumnValueOrNull("ThirdCategoryID"),
                            FourthCategoryID = reader.GetColumnValueOrNull("FourthCategoryID"),
                            FifthCategoryID = reader.GetColumnValueOrNull("FifthCategoryID")
                        });
                    }
                }
            }
        }   
    }

    public static class SqlDataReaderExtensions
    {
        public static string GetColumnValueOrNull(this SqlDataReader reader, string columnName)
        {
            if (!reader.HasColumn(columnName) || reader[columnName] == DBNull.Value)
            {
                return null;
            }
            return reader[columnName].ToString();
        }

        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
