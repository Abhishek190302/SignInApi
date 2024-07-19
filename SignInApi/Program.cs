using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SignInApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Bind SmsOptions from configuration

builder.Services.AddTransient<IListingService, ListingService>();

// Register the ICategoryService
builder.Services.AddTransient<ICategoryService, CategoryService>();

// Register the ICategoryServices
builder.Services.AddTransient<ICategoryServices, CategoryServices>();



builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<CompanyDetailsRepository>();
builder.Services.AddTransient<CommunicationRepository>();
// Register the IAddressRepositery
builder.Services.AddTransient<IAddressRepositery, AddressRepositery>();
builder.Services.AddTransient<CategoryRepository>();
builder.Services.AddTransient<SpecialisationRepository>();
builder.Services.AddTransient<WorkingHoursRepository>();
builder.Services.AddTransient<PaymentModeRepository>();
builder.Services.AddTransient<SocialNetworkRepository>();
builder.Services.AddTransient<KeywordRepository>();
builder.Services.AddTransient<ListingEnquiryService>();
builder.Services.AddTransient<IUserNewProfileService, UserNewProfileService>();

builder.Services.AddTransient<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
//Configure Cors



builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("https://frontend.myinteriormart.com", "http://localhost:3000").AllowAnyMethod().AllowAnyHeader(); // Add localhost:3000 to the allowed origins
        });     
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TokenService>();

builder.Services.AddAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();

app.UseHttpsRedirection();
app.UseCors("MyAllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseMiddleware<ExceptionHandlingMiddleware>(); // Add this line

app.MapControllers();

app.Run();
