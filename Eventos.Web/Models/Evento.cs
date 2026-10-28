using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Eventos.Web.Models;

public class Evento
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El lugar es obligatorio.")]
    [StringLength(100, ErrorMessage = "El lugar no puede superar los 100 caracteres.")]
    public string Lugar { get; set; } = string.Empty;

    [Range(1, 5000, ErrorMessage = "El aforo debe estar entre 1 y 5000.")]
    public int Aforo { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0", "99999999", ErrorMessage = "El precio no puede ser negativo.")]
    [DataType(DataType.Currency)]
    public decimal Precio { get; set; }

    // Migración 2: AgregarModalidadEvento
    [Required(ErrorMessage = "La modalidad es obligatoria.")]
    [StringLength(20, ErrorMessage = "La modalidad no puede superar los 20 caracteres.")]
    [RegularExpression("^(Presencial|Virtual)$", ErrorMessage = "La modalidad debe ser Presencial o Virtual.")]
    public string Modalidad { get; set; } = "Presencial";

    [Required(ErrorMessage = "Seleccione una categoría.")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    [ValidateNever]
    public Categoria? Categoria { get; set; }

    public bool Activo { get; set; } = true;

    // Navegación: un evento tiene muchas inscripciones
    [ValidateNever]
    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}