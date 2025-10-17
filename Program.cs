using inmobiliaria.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Pomelo.EntityFrameworkCore.MySql;

var builder = WebApplication.CreateBuilder(args);

// --- Configuración de la base de datos ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<inmobiliaria.Models.DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// --- Controladores y vistas ---
builder.Services.AddControllersWithViews();

// --- Autenticación por Cookies 
builder.Services.AddAuthentication(options =>
{
    // Configuración combinada de esquemas
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Usuario/Login";
    options.LogoutPath = "/Usuario/Logout";
    options.AccessDeniedPath = "/Home";
})
// Autenticación por JWT
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
});

// --- Repositorios y servicios ---
builder.Services.AddScoped<inmobiliaria.Models.RepositorioPropietario>();
builder.Services.AddScoped<inmobiliaria.Models.RepositorioInmueble>();
builder.Services.AddScoped<inmobiliaria.Models.RepositorioInquilino>();
builder.Services.AddScoped<inmobiliaria.Models.RepositorioContrato>();
builder.Services.AddScoped<inmobiliaria.Models.RepositorioPago>();
builder.Services.AddScoped<inmobiliaria.Models.RepositorioUsuario>();
builder.Services.AddScoped<inmobiliaria.Models.RepositorioTipoInmueble>();
builder.Services.AddScoped<inmobiliaria.Models.RepositorioAuditoria>();
builder.Services.AddScoped<AuditoriaHelper>();
builder.Services.AddScoped<inmobiliaria.lib.HashPasswordService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// MVC tradicional
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();

app.Run();
