using Dashboard_Sales_Dev.DAL.Repositories;
using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Error");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing from the configuration.");

builder.Services.AddScoped<IClientRepository>(_ => new ClientRepository(connectionString));
builder.Services.AddScoped<IModuleRepository>(_ => new ModuleRepository(connectionString));
builder.Services.AddScoped<IDealRepository>(_ => new DealRepository(connectionString));
builder.Services.AddScoped<IEmployeeRepository>(_ => new EmployeeRepository(connectionString));
builder.Services.AddScoped<ICommentRepository>(_ => new CommentRepository(connectionString));
builder.Services.AddScoped<IStatusRepository>(_ => new StatusRepository(connectionString));
builder.Services.AddScoped<ISavedFilterRepository>(_ => new SavedFilterRepository(connectionString));

builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<ModuleService>();
builder.Services.AddScoped<DealService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<SavedFilterService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();