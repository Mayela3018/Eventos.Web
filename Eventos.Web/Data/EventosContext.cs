using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Data;

// Configuración solo con Data Annotations: NO se sobrescribe OnModelCreating (sin Fluent API).
public class EventosContext : DbContext
{
    public EventosContext(DbContextOptions<EventosContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Asistente> Asistentes => Set<Asistente>();
    public DbSet<Inscripcion> Inscripciones => Set<Inscripcion>();
}