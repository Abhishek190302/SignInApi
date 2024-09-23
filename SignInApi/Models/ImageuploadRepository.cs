using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace SignInApi.Models
{
    public class ImageuploadRepository
    {
        private readonly string _connectionstring;
        public ImageuploadRepository(IConfiguration configuration)
        {
            _connectionstring = configuration.GetConnectionString("MimListing");
        }

        public async Task<logoImage> GetlogoImageByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionstring))
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
                    var logoImg = new logoImage
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        Listingid = row.Field<int?>("ListingID") ?? 0,
                        Imagepath = row.Field<string>("ImagePath") ?? string.Empty,
                        craeteddate = row.Field<DateTime>("CreatedDate"),
                        updateddate = row.Field<DateTime>("UpdateDate"),
                    };
                    Console.WriteLine($"Logo image found: {logoImg.Listingid}");
                    return logoImg;
                }

                Console.WriteLine("No logo image found.");
                return null;
            }
        }


        public async Task<OwnerImage> GetOwnerImageByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionstring))
            {
                // Modify the query to fetch all relevant details
                SqlCommand cmd = new SqlCommand(
                    "SELECT OwnerGuid, ListingID, ImagePath, Designation, OwnerName, LastName, CreatedDate, UpdateDate, CountryID, StateID " +
                    "FROM [dbo].[OwnerImage] WHERE ListingID = @ListingID", conn);
                cmd.Parameters.AddWithValue("@ListingID", listingId);

                await conn.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow firstRow = dt.Rows[0];

                    // Create a list to hold all image paths
                    List<string> imagePaths = new List<string>();
                    foreach (DataRow row in dt.Rows)
                    {
                        string imagePath = row.Field<string>("ImagePath");
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            imagePaths.Add(imagePath);
                        }
                    }

                    return new OwnerImage
                    {
                        OwnerGuid = firstRow.Field<string>("OwnerGuid") ?? string.Empty,
                        Listingid = firstRow.Field<int?>("ListingID") ?? 0,
                        Imagepath = imagePaths,
                        Designation = firstRow.Field<string>("Designation") ?? string.Empty,
                        OwnerName = firstRow.Field<string>("OwnerName") ?? string.Empty,
                        LastName = firstRow.Field<string>("LastName") ?? string.Empty,
                        craeteddate = firstRow.Field<DateTime>("CreatedDate"),
                        updateddate = firstRow.Field<DateTime>("UpdateDate"),
                        CountryId = firstRow.Field<int?>("CountryID") ?? 0,
                        StateId = firstRow.Field<int?>("StateID") ?? 0,
                    };
                }
                return null;
            }
        }

        //public async Task<OwnerImage> GetOwnerImageByListingIdAsync(int listingId)
        //{
        //    using (SqlConnection conn = new SqlConnection(_connectionstring))
        //    {
        //        SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[OwnerImage] WHERE ListingID = @ListingID", conn);
        //        cmd.Parameters.AddWithValue("@ListingID", listingId);
        //        await conn.OpenAsync();
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt);
        //        if (dt.Rows.Count > 0)
        //        {
        //            DataRow row = dt.Rows[0];
        //            return new OwnerImage
        //            {
        //                OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
        //                Listingid = row.Field<int?>("ListingID") ?? 0,
        //                Imagepath = row.Field<string>("ImagePath") ?? string.Empty,
        //                Designation = row.Field<string>("Designation") ?? string.Empty,
        //                OwnerName = row.Field<string>("OwnerName") ?? string.Empty,
        //                LastName = row.Field<string>("LastName") ?? string.Empty,
        //                craeteddate = row.Field<DateTime>("CreatedDate"),
        //                updateddate = row.Field<DateTime>("UpdateDate"),
        //                CountryId = row.Field<int?>("CountryID") ?? 0,
        //                StateId = row.Field<int?>("StateID") ?? 0,
        //            };
        //        }

        //        Console.WriteLine("No logo image found.");
        //        return null;
        //    }
        //}

        public async Task<GallerysImage> GetGallerysImageByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionstring))
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

                    List<string> imagePaths = new List<string>();
                    foreach (DataRow rows in dt.Rows)
                    {
                        string imagePath = rows.Field<string>("ImagePath");
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            imagePaths.Add(imagePath);
                        }
                    }


                    return new GallerysImage
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        Listingid = row.Field<int?>("ListingID") ?? 0,
                        Imagepath = imagePaths,
                        Imagetitle = row.Field<string>("ImageTitle") ?? string.Empty,
                        craeteddate = row.Field<DateTime>("CreatedDate"),
                        updateddate = row.Field<DateTime>("UpdateDate"),
                    };
                }
                return null;
            }
        }

        public async Task<BannerImage> GetBannerImageByListingIdAsync(int listingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionstring))
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
            using (SqlConnection conn = new SqlConnection(_connectionstring))
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

                    List<string> imagePaths = new List<string>();
                    foreach (DataRow rows in dt.Rows)
                    {
                        string imagePath = rows.Field<string>("ImagePath");
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            imagePaths.Add(imagePath);
                        }
                    }


                    return new CertificateImage
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        Listingid = row.Field<int?>("ListingID") ?? 0,
                        Imagepath = imagePaths,
                        Imagetitle = row.Field<string>("ImageTitle") ?? string.Empty,
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
            using (SqlConnection conn = new SqlConnection(_connectionstring))
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

                    List<string> imagePaths = new List<string>();
                    foreach (DataRow rows in dt.Rows)
                    {
                        string imagePath = rows.Field<string>("ImagePath");
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            imagePaths.Add(imagePath);
                        }
                    }

                    return new ClientImage
                    {
                        OwnerGuid = row.Field<string>("OwnerGuid") ?? string.Empty,
                        Listingid = row.Field<int?>("ListingID") ?? 0,
                        Imagepath = imagePaths,
                        Imagetitle = row.Field<string>("ImageTitle") ?? string.Empty,
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
    }

    public class logoImage
    {
        public string OwnerGuid { get; set; }
        public int Listingid { get; set; }
        public string Imagepath { get; set; }
        public DateTime craeteddate { get; set; }
        public DateTime updateddate { get; set; }
    }

    public class OwnerImage
    {
        public string OwnerGuid { get; set; }
        public int Listingid { get; set; }
        public List<string> Imagepath { get; set; }
        public string Designation { get; set; }
        public string OwnerName { get; set; }
        public string LastName { get; set; }
        public DateTime craeteddate { get;set; }
        public DateTime updateddate { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
    }

    public class GallerysImage
    {
        public string OwnerGuid { get; set; }
        public int Listingid { get; set; }
        public List<string> Imagepath { get; set; }
        public string Imagetitle { get; set; }
        public DateTime craeteddate { get; set; }
        public DateTime updateddate { get; set; }
    }

    public class BannerImage
    {
        public string OwnerGuid { get; set; }
        public int Listingid { get; set; }
        public string Imagepath { get; set; }
        public string Imagetitle { get; set; }
        public DateTime craeteddate { get; set; }
        public DateTime updateddate { get; set; }
    }

    public class CertificateImage
    {
        public string OwnerGuid { get; set; }
        public int Listingid { get; set; }
        public List<string> Imagepath { get; set; }
        public string Imagetitle { get; set; }
        public DateTime craeteddate { get; set; }
        public DateTime updateddate { get; set; }
    }

    public class ClientImage
    {
        public string OwnerGuid { get; set; }
        public int Listingid { get; set; }
        public List<string> Imagepath { get; set; }
        public string Imagetitle { get; set; }
        public DateTime craeteddate { get; set; }
        public DateTime updateddate { get; set; }
    }
}
