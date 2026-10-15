namespace Backend.Models;

public class BuscaProfessoresRequest
{
    public string? Idioma { get; set; }

    public IReadOnlyList<string>? Dias { get; set; }
    public string? InicioFaixa { get; set; }

    public string? FimFaixa { get; set; }

    public int DuracaoMinutos { get; set; }
}
