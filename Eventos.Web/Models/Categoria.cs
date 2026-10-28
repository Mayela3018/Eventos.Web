using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Eventos.Web.Models;

[Index(nameof(Nombre), IsUnique = true)]
public class Categoria
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
    [Display(Name = "Categoría")]
    public string Nombre { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    // Navegación: una categoría tiene muchos eventos
    [ValidateNever]
    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}