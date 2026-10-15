using System.Globalization;
using Backend.Models;
using Backend.Repositories;
using Backend.Services;

namespace Backend.Tests;

/// <summary>
/// Apoio para montar cenários de teste sem depender da API nem do
/// <c>professores.json</c> real: professores e parâmetros são construídos aqui e a
/// regra de negócio roda direto no <see cref="BuscaProfessoresService"/>.
/// </summary>
internal static class Cenario
{
    public const string NaoEnsinaIdioma = "Não ensina o idioma";
    public const string AgendaBloqueada = "Agenda bloqueada";
    public const string SemHorarioEmComum = "Sem horário em comum nos dias pedidos";

    public static DiaDaSemana Dia(string dia) =>
        DiaDaSemanaParser.TryParse(dia, out var valor)
            ? valor
            : throw new ArgumentException($"Dia da semana inválido no cenário: '{dia}'.");

    public static TimeOnly Hora(string hora) =>
        TimeOnly.ParseExact(hora, "HH:mm", CultureInfo.InvariantCulture);

    public static Disponibilidade Bloco(string dia, string inicio, string fim) =>
        new(Dia(dia), Hora(inicio), Hora(fim));

    public static AulaMarcada Aula(string dia, string inicio, int duracaoMinutos) =>
        new(Dia(dia), Hora(inicio), duracaoMinutos);

    public static Professor Prof(
        int id,
        string nome,
        string[] idiomas,
        bool agendaBloqueada = false,
        Disponibilidade[]? disponibilidade = null,
        AulaMarcada[]? aulasMarcadas = null) =>
        new()
        {
            Id = id,
            Nome = nome,
            Idiomas = idiomas,
            AgendaBloqueada = agendaBloqueada,
            Disponibilidade = disponibilidade ?? [],
            AulasMarcadas = aulasMarcadas ?? []
        };

    public static ParametrosBusca Parametros(
        string idioma,
        string[] dias,
        string inicio,
        string fim,
        int duracaoMinutos) =>
        new(idioma, dias.Select(Dia).ToList(), Hora(inicio), Hora(fim), duracaoMinutos);

    public static BuscaProfessoresService Servico(params Professor[] professores) =>
        new(new RepositorioEmMemoria(professores));

    public static BuscaProfessoresResponse Buscar(
        ParametrosBusca parametros,
        params Professor[] professores) =>
        Servico(professores).Buscar(parametros);

    public static ProfessorDisponivelResponse ProfessorElegivel(
        BuscaProfessoresResponse resposta,
        string nome) =>
        resposta.Professores.SingleOrDefault(p => p.Nome == nome)
        ?? throw new Xunit.Sdk.XunitException(
            $"Era esperado '{nome}' em professores, mas ele não está lá.");

    public static string? MotivoDe(BuscaProfessoresResponse resposta, string nome) =>
        resposta.NaoElegiveis.SingleOrDefault(p => p.Nome == nome)?.Motivo;

    public static IReadOnlyList<string> HorariosDe(BuscaProfessoresResponse resposta, string nome) =>
        ProfessorElegivel(resposta, nome).Horarios;

    public static bool EstaElegivel(BuscaProfessoresResponse resposta, string nome) =>
        resposta.Professores.Any(p => p.Nome == nome);

    private sealed class RepositorioEmMemoria : IProfessorRepository
    {
        private readonly IReadOnlyList<Professor> _professores;

        public RepositorioEmMemoria(params Professor[] professores) => _professores = professores;

        public IReadOnlyList<Professor> ObterTodos() => _professores;
    }
}
