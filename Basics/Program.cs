using Basics.AuthorizationRequirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("AuthCookie")
.AddCookie("AuthCookie", config =>
{
    config.Cookie.Name = "SimpleCookie";
    //default is --> config.LoginPath = "/Account/Login
    config.LoginPath = "/home/authenticate";
});

builder.Services.AddAuthorization(config =>
{
    //method 1 --> when we call secret page everything is ok

    //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    //var defaultAuthPolicy=defaultAuthBuilder
    //    .RequireAuthenticatedUser()
    //    .Build();
    //config.DefaultPolicy = defaultAuthPolicy;


    //method 2  --> when we call secret page --->AccessDeny ---->authorization process need to mobilephone clain to authorize request
    //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    //var defaultAuthPolicy=defaultAuthBuilder
    //    .RequireAuthenticatedUser()
    //    .RequireClaim(ClaimTypes.MobilePhone)
    //    .Build();
    //config.DefaultPolicy = defaultAuthPolicy;


    //method 2
    //config.AddPolicy("MobileNumberPolicy", policyBuilder =>
    //{
    //    policyBuilder.RequireClaim(ClaimTypes.MobilePhone);
    //});

    //method 3
    //config.AddPolicy("MobileNumberPolicy", policyBuilder =>
    //{
    //    policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.MobilePhone));
    //});


    //method 4
    config.AddPolicy("MobileNumberPolicy", policyBuilder =>
    {
        policyBuilder.CustomAuthorizationPolicyBuilder(ClaimTypes.MobilePhone);
    });

    config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));

});

builder.Services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();

builder.Services.AddControllersWithViews();

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
});


app.Run();
