using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _asistenteRepositorio;

    public AsistentesController(AsistenteRepositorio asistenteRepositorio)
    {
        _asistenteRepositorio = asistenteRepositorio;
    }

    // GET: /Asistentes
    public async Task<IActionResult> Index()
    {
        var asistentes = await _asistenteRepositorio.ListarActivosAsync();
        return View(asistentes);
    }

    // GET: /Asistentes/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Asistente());
    }

    // POST: /Asistentes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombres,Dni,Correo,Telefono")] Asistente asistente)
    {
        if (!string.IsNullOrWhiteSpace(asistente.Dni)
            && await _asistenteRepositorio.ExisteDniAsync(asistente.Dni.Trim()))
        {
            ModelState.AddModelError(nameof(Asistente.Dni), "Ya existe un asistente registrado con ese DNI.");
        }

        if (!string.IsNullOrWhiteSpace(asistente.Correo)
            && await _asistenteRepositorio.ExisteCorreoAsync(asistente.Correo.Trim().ToLowerInvariant()))
        {
            ModelState.AddModelError(nameof(Asistente.Correo), "Ya existe un asistente registrado con ese correo.");
        }

        if (!ModelState.IsValid)
        {
            return View(asistente);
        }

        await _asistenteRepositorio.CrearAsync(asistente);
        TempData["Exito"] = $"El asistente \"{asistente.Nombres}\" se registró correctamente.";
        return RedirectToAction(nameof(Index));
    }
}