
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Entity Framework and Identity services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//identityDbContext service
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{

})
   .AddEntityFrameworkStores<ApplicationDbContext>()
   .AddDefaultTokenProviders()
   .AddRoles<IdentityRole>();

// Configure JWT authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:secret"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
   options.TokenValidationParameters = new TokenValidationParameters
   {
       ValidateIssuer = false,
       ValidateAudience = false,
       ValidateIssuerSigningKey = true,
       IssuerSigningKey = new SymmetricSecurityKey(key)
   };
});

// Configure authorization policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminManagerUserPolicy", options => {
        options.RequireAuthenticatedUser();
        options.RequireRole("admin", "user");
    })
    .AddPolicy("AdminManagerPolicy", options => {
        options.RequireAuthenticatedUser();
        options.RequireRole("admin");
    })
    .AddPolicy("UserPolicy", options => {
        options.RequireAuthenticatedUser();
        options.RequireRole( "user");
    });


//building block
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
