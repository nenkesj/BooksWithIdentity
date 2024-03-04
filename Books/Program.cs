using Books;
using Books.Data;
using Books.Models;
using Books.Services;
using HowTo_DBLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

IConfigurationRoot _configuration;
var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
_configuration = configBuilder.Build();

DbContextOptionsBuilder<HowToDBContext> _optionsBuilder;

_optionsBuilder = new DbContextOptionsBuilder<HowToDBContext>();
_optionsBuilder.UseSqlServer(_configuration.GetConnectionString("HowToDB"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString1 = builder.Configuration.GetConnectionString("IdentityDB");
var connectionString2 = builder.Configuration.GetConnectionString("HowToDB");

var configuration = builder.Configuration;

builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString1));

builder.Services.AddDbContext<HowToDBContext>(options =>
    options.UseSqlServer(connectionString2));

builder.Services.AddHttpsRedirection(opts =>
{
    opts.HttpsPort = 44350;
});

builder.Services.AddHttpContextAccessor();

//builder.Services.AddDefaultIdentity<IdentityUser>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddIdentity<IdentityUser, IdentityRole>(opts =>
{
    opts.Password.RequiredLength = 8;
    opts.Password.RequireDigit = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireNonAlphanumeric = false;
    opts.SignIn.RequireConfirmedAccount = true;
}).AddEntityFrameworkStores<ApplicationDbContext>()
   .AddDefaultTokenProviders();

builder.Services.Configure<SecurityStampValidatorOptions>(opts =>
{
    opts.ValidationInterval = System.TimeSpan.FromMinutes(1);
});

builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();


builder.Services.AddScoped<TokenUrlEncoderService>();

builder.Services.AddScoped<IdentityEmailService>();

builder.Services.AddAuthentication();

//var Configuration = builder.Configuration;

builder.Services.AddAuthentication()
    .AddFacebook(opts =>
    {
        opts.AppId = configuration["Facebook:AppId"];
        opts.AppSecret = configuration["Facebook:AppSecret"];

        //opts.AppId = configuration["Authentication:Facebook:AppId"];
        //opts.AppSecret = configuration["Authentication:Facebook:AppSecret"];
    })
.AddGoogle(opts =>
{
    opts.ClientId = configuration["Authentication:Google:ClientId"];
    opts.ClientSecret = configuration["Authentication:Google:ClientSecret"];
})
.AddTwitter(opts =>
{
    opts.ConsumerKey = configuration["Authentication:Twitter:ConsumerAPIKey"];
    opts.ConsumerSecret = configuration["Authentication:Twitter:ConsumerSecret"];
    opts.RetrieveUserDetails = true;
});

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Identity/SignIn";
    opts.LogoutPath = "/Identity/SignOut";
    opts.AccessDeniedPath = "/Identity/Forbidden";
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IBookRepository, EFBookRepository>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(opts =>
{
    opts.IdleTimeout = TimeSpan.FromMinutes(30);
    opts.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapRazorPages();
});

app.SeedUserStoreForDashboard();

app.Run();
