using System.ComponentModel.DataAnnotations;

namespace Eventos.Web.ViewModels;

public class EventoListadoViewModel
{
    public int Id { get; set; }

    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime Fecha { get; set; }

    public string Lugar { get; set; } = string.Empty;

    public string Modalidad { get; set; } = string.Empty;

    public int Aforo { get; set; }

    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Precio { get; set; }

    [Display(Name = "Categoría")]
    public string Categoria { get; set; } = string.Empty;

    [Display(Name = "Inscritos confirmados")]
    public int Inscritos { get; set; }

    [Display(Name = "Cupos disponibles")]
    public int CuposDisponibles => Math.Max(0, Aforo - Inscritos);
}