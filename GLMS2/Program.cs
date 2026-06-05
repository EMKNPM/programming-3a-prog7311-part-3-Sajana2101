using GLMS2.Data;
using GLMS2.Interfaces;
using GLMS2.Services;
using GLMS2.Services.Factories;
using GLMS2.Services.Mediator;
using Microsoft.EntityFrameworkCore;
using GLMS2.Services.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection - Services
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IContractFactory, ContractFactory>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IMediator, GLMSMediator>();

builder.Services.AddHttpClient("GLMSApi", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"]
        ?? throw new InvalidOperationException("ApiSettings:BaseUrl is missing.");

    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IContractApiService, ContractApiService>();
builder.Services.AddScoped<IClientApiService, ClientApiService>();

// HttpClient for external currency API
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
