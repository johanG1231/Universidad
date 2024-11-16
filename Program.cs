using _123.Models;
using _123.Repositories;
using _123.Services;
using _123.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// **Configurar Logging**
builder.Logging.ClearProviders(); // Eliminar los proveedores de logging por defecto
builder.Logging.AddConsole(); // Añadir logging a la consola

// Agregar servicios al contenedor.
builder.Services.AddControllersWithViews();

// Configurar DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Añadir sesión
builder.Services.AddSession();

// Inyectar dependencias
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICompraService, CompraService>();

// Configuración de autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


// Configuración de políticas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Administrador"));
});

// Configurar otras opciones necesarias

var app = builder.Build();

// Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/Error"); // Redirige a la vista de error
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


// **Habilitar la autenticación y autorización**
app.UseAuthentication(); // Agregar autenticación
app.UseAuthorization(); // Agregar autorización

// **Habilitar el middleware de sesión**
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=Inicio}/{id?}");

app.Run();
