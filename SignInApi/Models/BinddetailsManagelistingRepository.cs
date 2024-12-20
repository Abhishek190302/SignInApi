﻿using System.Data.SqlClient;
using System.Data;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;

namespace SignInApi.Models
{
    public class BinddetailsManagelistingRepository
    {
        private readonly string _connectionString;
        private readonly string _connectionUser;
        public BinddetailsManagelistingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MimListing");
            _connectionUser = configuration.GetConnectionString("MimUser");
        }

        public async Task<Listing> GetListingByOwnerIdAsync(string ownerId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Listing] WHERE OwnerGuid = @OwnerGuid", conn);
                    cmd.Parameters.AddWithValue("@OwnerGuid", ownerId);
                    await conn.OpenAsync();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        return new Listing
                        {
                            Listingid = row.Field<int?>("ListingID") ?? 0,
                            OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                            CreatedDate = row.Field<DateTime?>("CreatedDate") ?? default(DateTime),
                            CreatedTime = row.Field<DateTime?>("CreatedTime") ?? default(DateTime),
                            IPAddress = row.Field<string>("IPAddress") ?? string.Empty,
                            Status = row.Field<int?>("Status") ?? 0,
                            CompanyName = row.Field<string>("CompanyName") ?? string.Empty,
                            BusinessCategory = row.Field<string>("BusinessCategory") ?? string.Empty,
                            NatureOfBusiness = row.Field<string>("NatureOfBusiness") ?? string.Empty,
                            YearOfEstablishment = row.Field<DateTime>("YearOfEstablishment"),
                            NumberOfEmployees = row.Field<int?>("NumberOfEmployees") ?? 0,
                            Turnover = row.Field<string>("Turnover") ?? string.Empty,
                            GSTNumber = row.Field<string>("GSTNumber") ?? string.Empty,
                            Description = row.Field<string>("Description") ?? string.Empty,
                            Name = row.Field<string>("Name") ?? string.Empty,
                            LastName = row.Field<string>("LastName") ?? string.Empty,
                            Gender = row.Field<string>("Gender") ?? string.Empty,
                            Designation = row.Field<string>("Designation") ?? string.Empty,
                            ListingURL = row.Field<string>("ListingURL") ?? string.Empty,
                            ApprovedOrRejectedBy = row.Field<bool?>("ApprovedOrRejectedBy") ?? false,
                            Rejected = row.Field<bool?>("Rejected") ?? false,
                            Steps = row.Field<int?>("Steps") ?? 0,
                            Id = row.Field<Guid?>("Id") ?? Guid.Empty,
                            ClaimedListing = row.Field<bool?>("ClaimedListing") ?? false,
                            SelfCreated = row.Field<bool?>("SelfCreated") ?? false
                        };
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<Communication> GetCommunicationByListingIdAsync(int listingid , string emailid)
        {
            string email = string.Empty;

            using (SqlConnection conn = new SqlConnection(_connectionUser))
            {
                SqlCommand cmd = new SqlCommand("SELECT Email FROM [dbo].[AspNetUsers] WHERE Email = @Email", conn);
                cmd.Parameters.AddWithValue("@Email", emailid);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if(dt.Rows.Count>0)
                {
                    email = dt.Rows[0]["Email"].ToString();
                }
            }
            
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Communication] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingid);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Communication
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        ListingID = row.Field<int?>("ListingID") ?? 0,
                        Email = email,
                        Mobile = row.Field<string>("Mobile") ?? string.Empty,
                        Telephone = row.Field<string>("Telephone") ?? string.Empty,
                        TelephoneSecond = row.Field<string>("TelephoneSecond") ?? string.Empty,
                        Whatsapp = row.Field<string>("Whatsapp") ?? string.Empty,
                        Website = row.Field<string>("Website") ?? string.Empty,
                        TollFree = row.Field<string>("TollFree") ?? string.Empty,
                        IPAddress = row.Field<string>("IPAddress") ?? string.Empty,
                        Language = row.Field<string>("Language") ?? string.Empty,

                    };
                }
                return null;
            }
        }


        public async Task<Communication> GetCommunicationRegisteremailAsync(string emailid)
        {
            string email = string.Empty;
            string phonenumber = string.Empty;

            using (SqlConnection conn = new SqlConnection(_connectionUser))
            {
                SqlCommand cmd = new SqlCommand("SELECT Email,PhoneNumber FROM [dbo].[AspNetUsers] WHERE Email = @Email", conn);
                cmd.Parameters.AddWithValue("@Email", emailid);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    email = dt.Rows[0]["Email"].ToString();
                    phonenumber = dt.Rows[0]["PhoneNumber"].ToString();
                }
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Communication]", conn);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Communication
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        ListingID = row.Field<int?>("ListingID") ?? 0,
                        Email = email,
                        Mobile = phonenumber,
                        Telephone = string.Empty,
                        TelephoneSecond = string.Empty,
                        Whatsapp = string.Empty,
                        Website =  string.Empty,
                        TollFree = string.Empty,
                        IPAddress = string.Empty,
                        Language = string.Empty,

                    };
                }
                return null;
            }
        }

        public async Task<Address> GetAddressByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Address] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Address
                    {
                        ListingID = row.Field<int?>("ListingID") ?? 0,
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        IPAddress = row.Field<string>("IPAddress") ?? string.Empty,
                        CountryID = row.Field<int?>("CountryID") ?? 0,
                        StateID = row.Field<int?>("StateID") ?? 0,
                        CityID = row.Field<int?>("City") ?? 0,
                        AssemblyID = row.Field<int?>("AssemblyID") ?? 0,
                        PincodeID = row.Field<int?>("PincodeID") ?? 0,
                        LocalityID = row.Field<int?>("LocalityID") ?? 0,
                        LocalAddress = row.Field<string>("LocalAddress") ?? string.Empty,

                    };
                }
                return null;
            }
        }

        public async Task<Categories> GetCategoryByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Categories] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Categories
                    {
                        ListingID = row.Field<int?>("ListingID") ?? 0,
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        IPAddress = row.Field<string>("IPAddress") ?? string.Empty,
                        FirstCategoryID = row.Field<int?>("FirstCategoryID") ?? 0,
                        SecondCategoryID = row.Field<int?>("SecondCategoryID") ?? 0,
                        ThirdCategoryID = row.Field<string>("ThirdCategories") ?? string.Empty,
                        FourthCategoryID = row.Field<string>("FourthCategories") ?? string.Empty,
                        FifthCategoryID = row.Field<string>("FifthCategories") ?? string.Empty,
                        SixthCategoryID = row.Field<string>("SixthCategories") ?? string.Empty,
                    };
                }
                return null;
            }
        }


        public async Task<FirstBussinessCategories> GetCategoryByBussinessCategoryAsync(string bussinessCategory)
        {
            string bussinesscategory = string.Empty;

            using (SqlConnection conn = new SqlConnection(_connectionUser))
            {
                SqlCommand cmd = new SqlCommand("SELECT BusinessCategory FROM [dbo].[AspNetUsers] WHERE BusinessCategory = @BusinessCategory", conn);
                cmd.Parameters.AddWithValue("@BusinessCategory", bussinessCategory);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    bussinesscategory = dt.Rows[0]["BusinessCategory"].ToString(); 
                }
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Categories]", conn);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new FirstBussinessCategories
                    {
                        ListingID = row.Field<int?>("ListingID") ?? 0,
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        IPAddress = row.Field<string>("IPAddress") ?? string.Empty,
                        firstCategoryID = bussinesscategory
                    };
                }
                return null;
            }
        }


        public async Task<Specialisation> GetSpecialisationByListingId(int listingId)
        {
            Specialisation specialisation = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Specialisation] WHERE ListingID = @ListingID", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", listingId);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            DataRow row = dataTable.Rows[0];
                            specialisation = new Specialisation
                            {
                                ListingID = Convert.ToInt32(row["ListingID"]),
                                OwnerGuid = Convert.ToString(row["OwnerGuid"]),
                                IPAddress = Convert.ToString(row["IPAddress"]),
                                AcceptTenderWork = Convert.ToBoolean(row["AcceptTenderWork"]),
                                Bank = Convert.ToBoolean(row["Banks"]),
                                BeautyParlors = Convert.ToBoolean(row["BeautyParlors"]),
                                Bungalow = Convert.ToBoolean(row["Bungalow"]),
                                CallCenter = Convert.ToBoolean(row["CallCenter"]),
                                Church = Convert.ToBoolean(row["Church"]),
                                Company = Convert.ToBoolean(row["Company"]),
                                ComputerInstitute = Convert.ToBoolean(row["ComputerInstitute"]),
                                Dispensary = Convert.ToBoolean(row["Dispensary"]),
                                ExhibitionStall = Convert.ToBoolean(row["ExhibitionStall"]),
                                Factory = Convert.ToBoolean(row["Factory"]),
                                Farmhouse = Convert.ToBoolean(row["Farmhouse"]),
                                Gurudwara = Convert.ToBoolean(row["Gurudwara"]),
                                Gym = Convert.ToBoolean(row["Gym"]),
                                HealthClub = Convert.ToBoolean(row["HealthClub"]),
                                Home = Convert.ToBoolean(row["Home"]),
                                Hospital = Convert.ToBoolean(row["Hospital"]),
                                Hotel = Convert.ToBoolean(row["Hotel"]),
                                Laboratory = Convert.ToBoolean(row["Laboratory"]),
                                Mandir = Convert.ToBoolean(row["Mandir"]),
                                Mosque = Convert.ToBoolean(row["Mosque"]),
                                Office = Convert.ToBoolean(row["Office"]),
                                Plazas = Convert.ToBoolean(row["Plazas"]),
                                ResidentialSociety = Convert.ToBoolean(row["ResidentialSociety"]),
                                Resorts = Convert.ToBoolean(row["Resorts"]),
                                Restaurants = Convert.ToBoolean(row["Restaurants"]),
                                Salons = Convert.ToBoolean(row["Salons"]),
                                Shop = Convert.ToBoolean(row["Shop"]),
                                ShoppingMall = Convert.ToBoolean(row["ShoppingMall"]),
                                Showroom = Convert.ToBoolean(row["Showroom"]),
                                Warehouse = Convert.ToBoolean(row["Warehouse"])
                            };
                        }
                    }
                }
            }
            return specialisation;
        }

        public async Task<WorkingHours> GetWorkingHoursByListingId(int listingId)
        {
            WorkingHours workinghours = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[WorkingHours] WHERE ListingID = @ListingID", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", listingId);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            workinghours = new WorkingHours
                            {
                                ListingID = Convert.ToInt32(row["ListingID"]),
                                OwnerGuid = Convert.ToString(row["OwnerGuid"]),
                                IPAddress = Convert.ToString(row["IPAddress"]),
                                MondayFrom = row.Field<DateTime>("MondayFrom"),
                                MondayTo = row.Field<DateTime>("MondayTo"),
                                TuesdayFrom = row.Field<DateTime>("TuesdayFrom"),
                                TuesdayTo = row.Field<DateTime>("TuesdayTo"),
                                WednesdayFrom = row.Field<DateTime>("WednesdayFrom"),
                                WednesdayTo = row.Field<DateTime>("WednesdayTo"),
                                ThursdayFrom = row.Field<DateTime>("ThursdayFrom"),
                                ThursdayTo = row.Field<DateTime>("ThursdayTo"),
                                FridayFrom = row.Field<DateTime>("FridayFrom"),
                                FridayTo = row.Field<DateTime>("FridayTo"),
                                SaturdayFrom = row.Field<DateTime>("SaturdayFrom"),
                                SaturdayTo = row.Field<DateTime>("SaturdayTo"),
                                SundayFrom = row.Field<DateTime>("SundayFrom"),
                                SundayTo = row.Field<DateTime>("SundayTo"),
                                SaturdayHoliday = Convert.ToBoolean(row["SaturdayHoliday"]),
                                SundayHoliday = Convert.ToBoolean(row["SundayHoliday"]),
                            };
                        }
                    }
                }
            }
            return workinghours;
        }

        public async Task<PaymentMode> GetPaymentModeByListingId(int listingId)
        {
            PaymentMode paymentmode = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[PaymentMode] WHERE ListingID = @ListingID", conn))
                {
                    cmd.Parameters.AddWithValue("@ListingID", listingId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            paymentmode = new PaymentMode
                            {
                                ListingID = Convert.ToInt32(row["ListingID"]),
                                OwnerGuid = Convert.ToString(row["OwnerGuid"]),
                                IPAddress = Convert.ToString(row["IPAddress"]),
                                Cash = Convert.ToBoolean(row["Cash"]),
                                Cheque = Convert.ToBoolean(row["Cheque"]),
                                RtgsNeft = Convert.ToBoolean(row["RtgsNeft"]),
                                DebitCard = Convert.ToBoolean(row["DebitCard"]),
                                CreditCard = Convert.ToBoolean(row["CreditCard"]),
                                NetBanking = Convert.ToBoolean(row["NetBanking"]),
                            };
                        }
                    }
                }
            }
            return paymentmode;
        }

        public async Task<SocialNetwork> GetSocialNetworkByListingId(int listingId)
        {
            SocialNetwork socialNetwork = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[SocialNetwork] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        socialNetwork = new SocialNetwork
                        {
                            ListingID = (int)row["ListingID"],
                            OwnerGuid = row["OwnerGuid"].ToString(),
                            IPAddress = row["IPAddress"].ToString(),
                            Facebook = row["Facebook"].ToString(),
                            WhatsappGroupLink = row["WhatsappGroupLink"].ToString(),
                            Linkedin = row["Linkedin"].ToString(),
                            Twitter = row["Twitter"].ToString(),
                            Youtube = row["Youtube"].ToString(),
                            Instagram = row["Instagram"].ToString(),
                            Pinterest = row["Pinterest"].ToString()
                        };
                    }
                }
            }

            return socialNetwork;
        }

        public async Task<logoImage> GetlogoImageByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[LogoImage] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new logoImage
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        Listingid = row.Field<int?>("ListingID") ?? 0,
                        Imagepath = row.Field<string>("ImagePath") ?? string.Empty,
                        craeteddate = row.Field<DateTime>("CreatedDate"),
                        updateddate = row.Field<DateTime>("UpdateDate"),
                    };
                }
                return null;
            }
        }

        public async Task<OwnerImage> GetOwnerImageByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT OwnerGuid, ListingID, ImagePath, Designation, OwnerName, LastName, CreatedDate, UpdateDate, CountryID, StateID, MrndMs " +
                    "FROM [dbo].[OwnerImage] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);

                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow firstRow = dt.Rows[0];

                    // Assuming the ImagePath column contains a single concatenated string
                    string concatenatedImagePaths = firstRow.Field<string>("ImagePath") ?? string.Empty;
                    List<string> imagePaths = concatenatedImagePaths
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();

                    string concatenatedFirstname = firstRow.Field<string>("OwnerName") ?? string.Empty;
                    List<string> Firstname = concatenatedFirstname
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();

                    string concatenatedLastname = firstRow.Field<string>("LastName") ?? string.Empty;
                    List<string> Lastname = concatenatedLastname
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();

                    string concatenatedDesignation = firstRow.Field<string>("Designation") ?? string.Empty;
                    List<string> Designation = concatenatedDesignation
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();

                    string concatenatedPrefix = firstRow.Field<string>("MrndMs") ?? string.Empty;
                    List<string> Prefix = concatenatedPrefix
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();




                    return new OwnerImage
                    {
                        OwnerGuid = firstRow.Field<string>("OwnerGuid") ?? string.Empty,
                        Listingid = firstRow.Field<int?>("ListingID") ?? 0,
                        Imagepath = imagePaths,
                        Designation = Designation,
                        OwnerName = Firstname,
                        LastName = Lastname,
                        craeteddate = firstRow.Field<DateTime>("CreatedDate"),
                        updateddate = firstRow.Field<DateTime>("UpdateDate"),
                        CountryId = firstRow.Field<int?>("CountryID") ?? 0,
                        StateId = firstRow.Field<int?>("StateID") ?? 0,
                        Prefix = Prefix,
                    };
                }
                return null;
            }
        }

        public async Task<GallerysImage> GetGallerysImageByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[GalleryImage] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    string concatenatedImagePaths = row.Field<string>("ImagePath") ?? string.Empty;
                    List<string> imagePaths = concatenatedImagePaths
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();

                    string concatenatedImageTitle = row.Field<string>("ImageTitle") ?? string.Empty;
                    List<string> imageTitle = concatenatedImageTitle
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();


                    return new GallerysImage
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        Listingid = row.Field<int?>("ListingID") ?? 0,
                        Imagepath = imagePaths,
                        Imagetitle = imageTitle,
                        craeteddate = row.Field<DateTime>("CreatedDate"),
                        updateddate = row.Field<DateTime>("UpdateDate"),
                    };
                }
                return null;
            }
        }

        public async Task<BannerImage> GetBannerImageByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[BannerDetail] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new BannerImage
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        Listingid = row.Field<int?>("ListingID") ?? 0,
                        Imagepath = row.Field<string>("ImagePath") ?? string.Empty,
                        Imagetitle = row.Field<string>("ImageTitle") ?? string.Empty,
                        craeteddate = row.Field<DateTime>("CreatedDate"),
                        updateddate = row.Field<DateTime>("UpdateDate"),
                    };
                }
                return null;
            }
        }

        public async Task<CertificateImage> GetCertificateImageByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[CertificationDetail] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {

                    DataRow row = dt.Rows[0];

                    string concatenatedImagePaths = row.Field<string>("ImagePath") ?? string.Empty;
                    List<string> imagePaths = concatenatedImagePaths
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();

                    string concatenatedImageTitle = row.Field<string>("ImageTitle") ?? string.Empty;
                    List<string> imageTitle = concatenatedImageTitle
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();


                    return new CertificateImage
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        Listingid = row.Field<int?>("ListingID") ?? 0,
                        Imagepath = imagePaths,
                        Imagetitle = imageTitle,
                        craeteddate = row.Field<DateTime>("CreatedDate"),
                        updateddate = row.Field<DateTime>("UpdateDate"),
                    };


                    //DataRow row = dt.Rows[0];
                    //return new CertificateImage
                    //{
                    //    OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                    //    Listingid = row.Field<int?>("ListingID") ?? 0,
                    //    Imagepath = row.Field<string>("ImagePath") ?? string.Empty,
                    //    Imagetitle = row.Field<string>("ImageTitle") ?? string.Empty,
                    //    craeteddate = row.Field<DateTime>("CreatedDate"),
                    //    updateddate = row.Field<DateTime>("UpdateDate"),
                    //};
                }
                return null;
            }
        }

        public async Task<ClientImage> GetClientImageByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[ClientDetail] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    string concatenatedImagePaths = row.Field<string>("ImagePath") ?? string.Empty;
                    List<string> imagePaths = concatenatedImagePaths
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();

                    string concatenatedImageTitle = row.Field<string>("ImageTitle") ?? string.Empty;
                    List<string> imageTitle = concatenatedImageTitle
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .ToList();


                    return new ClientImage
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        Listingid = row.Field<int?>("ListingID") ?? 0,
                        Imagepath = imagePaths,
                        Imagetitle = imageTitle,
                        craeteddate = row.Field<DateTime>("CreatedDate"),
                        updateddate = row.Field<DateTime>("UpdateDate"),
                    };

                    //DataRow row = dt.Rows[0];
                    //return new ClientImage
                    //{
                    //    OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                    //    Listingid = row.Field<int?>("ListingID") ?? 0,
                    //    Imagepath = row.Field<string>("ImagePath") ?? string.Empty,
                    //    Imagetitle = row.Field<string>("ImageTitle") ?? string.Empty,
                    //    craeteddate = row.Field<DateTime>("CreatedDate"),
                    //    updateddate = row.Field<DateTime>("UpdateDate"),
                    //};
                }
                return null;
            }
        }

        public async Task<int?> GetListingIdByPhoneNumberAsync(string phoneNumber)
        {
            try
            {
                int? listingId = null;

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = @"SELECT ListingID FROM [listing].[Communication] WHERE Mobile = @PhoneNumber";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        var result = await command.ExecuteScalarAsync();

                        if (result != null && int.TryParse(result.ToString(), out int id))
                        {
                            listingId = id;
                        }
                    }
                }

                return listingId;
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }

        public async Task<Listing> GetListingByListingIdAsync(int listingid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Listing] WHERE ListingID = @ListingID", conn);
                    cmd.Parameters.AddWithValue("@ListingID", listingid);
                    await conn.OpenAsync();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        return new Listing
                        {
                            Listingid = row.Field<int?>("ListingID") ?? 0,
                            OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                            CreatedDate = row.Field<DateTime?>("CreatedDate") ?? default(DateTime),
                            CreatedTime = row.Field<DateTime?>("CreatedTime") ?? default(DateTime),
                            IPAddress = row.Field<string>("IPAddress") ?? string.Empty,
                            Status = row.Field<int?>("Status") ?? 0,
                            CompanyName = row.Field<string>("CompanyName") ?? string.Empty,
                            BusinessCategory = row.Field<string>("BusinessCategory") ?? string.Empty,
                            NatureOfBusiness = row.Field<string>("NatureOfBusiness") ?? string.Empty,
                            YearOfEstablishment = row.Field<DateTime>("YearOfEstablishment"),
                            NumberOfEmployees = row.Field<int?>("NumberOfEmployees") ?? 0,
                            Turnover = row.Field<string>("Turnover") ?? string.Empty,
                            GSTNumber = row.Field<string>("GSTNumber") ?? string.Empty,
                            Description = row.Field<string>("Description") ?? string.Empty,
                            Name = row.Field<string>("Name") ?? string.Empty,
                            LastName = row.Field<string>("LastName") ?? string.Empty,
                            Gender = row.Field<string>("Gender") ?? string.Empty,
                            Designation = row.Field<string>("Designation") ?? string.Empty,
                            ListingURL = row.Field<string>("ListingURL") ?? string.Empty,
                            ApprovedOrRejectedBy = row.Field<bool?>("ApprovedOrRejectedBy") ?? false,
                            Rejected = row.Field<bool?>("Rejected") ?? false,
                            Steps = row.Field<int?>("Steps") ?? 0,
                            Id = row.Field<Guid?>("Id") ?? Guid.Empty,
                            ClaimedListing = row.Field<bool?>("ClaimedListing") ?? false,
                            SelfCreated = row.Field<bool?>("SelfCreated") ?? false
                        };
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<Communication> GetCommunicationByClaimListingIdAsync(int listingid)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [listing].[Communication] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingid);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Communication
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        ListingID = row.Field<int?>("ListingID") ?? 0,
                        Email = row.Field<string>("Email") ?? string.Empty,
                        Mobile = row.Field<string>("Mobile") ?? string.Empty,
                        Telephone = row.Field<string>("Telephone") ?? string.Empty,
                        TelephoneSecond = row.Field<string>("TelephoneSecond") ?? string.Empty,
                        Whatsapp = row.Field<string>("Whatsapp") ?? string.Empty,
                        Website = row.Field<string>("Website") ?? string.Empty,
                        TollFree = row.Field<string>("TollFree") ?? string.Empty,
                        IPAddress = row.Field<string>("IPAddress") ?? string.Empty,
                        Language = row.Field<string>("Language") ?? string.Empty,

                    };
                }
                return null;
            }
        }

        public async Task<SavePincodesModel> GetWorkingAreaByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[WorkingArea] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);
                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    var pincodes = row.Field<string>("PincodeID") ?? string.Empty;
                    List<int> selectedPincodes = pincodes
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries) // Split by comma
                        .Select(p => int.TryParse(p, out int value) ? value : 0) // Parse to int
                        .Where(p => p > 0)
                        .ToList();

                    return new SavePincodesModel
                    {
                        ListingID = row.Field<int?>("ListingID") ?? 0,                    
                        CountryID = row.Field<int?>("CountryID") ?? 0,
                        StateID = row.Field<int?>("StateID") ?? 0,
                        CityID = row.Field<int?>("CityID") ?? 0,
                        AssemblyID = row.Field<int?>("AssemblyID") ?? 0,
                        SelectedPincodesContainer = selectedPincodes,
                    };
                }
                return null;
            }
        }
    }
}
