using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _inscripcionRepositorio;

    public InscripcionesController(InscripcionRepositorio inscripcionRepositorio)
    {
        _inscripcionRepositorio = inscripcionRepositorio;
    }

    // GET: /Inscripciones
    public async Task<IActionResult> Index()
    {
        var inscripciones = await _inscripcionRepositorio.ListarAsync();
        return View(inscripciones);
    }

    // GET: /Inscripciones/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Inscripcion());
    }

    // POST: /Inscripciones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventoId,AsistenteId")] Inscripcion inscripcion)
    {
        if (ModelState.IsValid)
        {
            var evento = await _inscripcionRepositorio.ObtenerEventoActivoAsync(inscripcion.EventoId);

            if (evento is null)
            {
                ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento seleccionado no existe o no está activo.");
            }
            else
            {
                // 1) El evento no debe haber pasado
                if (evento.Fecha < DateTime.Now)
                {
                    ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento ya pasó; no se permiten nuevas inscripciones.");
                }

                // 2) El aforo no debe estar completo
                var confirmadas = await _inscripcionRepositorio.ContarConfirmadasAsync(evento.Id);
                if (confirmadas >= evento.Aforo)
                {
                    ModelState.AddModelError(nameof(Inscripcion.EventoId), $"El aforo del evento está completo ({confirmadas}/{evento.Aforo}).");
                }
            }

            // 3) El asistente debe existir y no estar ya inscrito (Confirmada)
            if (!await _inscripcionRepositorio.AsistenteActivoExisteAsync(inscripcion.AsistenteId))
            {
                ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente seleccionado no existe o no está activo.");
            }
            else if (await _inscripcionRepositorio.YaInscritoAsync(inscripcion.EventoId, inscripcion.AsistenteId))
            {
                ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente ya está inscrito (Confirmada) en este evento.");
            }
        }

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(inscripcion.EventoId, inscripcion.AsistenteId);
            return View(inscripcion);
        }

        await _inscripcionRepositorio.CrearAsync(inscripcion);
        TempData["Exito"] = "La inscripción se registró correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Inscripciones/Cancelar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var cancelada = await _inscripcionRepositorio.CancelarAsync(id);

        if (cancelada)
        {
            TempData["Exito"] = "La inscripción fue cancelada.";
        }
        else
        {
            TempData["Error"] = "No se pudo cancelar: la inscripción no existe o ya estaba cancelada.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(int? eventoId = null, int? asistenteId = null)
    {
        var eventos = await _inscripcionRepositorio.ListarEventosActivosAsync();
        var asistentes = await _inscripcionRepositorio.ListarAsistentesActivosAsync();

        ViewBag.Eventos = new SelectList(
            eventos.Select(e => new { e.Id, Texto = $"{e.Titulo} - {e.Fecha:dd/MM/yyyy HH:mm}" }),
            "Id", "Texto", eventoId);

        ViewBag.Asistentes = new SelectList(
            asistentes.Select(a => new { a.Id, Texto = $"{a.Nombres} - DNI {a.Dni}" }),
            "Id", "Texto", asistenteId);
    }
}