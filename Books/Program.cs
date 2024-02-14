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

builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();


builder.Services.AddScoped<TokenUrlEncoderService>();

builder.Services.AddScoped<IdentityEmailService>();

builder.Services.AddAuthentication();

var Configuration = builder.Configuration;

builder.Services.AddAuthentication();
//    .AddFacebook(opts =>
//    {
//        opts.AppId = Configuration["Facebook:AppId"];
//        opts.AppSecret = Configuration["Facebook:AppSecret"];
//    })
//    .AddTwitter(opts =>
//    {
//        opts.ConsumerKey = Configuration["Twitter:ApiKey"];
//        opts.ConsumerSecret = Configuration["Twitter:ApiSecret"];
//    });

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

app.Run();
