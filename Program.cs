using Microsoft.EntityFrameworkCore;
using AMRVI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// Add Authentication
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Add DbContext with conditional provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Always use SQL Server (Local or Production)
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add HttpContextAccessor (diperlukan untuk PlantService)
builder.Services.AddHttpContextAccessor();

// Add PlantService
builder.Services.AddScoped<AMRVI.Services.PlantService>();

var app = builder.Build();

// Auto-migrate database (commented out - run manually with: dotnet ef database update)
/*
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}
*/



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<AMRVI.Hubs.NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
