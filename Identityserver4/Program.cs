using IdentityServer4.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.UseWebRoot("wwwroot");
// builder.WebHost.UseStaticWebAssets();


builder.Services.AddDbContext<AppDbContext>(config => config.UseInMemoryDatabase("Memory"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
{
    config.Password.RequiredLength = 3;
    config.Password.RequireUppercase = false;
    config.Password.RequireLowercase = false;
    config.Password.RequireDigit = false;
    config.Password.RequireNonAlphanumeric = false;
    config.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(config =>
{
    config.Cookie.Name = "IdentityCookie";
    config.LoginPath = "/home/login";
});

// builder.Services.AddAuthentication("AuthCookie")
// .AddCookie("AuthCookie", config =>
// {
//     config.Cookie.Name = "SimpleCookie";
//     config.LoginPath = "/home/authenticate";
// });


var mailKitOptions = builder.Configuration.GetSection("MailKit").Get<MailKitOptions>();
builder.Services.AddMailKit(config => config.UseMailKit(mailKitOptions));

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var rootPath = builder.Environment.ContentRootPath;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseRouting();

//who you are?
app.UseAuthentication();

//what do i do/ where could i go
app.UseAuthorization();

app.UseEndpoints(ep =>
{
    ep.MapDefaultControllerRoute();
    ep.MapRazorPages();
});


app.Run();
