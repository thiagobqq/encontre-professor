using System.Globalization;
using Backend.Models;
using Backend.Repositories;

namespace Backend.Services;

public class BuscaProfessoresService : IBuscaProfessoresService
{
    private const string FormatoHora = "HH:mm";

    private const int MinutosDaGrade = 30;

    private const string MotivoNaoEnsinaIdioma = "Não ensina o idioma";
    private const string MotivoAgendaBloqueada = "Agenda bloqueada";
    private const string MotivoSemHorarioEmComum = "Sem horário em comum nos dias pedidos";

    private readonly IProfessorRepository _repository;

    public BuscaProfessoresService(IProfessorRepository repository)
    {
        _repository = repository;
    }

    public BuscaProfessoresResponse Buscar(ParametrosBusca parametros)
    {
        var professores = new List<ProfessorDisponivelResponse>();
        var naoElegiveis = new List<ProfessorNaoElegivelResponse>();

        foreach (var professor in _repository.ObterTodos())
        {
            if (!EnsinaIdioma(professor, parametros.Idioma))
            {
                naoElegiveis.Add(NaoElegivel(professor, MotivoNaoEnsinaIdioma));
                continue;
            }

            if (professor.AgendaBloqueada)
            {
                naoElegiveis.Add(NaoElegivel(professor, MotivoAgendaBloqueada));
                continue;
            }

            var horarios = ObterHorariosValidos(professor, parametros);
            if (horarios.Count == 0)
            {
                naoElegiveis.Add(NaoElegivel(professor, MotivoSemHorarioEmComum));
                continue;
            }

            professores.Add(new ProfessorDisponivelResponse
            {
                Id = professor.Id,
                Nome = professor.Nome,
                AulasNaSemana = professor.AulasMarcadas.Count,
                Horarios = horarios
                    .Select(hora => hora.ToString(FormatoHora, CultureInfo.InvariantCulture))
                    .ToList()
            });
        }

        return new BuscaProfessoresResponse
        {
            Professores = professores
                .OrderBy(p => p.AulasNaSemana)
                .ThenBy(p => p.Nome, StringComparer.InvariantCulture)
                .ToList(),
            NaoElegiveis = naoElegiveis
                .OrderBy(p => p.Nome, StringComparer.InvariantCulture)
                .ToList()
        };
    }

    private static List<TimeOnly> ObterHorariosValidos(Professor professor, ParametrosBusca parametros)
    {
        var horarios = new List<TimeOnly>();

        var inicioFaixa = EmMinutos(parametros.InicioFaixa);
        var fimFaixa = EmMinutos(parametros.FimFaixa);

        for (var inicio = inicioFaixa;
             inicio + parametros.DuracaoMinutos <= fimFaixa;
             inicio += MinutosDaGrade)
        {
            if (parametros.Dias.All(dia => AulaCabe(professor, dia, inicio, parametros.DuracaoMinutos)))
                horarios.Add(TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(inicio)));
        }

        return horarios;
    }

    private static bool AulaCabe(Professor professor, DiaDaSemana dia, int inicio, int duracaoMinutos)
    {
        var fim = inicio + duracaoMinutos;

        // Cabe inteira dentro de um bloco do dia — terminar exatamente no fim vale.
        var cabeEmBloco = professor.Disponibilidade
            .Where(bloco => bloco.Dia == dia)
            .Any(bloco => EmMinutos(bloco.Inicio) <= inicio && fim <= EmMinutos(bloco.Fim));

        if (!cabeEmBloco)
            return false;

        // Intervalos meio-abertos: encostar é permitido, sobrepor não.
        var sobrepoeAula = professor.AulasMarcadas
            .Where(aula => aula.Dia == dia)
            .Any(aula => inicio < EmMinutos(aula.Inicio) + aula.DuracaoMinutos
                         && EmMinutos(aula.Inicio) < fim);

        return !sobrepoeAula;
    }

    private static bool EnsinaIdioma(Professor professor, string idioma) =>
        professor.Idiomas.Contains(idioma, StringComparer.OrdinalIgnoreCase);

    private static int EmMinutos(TimeOnly hora) => hora.Hour * 60 + hora.Minute;

    private static ProfessorNaoElegivelResponse NaoElegivel(Professor professor, string motivo) =>
        new()
        {
            Id = professor.Id,
            Nome = professor.Nome,
            Motivo = motivo
        };
}
