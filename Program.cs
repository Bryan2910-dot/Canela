using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Canela.Data;
using Canela.Service;
using Canela.Integration.Exchange;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Sesión (mejorada para el carrito)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo aumentado para el carrito
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "Canela.Carrito.Session";
});

// Configuración de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configuración de Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Servicios personalizados
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<ExchangeIntegration>();

// Configuración del HttpContext
builder.Services.AddHttpContextAccessor(); // Necesario para el carrito

// Configuración de MVC
builder.Services.AddControllersWithViews();

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1",
        Description = "Descripción de la API"
    });
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodos", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("PermitirTodos");
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Asegúrate de que esto esté después de UseRouting() y antes de Map endpoints

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();