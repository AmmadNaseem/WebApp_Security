using Microsoft.AspNetCore.Authorization;
using WebApp_WithIdentity.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    //========cookie name is the most important aspect in cookie authentication.
    options.Cookie.Name = "MyCookieAuth";
    // for explicitly specify the login page
    options.LoginPath = "/Account/Login";
    //options.AccessDeniedPath="/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(2); // cookie will expire after 30 second
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBelongToHRDepartment",
        policy => policy
        .RequireClaim("Department", "HR")
        .Requirements.Add(new HRManagerProbationRequirement(3))); // this is for specific requirment adding.
});
builder.Services.AddSingleton<IAuthorizationHandler,HRManagerProbationRequirementHandler>();

builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
