using GLMS2.Data;
using GLMS2.Interfaces;
using GLMS2.Services;
using GLMS2.Services.Api;
using GLMS2.Services.Factories;
using GLMS2.Services.Mediator;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC controllers and views
builder.Services.AddControllersWithViews();

// MVC frontend HttpClient used to call the backend API
builder.Services.AddHttpClient("GLMSApi", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"]
        ?? throw new InvalidOperationException("ApiSettings:BaseUrl is missing.");

    client.BaseAddress = new Uri(baseUrl);
});

// Allows MVC services to access HttpContext and Session
builder.Services.AddHttpContextAccessor();

// Enables session storage for the JWT token
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Frontend API services
builder.Services.AddScoped<IContractApiService, ContractApiService>();
builder.Services.AddScoped<IClientApiService, ClientApiService>();
builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<IServiceRequestApiService, ServiceRequestApiService>();

// Existing database connection
// Keep this for now because Service Requests and other existing features may still depend on it.
// Later, once everything is fully refactored, the MVC project should no longer use the database directly.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Existing dependency injection services
// Keep these for now until the full frontend is refactored to use the API.
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IContractFactory, ContractFactory>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IMediator, GLMSMediator>();

// HttpClient for external currency API
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Session must come after UseRouting and before actions try to access session
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();