using System.Data.SqlClient;

namespace SignInApi.Models
{
    public class CategoryServices: ICategoryServices
    {
        private readonly string _connectionString;

        public CategoryServices(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimCategory");
        }

        public async Task<CategoryResponse> GetCategoriesForWebDevelopment()
        {
            var response = new CategoryResponse
            {
               
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>(),
                SixthCategories = new List<SixthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                
                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory]where SecondCategoryID='94'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('203','204','205')", conn))
                {  
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('185','186','187','188','189','190','191','192','193','194','2169','4231','183','184')", conn))
                {  
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Sixth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[SixthCategory] WHERE FifthCategoryID in ('3530','3531')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.SixthCategories.Add(new SixthCategory
                            {
                                FifthCategoryId = reader["FifthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForNetting()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory]where SecondCategoryID='79'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('262','263','264','1708','1709','1710','1711','1712','1713')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('2989','2990')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForPrinting()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory]where SecondCategoryID='81'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('269','270','271','272','273')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }               
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForPackcaging()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory]where SecondCategoryID='82'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('1425','1426','1427','1428')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForArtModuling()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] WHERE SecondCategoryID='83'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('2524','2525','2526','2527')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('4098','4099','4100','4101','4102','4103','4104','4105','4106')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForComputeriseCutting()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] WHERE SecondCategoryID='84'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('279','280','281','1731')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('266','267','268','269','270','271','272','273','274','275','276','277','278','279','280','281','282','283','284','285','286','287','288','289','290','291','292','293','294','295','296','297','298','3032','3033','3034','3035','3036','3037','3038','3039','3040')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForMetalBending()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] WHERE SecondCategoryID='85'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('283','284','285','286')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('308','309','310','311','312','313','314','315','316','317','318','319','320','321','322','323')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForPump()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
               
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory]where SecondCategoryID='86'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForSolarPanel()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory]where SecondCategoryID='87'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('288','289','290','291','292','293','294','295')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForTankCleaning()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='88'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('296','297','298')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForPackersMovers()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='89'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('226','227','228')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForGardening()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='90'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('302','303','304')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForLegalAdvisory()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] WHERE SecondCategoryID='92'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('195','196')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('159','160','161','162','163','164','165','166','167','168','169','170','171','172','173')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForSecurity()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='93'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('199','1415','1416')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForAutomation()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] WHERE SecondCategoryID='96'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('206','207','208','1700','1701','1702','1703','1704','1705','1706','1707','2575','2576')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('4217','4218','4219','4220','4221','4222','4223','4224','4225','4226','4209','4210','4211','4212','4213','4214','4215','4216')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForRealEastate()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='98'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('266','267','268')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForPestControl()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='99'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('229','230','231','232','233','234','235','236','237','238','239','240')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }

        public async Task<CategoryResponse> GetCategoriesForHouseKeping()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='100'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('2530','2531','2532','2533','2534','2535','2536','2537','2538','2539','2540','2541')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForTransport()
        {
            var response = new CategoryResponse
            {

                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>(),
                SixthCategories = new List<SixthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='101'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('250','2594')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('4232','4233','4237','4234','4235','4236')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Sixth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[SixthCategory] WHERE FifthCategoryID in ('3590','3591','3592','3594','3595','3596','3597','3598')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.SixthCategories.Add(new SixthCategory
                            {
                                FifthCategoryId = reader["FifthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }

        public async Task<CategoryResponse> GetCategoriesForAdvertisingAgency()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] WHERE SecondCategoryID='102'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('251','252','253')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('240','241','244','245','2175','242')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForWaterSuppliers()
        {
            var response = new CategoryResponse
            {

                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>(),
                SixthCategories = new List<SixthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='103'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('301','2595')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('4244','4245','4246','4247','4238','4239','4254','4255')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Sixth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[SixthCategory] WHERE FifthCategoryID in ('3599','3600','3601','3602','3603','3604','3605','3606','3607','3608','3609','3610')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.SixthCategories.Add(new SixthCategory
                            {
                                FifthCategoryId = reader["FifthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForPolishCoating()
        {
            var response = new CategoryResponse
            {

                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>(),
                SixthCategories = new List<SixthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='104'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('2341','2600','2602','2603','2604')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('3815','3816','4278','4279','4280','4274','4275','4276','4286','4281','4282','4283','4284','4285')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Sixth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[SixthCategory] WHERE FifthCategoryID in ('3611','3612','3613','3614','3615','3616','3617','3618','3619','3620')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.SixthCategories.Add(new SixthCategory
                            {
                                FifthCategoryId = reader["FifthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }

        public async Task<CategoryResponse> GetCategoriesForToursTravels()
        {
            var response = new CategoryResponse
            {

                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>(),
                SixthCategories = new List<SixthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='226'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('1622','1623','1624','1625','1626','1627','1628','2606')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('3616','3617','3618','4287','4288','4289','4290','4291','4306','4307','4308','4309','4310','4311')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Sixth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[SixthCategory] WHERE FifthCategoryID in ('3621','3622','3623','3624','3625','3626','3627','3628','3629','3630')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.SixthCategories.Add(new SixthCategory
                            {
                                FifthCategoryId = reader["FifthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForCourier()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='238'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('1666','1667','1668','1669','1670','1671','1672','1673')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }

        public async Task<CategoryResponse> GetCategoriesForLeafing()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='245'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('1423','1424','1732','1733','1734','1735','1736','1737','1738')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }

        public async Task<CategoryResponse> GetCategoriesForConsultants()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>(),
                FifthCategories = new List<FifthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] WHERE SecondCategoryID='267'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('1815','1818','1819','1822','1823','1825','1827','1828','1830','1831','1833','1835')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fifth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FifthCategory] WHERE FourthCategoryID in ('4192','4193','4194','4195','4199','4196','4197','4198','4205','4206','4207','4201','4202','4203','4204','4200','4208')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FifthCategories.Add(new FifthCategory
                            {
                                FourthCategoryId = reader["FourthCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForSurveyors()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='268'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('1845','1846','1847','1848','1849','1850','1851','1852','1853','1854','1855','1856','1857','1858','1859','1860','1861','1862','1863','1864','1865','1866','1867','1868','1869','1870','1871','1872','1873','1874','1875','1876','1877','1878','1879','1880','1881','1882','1883')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }


        public async Task<CategoryResponse> GetCategoriesForEngineers()
        {
            var response = new CategoryResponse
            {
                ThirdCategories = new List<ThirdCategory>(),
                FourthCategories = new List<FourthCategory>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Fetch Third Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[ThirdCategory] where SecondCategoryID='279'", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.ThirdCategories.Add(new ThirdCategory
                            {
                                SecondCategoryId = reader["SecondCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }

                // Fetch Fourth Categories
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [cat].[FourthCategory] WHERE ThirdCategoryID in('1922','1923','1924','1925','1926','2573','2574','2593')", conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.FourthCategories.Add(new FourthCategory
                            {
                                ThirdCategoryId = reader["ThirdCategoryID"].ToString(),
                                ImageURL = reader["URL"].ToString(),
                                Name = reader["Name"].ToString(),
                                SearchKeywordName = reader["SearchKeywordName"].ToString(),
                                SortOrder = reader["SortOrder"].ToString()
                            });
                        }
                    }
                }
            }

            return response;
        }
    }
}
