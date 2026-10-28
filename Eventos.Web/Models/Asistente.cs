using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Models;

[Index(nameof(Dni), IsUnique = true)]
[Index(nameof(Correo), IsUnique = true)]
public class Asistente
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, ErrorMessage = "Los nombres no pueden superar los 100 caracteres.")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos numéricos.")]
    [Display(Name = "DNI")]
    public string Dni { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
    public string Correo { get; set; } = string.Empty;

    // Migración 3: AgregarTelefonoAsistente (opcional, máx. 9)
    [StringLength(9, ErrorMessage = "El teléfono no puede superar los 9 caracteres.")]
    [RegularExpression(@"^\d{6,9}$", ErrorMessage = "El teléfono solo debe contener dígitos (de 6 a 9).")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    public bool Activo { get; set; } = true;

    [ValidateNever]
    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}