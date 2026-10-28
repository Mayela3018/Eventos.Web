namespace Eventos.Web.Models;

/// <summary>
/// Valores permitidos para el estado de una inscripción (evita escribir "strings mágicos").
/// </summary>
public static class EstadoInscripcion
{
    public const string Confirmada = "Confirmada";
    public const string Cancelada = "Cancelada";
}