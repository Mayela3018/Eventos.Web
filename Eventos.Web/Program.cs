using System.Globalization;
using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Contexto de EF Core
builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventosDB")));

// Repositorios (inyección de dependencias)
builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Cultura Perú (S/) con punto decimal, para que los precios se lean y validen bien
var cultura = new CultureInfo("es-PE");
cultura.NumberFormat.NumberDecimalSeparator = ".";
cultura.NumberFormat.CurrencyDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new List<CultureInfo> { cultura },
    SupportedUICultures = new List<CultureInfo> { cultura }
});

// ================== DATOS INICIALES ==================
// Solo se ejecutan si las migraciones ya fueron aplicadas (Update-Database)
// y solo si cada tabla está vacía.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var pendientes = (await context.Database.GetPendingMigrationsAsync()).ToList();

    if (pendientes.Count > 0)
    {
        logger.LogWarning("Hay {Cantidad} migración(es) pendiente(s). Ejecuta Update-Database antes de cargar los datos iniciales.", pendientes.Count);
    }
    else
    {
        // 1) Categorías
        if (!await context.Categorias.AnyAsync())
        {
            context.Categorias.AddRange(
                new Categoria { Nombre = "Tecnología" },
                new Categoria { Nombre = "Música" },
                new Categoria { Nombre = "Deportes" }
            );
            await context.SaveChangesAsync();
        }

        // 2) Eventos
        if (!await context.Eventos.AnyAsync())
        {
            var ids = await context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Select(c => c.Id)
                .ToListAsync();

            var hoy = DateTime.Today;

            context.Eventos.AddRange(
                new Evento { Titulo = "Conferencia de Inteligencia Artificial", Fecha = hoy.AddDays(15).AddHours(18), Lugar = "Auditorio Principal", Aforo = 120, Precio = 0m, Modalidad = "Virtual", CategoriaId = ids[0] },
                new Evento { Titulo = "Taller de ASP.NET Core MVC", Fecha = hoy.AddDays(20).AddHours(15), Lugar = "Laboratorio 301", Aforo = 2, Precio = 35.50m, Modalidad = "Presencial", CategoriaId = ids[0] },
                new Evento { Titulo = "Concierto Rock Universitario", Fecha = hoy.AddDays(30).AddHours(20), Lugar = "Explanada Central", Aforo = 500, Precio = 25m, Modalidad = "Presencial", CategoriaId = ids[1 % ids.Count] },
                new Evento { Titulo = "Maratón 10K", Fecha = hoy.AddDays(-7).AddHours(7), Lugar = "Circuito de Playas", Aforo = 300, Precio = 40m, Modalidad = "Presencial", CategoriaId = ids[2 % ids.Count] }
            );
            await context.SaveChangesAsync();
        }

        // 3) Asistentes
        if (!await context.Asistentes.AnyAsync())
        {
            context.Asistentes.AddRange(
                new Asistente { Nombres = "Carlos Mendoza Ríos", Dni = "70123456", Correo = "carlos.mendoza@correo.com", Telefono = "987654321" },
                new Asistente { Nombres = "Lucía Fernández Paz", Dni = "71234567", Correo = "lucia.fernandez@correo.com" },
                new Asistente { Nombres = "Jorge Quispe Huamán", Dni = "72345678", Correo = "jorge.quispe@correo.com", Telefono = "912345678" },
                new Asistente { Nombres = "Andrea Salas Torres", Dni = "73456789", Correo = "andrea.salas@correo.com" },
                new Asistente { Nombres = "Diego Rojas Vargas", Dni = "74567890", Correo = "diego.rojas@correo.com" }
            );
            await context.SaveChangesAsync();
        }

        logger.LogInformation("Datos iniciales verificados.");
    }
}
// =====================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();