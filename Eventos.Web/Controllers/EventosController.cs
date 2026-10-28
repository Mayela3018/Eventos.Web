using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Eventos.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController : Controller
{
    private readonly EventoRepositorio _eventoRepositorio;

    public EventosController(EventoRepositorio eventoRepositorio)
    {
        _eventoRepositorio = eventoRepositorio;
    }

    // GET: /Eventos?categoriaId=1
    public async Task<IActionResult> Index(int? categoriaId)
    {
        var categorias = await _eventoRepositorio.ListarCategoriasActivasAsync();

        var modelo = new EventosIndexViewModel
        {
            CategoriaId = categoriaId,
            Categorias = new SelectList(categorias, nameof(Categoria.Id), nameof(Categoria.Nombre), categoriaId),
            Eventos = await _eventoRepositorio.ListarActivosAsync(categoriaId)
        };

        return View(modelo);
    }

    // GET: /Eventos/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var evento = await _eventoRepositorio.ObtenerDetalleAsync(id);
        if (evento is null)
        {
            return NotFound();
        }

        return View(evento);
    }

    // GET: /Eventos/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarCategoriasAsync();
        return View(new Evento { Fecha = DateTime.Today.AddDays(7).AddHours(18) });
    }

    // POST: /Eventos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Titulo,Fecha,Lugar,Aforo,Precio,Modalidad,CategoriaId")] Evento evento)
    {
        await ValidarCategoriaAsync(evento.CategoriaId);

        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(evento.CategoriaId);
            return View(evento);
        }

        await _eventoRepositorio.CrearAsync(evento);
        TempData["Exito"] = $"El evento \"{evento.Titulo}\" se registró correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Eventos/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var evento = await _eventoRepositorio.ObtenerPorIdAsync(id);
        if (evento is null)
        {
            return NotFound();
        }

        await CargarCategoriasAsync(evento.CategoriaId);
        return View(evento);
    }

    // POST: /Eventos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Fecha,Lugar,Aforo,Precio,Modalidad,CategoriaId")] Evento evento)
    {
        if (id != evento.Id)
        {
            return NotFound();
        }

        await ValidarCategoriaAsync(evento.CategoriaId);

        var inscritos = await _eventoRepositorio.ContarConfirmadasAsync(id);
        if (evento.Aforo < inscritos)
        {
            ModelState.AddModelError(nameof(Evento.Aforo),
                $"El aforo no puede ser menor que los {inscritos} inscritos confirmados.");
        }

        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(evento.CategoriaId);
            return View(evento);
        }

        var actualizado = await _eventoRepositorio.ActualizarAsync(evento);
        if (!actualizado)
        {
            return NotFound();
        }

        TempData["Exito"] = $"El evento \"{evento.Titulo}\" se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Eventos/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var evento = await _eventoRepositorio.ObtenerDetalleAsync(id);
        if (evento is null)
        {
            return NotFound();
        }

        return View(evento);
    }

    // POST: /Eventos/Delete/5  (eliminación lógica)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eliminado = await _eventoRepositorio.EliminarAsync(id);
        if (!eliminado)
        {
            return NotFound();
        }

        TempData["Exito"] = "El evento se eliminó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarCategoriaAsync(int categoriaId)
    {
        if (categoriaId > 0 && !await _eventoRepositorio.CategoriaActivaExisteAsync(categoriaId))
        {
            ModelState.AddModelError(nameof(Evento.CategoriaId), "La categoría seleccionada no es válida.");
        }
    }

    private async Task CargarCategoriasAsync(int? seleccionada = null)
    {
        var categorias = await _eventoRepositorio.ListarCategoriasActivasAsync();
        ViewBag.Categorias = new SelectList(categorias, nameof(Categoria.Id), nameof(Categoria.Nombre), seleccionada);
    }
}