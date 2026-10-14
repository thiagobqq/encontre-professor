using System.Globalization;
using System.Text.Json;
using Backend.Models;

namespace Backend.Repositories;

public class JsonProfessorRepository : IProfessorRepository
{
    private const string FormatoHora = "HH:mm";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IReadOnlyList<Professor> _professores;

    public JsonProfessorRepository(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo))
            throw new FileNotFoundException($"Arquivo de professores não encontrado: {caminhoArquivo}", caminhoArquivo);

        var json = File.ReadAllText(caminhoArquivo);
        var arquivo = JsonSerializer.Deserialize<ArquivoProfessores>(json, JsonOptions)
            ?? throw new InvalidDataException($"Arquivo de professores inválido: {caminhoArquivo}");

        _professores = arquivo.Professores.Select(Mapear).ToList();
    }

    public IReadOnlyList<Professor> ObterTodos() => _professores;

    private static Professor Mapear(ProfessorJson p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Idiomas = p.Idiomas,
        AgendaBloqueada = p.AgendaBloqueada,
        Disponibilidade = p.Disponibilidade
            .Select(d => new Disponibilidade(ParseDia(d.Dia), ParseHora(d.Inicio), ParseHora(d.Fim)))
            .ToList(),
        AulasMarcadas = p.AulasMarcadas
            .Select(a => new AulaMarcada(ParseDia(a.Dia), ParseHora(a.Inicio), a.DuracaoMinutos))
            .ToList()
    };

    private static DiaDaSemana ParseDia(string dia) =>
        DiaDaSemanaParser.TryParse(dia, out var parsed)
            ? parsed
            : throw new InvalidDataException($"Dia da semana inválido no arquivo de professores: '{dia}'.");

    private static TimeOnly ParseHora(string hora) =>
        TimeOnly.ParseExact(hora, FormatoHora, CultureInfo.InvariantCulture);
    private record ArquivoProfessores(List<ProfessorJson> Professores);

    private record ProfessorJson(
        int Id,
        string Nome,
        List<string> Idiomas,
        bool AgendaBloqueada,
        List<DisponibilidadeJson> Disponibilidade,
        List<AulaMarcadaJson> AulasMarcadas);

    private record DisponibilidadeJson(string Dia, string Inicio, string Fim);

    private record AulaMarcadaJson(string Dia, string Inicio, int DuracaoMinutos);
}
