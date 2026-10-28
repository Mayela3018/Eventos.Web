using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.ViewModels;

public class EventosIndexViewModel
{
    public int? CategoriaId { get; set; }

    public SelectList? Categorias { get; set; }

    public List<EventoListadoViewModel> Eventos { get; set; } = new();
}