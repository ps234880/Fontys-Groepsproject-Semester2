using Dashboard_Sales_Dev.DAL.Repositories;
using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing from the configuration.");

builder.Services.AddScoped<IClientRepository>(_ => new ClientRepository(connectionString));
builder.Services.AddScoped<IModuleRepository>(_ => new ModuleRepository(connectionString));
builder.Services.AddScoped<IQuotationRepository>(_ => new QuotationRepository(connectionString));

builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<ModuleService>();
builder.Services.AddScoped<QuotationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();