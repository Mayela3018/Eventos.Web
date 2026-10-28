using Eventos.Web.Models;

namespace Eventos.Web.ViewModels;

public class InscripcionListadoViewModel
{
    public int Id { get; set; }
    public string Evento { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public string Asistente { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public string Estado { get; set; } = string.Empty;

    public bool PuedeCancelar => Estado == EstadoInscripcion.Confirmada;
}