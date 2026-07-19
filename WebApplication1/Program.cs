using Entidades;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Servicios;
using WebApplication1.Hubs;

var builder = WebApplication.CreateBuilder(args);

// services SIEMPRE antes de Build
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddDbContext<MusicTradeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servicio de hashing de contraseñas (login propio, sin Identity)
builder.Services.AddScoped<PasswordService>();

builder.Services.AddScoped<IPublicacionService, PublicacionService>();
builder.Services.AddScoped<IConversacionService, ConversacionService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Autenticación basada en cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "MusicTrade.Auth";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Migrar la base al arrancar, usando el DbContext ya configurado por DI
// (con el connection string que vino de appsettings.json, no hardcodeado)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MusicTradeDbContext>();
    db.Database.Migrate();
}

// pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapHub<OfertaHub>("/hubs/oferta");
app.MapHub<ChatHub>("/hubs/chat");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
