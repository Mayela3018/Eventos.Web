using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Eventos.Web.Models;

public class Inscripcion
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Seleccione un evento.")]
    [Display(Name = "Evento")]
    public int EventoId { get; set; }

    [ForeignKey(nameof(EventoId))]
    [ValidateNever]
    public Evento? Evento { get; set; }

    [Required(ErrorMessage = "Seleccione un asistente.")]
    [Display(Name = "Asistente")]
    public int AsistenteId { get; set; }

    [ForeignKey(nameof(AsistenteId))]
    [ValidateNever]
    public Asistente? Asistente { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Fecha de inscripción")]
    public DateTime FechaInscripcion { get; set; } = DateTime.Now;

    [Required]
    [StringLength(20)]
    [RegularExpression("^(Confirmada|Cancelada)$", ErrorMessage = "El estado debe ser Confirmada o Cancelada.")]
    public string Estado { get; set; } = EstadoInscripcion.Confirmada;
}