namespace Backend.Models;

public class Professor
{
    public required int Id { get; init; }
    public required string Nome { get; init; }
    public required IReadOnlyList<string> Idiomas { get; init; }
    public required bool AgendaBloqueada { get; init; }
    public required IReadOnlyList<Disponibilidade> Disponibilidade { get; init; }
    public required IReadOnlyList<AulaMarcada> AulasMarcadas { get; init; }
}
