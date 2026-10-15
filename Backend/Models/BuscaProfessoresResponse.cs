namespace Backend.Models;

public class BuscaProfessoresResponse
{
    public IReadOnlyList<ProfessorDisponivelResponse> Professores { get; init; } = [];

    public IReadOnlyList<ProfessorNaoElegivelResponse> NaoElegiveis { get; init; } = [];
}

public class ProfessorDisponivelResponse
{
    public int Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public int AulasNaSemana { get; init; }

    public IReadOnlyList<string> Horarios { get; init; } = [];
}

public class ProfessorNaoElegivelResponse
{
    public int Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string Motivo { get; init; } = string.Empty;
}
