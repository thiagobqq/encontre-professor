namespace Backend.Models;

public enum DiaDaSemana{ Segunda, Terca, Quarta, Quinta, Sexta, Sabado, Domingo}

public static class DiaDaSemanaParser
{
    private static readonly Dictionary<string, DiaDaSemana> Mapa = new(StringComparer.OrdinalIgnoreCase)
    {
        ["segunda"] = DiaDaSemana.Segunda,
        ["terca"] = DiaDaSemana.Terca,
        ["terça"] = DiaDaSemana.Terca,
        ["quarta"] = DiaDaSemana.Quarta,
        ["quinta"] = DiaDaSemana.Quinta,
        ["sexta"] = DiaDaSemana.Sexta,
        ["sabado"] = DiaDaSemana.Sabado,
        ["sábado"] = DiaDaSemana.Sabado,
        ["domingo"] = DiaDaSemana.Domingo
    };

    public static bool TryParse(string? valor, out DiaDaSemana dia)
    {
        return Mapa.TryGetValue(valor?.Trim() ?? string.Empty, out dia);
    }
}
