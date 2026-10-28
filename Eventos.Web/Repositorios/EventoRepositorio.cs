using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio
{
    private readonly EventosContext _context;

    public EventoRepositorio(EventosContext context)
    {
        _context = context;
    }

    // Eventos activos con su categoría, filtro opcional y orden por fecha
    public async Task<List<EventoListadoViewModel>> ListarActivosAsync(int? categoriaId)
    {
        var consulta = _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo);

        if (categoriaId.HasValue)
        {
            consulta = consulta.Where(e => e.CategoriaId == categoriaId.Value);
        }

        return await Proyectar(consulta.OrderBy(e => e.Fecha)).ToListAsync();
    }

    public async Task<EventoListadoViewModel?> ObtenerDetalleAsync(int id)
    {
        return await Proyectar(_context.Eventos
                .AsNoTracking()
                .Where(e => e.Id == id && e.Activo))
            .FirstOrDefaultAsync();
    }

    public async Task<Evento?> ObtenerPorIdAsync(int id)
    {
        return await _context.Eventos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.Activo);
    }

    public async Task<int> ContarConfirmadasAsync(int eventoId)
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .CountAsync(i => i.EventoId == eventoId && i.Estado == EstadoInscripcion.Confirmada);
    }

    public async Task CrearAsync(Evento evento)
    {
        evento.Activo = true;
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ActualizarAsync(Evento evento)
    {
        var existente = await _context.Eventos
            .FirstOrDefaultAsync(e => e.Id == evento.Id && e.Activo);

        if (existente is null)
        {
            return false;
        }

        existente.Titulo = evento.Titulo;
        existente.Fecha = evento.Fecha;
        existente.Lugar = evento.Lugar;
        existente.Aforo = evento.Aforo;
        existente.Precio = evento.Precio;
        existente.Modalidad = evento.Modalidad;
        existente.CategoriaId = evento.CategoriaId;

        await _context.SaveChangesAsync();
        return true;
    }

    // Eliminación lógica: Activo = false (nunca DELETE físico)
    public async Task<bool> EliminarAsync(int id)
    {
        var evento = await _context.Eventos
            .FirstOrDefaultAsync(e => e.Id == id && e.Activo);

        if (evento is null)
        {
            return false;
        }

        evento.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    // Datos para la lista desplegable de categorías
    public async Task<List<Categoria>> ListarCategoriasActivasAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<bool> CategoriaActivaExisteAsync(int categoriaId)
    {
        return await _context.Categorias
            .AsNoTracking()
            .AnyAsync(c => c.Id == categoriaId && c.Activo);
    }

    private static IQueryable<EventoListadoViewModel> Proyectar(IQueryable<Evento> consulta)
    {
        return consulta.Select(e => new EventoListadoViewModel
        {
            Id = e.Id,
            Titulo = e.Titulo,
            Fecha = e.Fecha,
            Lugar = e.Lugar,
            Modalidad = e.Modalidad,
            Aforo = e.Aforo,
            Precio = e.Precio,
            Categoria = e.Categoria!.Nombre,
            Inscritos = e.Inscripciones.Count(i => i.Estado == EstadoInscripcion.Confirmada)
        });
    }
}