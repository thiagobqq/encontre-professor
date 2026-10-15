namespace Backend.Models;
public record ParametrosBusca(
    string Idioma,
    IReadOnlyList<DiaDaSemana> Dias,
    TimeOnly InicioFaixa,
    TimeOnly FimFaixa,
    int DuracaoMinutos);
