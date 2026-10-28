using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class InscripcionRepositorio
{
    private readonly EventosContext _context;

    public InscripcionRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<InscripcionListadoViewModel>> ListarAsync()
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .OrderByDescending(i => i.FechaInscripcion)
            .Select(i => new InscripcionListadoViewModel
            {
                Id = i.Id,
                Evento = i.Evento!.Titulo,
                FechaEvento = i.Evento.Fecha,
                Asistente = i.Asistente!.Nombres,
                Dni = i.Asistente.Dni,
                FechaInscripcion = i.FechaInscripcion,
                Estado = i.Estado
            })
            .ToListAsync();
    }

    // Datos para las listas desplegables
    public async Task<List<Evento>> ListarEventosActivosAsync()
    {
        return await _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo)
            .OrderBy(e => e.Fecha)
            .ToListAsync();
    }

    public async Task<List<Asistente>> ListarAsistentesActivosAsync()
    {
        return await _context.Asistentes
            .AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();
    }

    public async Task<Evento?> ObtenerEventoActivoAsync(int eventoId)
    {
        return await _context.Eventos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventoId && e.Activo);
    }

    public async Task<bool> AsistenteActivoExisteAsync(int asistenteId)
    {
        return await _context.Asistentes
            .AsNoTracking()
            .AnyAsync(a => a.Id == asistenteId && a.Activo);
    }

    public async Task<bool> YaInscritoAsync(int eventoId, int asistenteId)
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .AnyAsync(i => i.EventoId == eventoId
                        && i.AsistenteId == asistenteId
                        && i.Estado == EstadoInscripcion.Confirmada);
    }

    public async Task<int> ContarConfirmadasAsync(int eventoId)
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .CountAsync(i => i.EventoId == eventoId && i.Estado == EstadoInscripcion.Confirmada);
    }

    public async Task CrearAsync(Inscripcion inscripcion)
    {
        inscripcion.Estado = EstadoInscripcion.Confirmada;
        inscripcion.FechaInscripcion = DateTime.Now;

        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();
    }

    // Cancelar = cambiar el Estado (no se elimina)
    public async Task<bool> CancelarAsync(int id)
    {
        var inscripcion = await _context.Inscripciones
            .FirstOrDefaultAsync(i => i.Id == id && i.Estado == EstadoInscripcion.Confirmada);

        if (inscripcion is null)
        {
            return false;
        }

        inscripcion.Estado = EstadoInscripcion.Cancelada;
        await _context.SaveChangesAsync();
        return true;
    }
}