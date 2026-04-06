using Hospital.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Hospital.Utilities;
using Hospital.Repositories.Interfaces;
using Hospital.Repositories.Implementation;
using Microsoft.AspNetCore.Identity.UI.Services;
using Hospital.Models;
using Hospital.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Support both URL-style (Render) and key=value style connection strings
var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
string npgsqlConnectionString = rawConnectionString;

if (rawConnectionString.StartsWith("postgres://") || rawConnectionString.StartsWith("postgresql://"))
{
    var uri = new Uri(rawConnectionString);
    var host = uri.Host;
    var dbPort = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    // Use Uri to safely decode userinfo — password may contain special chars
    var userInfo = uri.UserInfo;
    var colonIdx = userInfo.IndexOf(':');
    var username = Uri.UnescapeDataString(userInfo[..colonIdx]);
    var password = Uri.UnescapeDataString(userInfo[(colonIdx + 1)..]);
    npgsqlConnectionString = $"Host={host};Port={dbPort};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(npgsqlConnectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddScoped<IDbIntializer, DbIntializer>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddTransient<IHospitalInfo, HospitalInfoService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddTransient<IDoctorService, DoctorService>();
builder.Services.AddTransient<IRoomService, RoomService>(); 
builder.Services.AddTransient<IContactService, ContactService>();
builder.Services.AddTransient<IApplicationUserService, ApplicationUserService>();
builder.Services.AddTransient<ILabService, LabService>();
builder.Services.AddTransient<IMedicalAssistantService, MedicalAssistantService>();
builder.Services.AddTransient<ISafetyCheckerService, SafetyCheckerService>();
builder.Services.AddTransient<IBedStatusHandler, BedStatusHandler>();
builder.Services.AddTransient<IRoomAllocationService, RoomAllocationService>();
builder.Services.AddTransient<IAIAllocationService, AIAllocationService>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Bind to Render's PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://+:{port}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Skip HTTPS redirect in container environments (Render handles TLS externally)
if (!app.Environment.IsEnvironment("Container") && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RENDER")))
    app.UseHttpsRedirection();
else if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseStaticFiles();
DataSeeding();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{Area=Admin}/{controller=Hospitals}/{action=Index}/{id?}");

app.Run();

void DataSeeding()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.
            GetRequiredService<IDbIntializer>();
        dbInitializer.Initialize();
        // Seed data logic here
    }
}
