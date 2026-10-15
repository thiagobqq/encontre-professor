using System.Globalization;
using Backend.Models;

namespace Backend.Services;
public class ValidadorBusca
{
    private const string FormatoHora = "HH:mm";

    private const int MinutosDaGrade = 30;

    private static readonly int[] DuracoesPermitidas = [30, 60, 90];

    public ResultadoValidacao Validar(BuscaProfessoresRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Idioma))
            return ResultadoValidacao.Falha("Informe o idioma.");

        if (request.Dias is null || request.Dias.Count == 0)
            return ResultadoValidacao.Falha("Informe ao menos um dia da semana.");

        var dias = new List<DiaDaSemana>();
        foreach (var dia in request.Dias)
        {
            if (!DiaDaSemanaParser.TryParse(dia, out var diaValido))
                return ResultadoValidacao.Falha($"Dia da semana inválido: '{Descrever(dia)}'.");

            if (!dias.Contains(diaValido))
                dias.Add(diaValido);
        }

        if (!TentarParseHora(request.InicioFaixa, out var inicioFaixa))
            return ResultadoValidacao.Falha($"Horário inválido: '{Descrever(request.InicioFaixa)}'. Use o formato HH:mm.");

        if (!TentarParseHora(request.FimFaixa, out var fimFaixa))
            return ResultadoValidacao.Falha($"Horário inválido: '{Descrever(request.FimFaixa)}'. Use o formato HH:mm.");

   
        if (!EstaNaGrade(inicioFaixa))
            return ResultadoValidacao.Falha("O início da faixa deve ser em hora cheia ou meia hora (:00 ou :30).");

        if (fimFaixa <= inicioFaixa)
            return ResultadoValidacao.Falha("O fim da faixa deve ser posterior ao início.");

        if (!DuracoesPermitidas.Contains(request.DuracaoMinutos))
            return ResultadoValidacao.Falha("A duração deve ser 30, 60 ou 90 minutos.");

        return ResultadoValidacao.Ok(new ParametrosBusca(
            request.Idioma.Trim(),
            dias,
            inicioFaixa,
            fimFaixa,
            request.DuracaoMinutos));
    }

    private static bool TentarParseHora(string? valor, out TimeOnly hora) =>
        TimeOnly.TryParseExact(
            valor?.Trim(),
            FormatoHora,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out hora);

    private static bool EstaNaGrade(TimeOnly hora) => hora.Minute % MinutosDaGrade == 0;

    private static string Descrever(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? "(vazio)" : valor.Trim();
}

public record ResultadoValidacao(ParametrosBusca? Parametros, string? Erro)
{
    public bool Valido => Erro is null;

    public static ResultadoValidacao Ok(ParametrosBusca parametros) => new(parametros, null);

    public static ResultadoValidacao Falha(string erro) => new(null, erro);
}