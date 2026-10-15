using Backend.Models;
using Backend.Services;

namespace Backend.Tests;

/// <summary>
/// Seção A dos casos de teste: entrada inválida deve virar 400 com mensagem clara.
/// Aqui exercitamos o <see cref="ValidadorBusca"/> diretamente (é ele que o
/// Controller usa para decidir entre 200 e 400), sem subir a API.
/// </summary>
public class ValidadorBuscaTests
{
    private static readonly ValidadorBusca Validador = new();

    private static ResultadoValidacao Validar(
        string? idioma = "Espanhol",
        IReadOnlyList<string>? dias = null,
        string? inicio = "18:00",
        string? fim = "20:00",
        int duracao = 60) =>
        Validador.Validar(new BuscaProfessoresRequest
        {
            Idioma = idioma,
            Dias = dias ?? new[] { "segunda" },
            InicioFaixa = inicio,
            FimFaixa = fim,
            DuracaoMinutos = duracao
        });

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void A1_idioma_vazio_ou_ausente_e_rejeitado(string? idioma)
    {
        var resultado = Validar(idioma: idioma);

        Assert.False(resultado.Valido);
        Assert.Contains("idioma", resultado.Erro!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void A2_nenhum_dia_informado_e_rejeitado()
    {
        // O atalho Validar() preenche os dias por padrão, então o caso nulo é
        // construído na mão.
        var vazio = Validar(dias: Array.Empty<string>());
        var nulo = Validador.Validar(new BuscaProfessoresRequest
        {
            Idioma = "Espanhol",
            Dias = null,
            InicioFaixa = "18:00",
            FimFaixa = "20:00",
            DuracaoMinutos = 60
        });

        Assert.False(vazio.Valido);
        Assert.False(nulo.Valido);
        Assert.Contains("dia", vazio.Erro!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void A3_dia_invalido_e_rejeitado_e_a_mensagem_cita_o_dia()
    {
        var resultado = Validar(dias: new[] { "segunda", "feriado" });

        Assert.False(resultado.Valido);
        Assert.Contains("feriado", resultado.Erro!);
    }

    [Theory]
    [InlineData("8:00")]
    [InlineData("18h")]
    public void A4_horario_fora_do_formato_HH_mm_e_rejeitado(string inicio)
    {
        Assert.False(Validar(inicio: inicio).Valido);
    }

    [Fact]
    public void A5_fim_igual_ou_anterior_ao_inicio_e_rejeitado()
    {
        Assert.False(Validar(inicio: "18:00", fim: "18:00").Valido);
        Assert.False(Validar(inicio: "20:00", fim: "18:00").Valido);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(120)]
    public void A6_duracao_fora_de_30_60_90_e_rejeitada(int duracao)
    {
        Assert.False(Validar(duracao: duracao).Valido);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(90)]
    public void A7_duracoes_30_60_90_sao_aceitas(int duracao)
    {
        var resultado = Validar(duracao: duracao);

        Assert.True(resultado.Valido);
        Assert.Equal(duracao, resultado.Parametros!.DuracaoMinutos);
    }

    [Fact]
    public void A8_dia_acentuado_maiusculo_e_repetido_sao_normalizados()
    {
        var acentuado = Validar(dias: new[] { "terça" });
        var maiusculo = Validar(dias: new[] { "Segunda" });
        var repetido = Validar(dias: new[] { "segunda", "segunda" });

        Assert.Equal(DiaDaSemana.Terca, Assert.Single(acentuado.Parametros!.Dias));
        Assert.Equal(DiaDaSemana.Segunda, Assert.Single(maiusculo.Parametros!.Dias));
        Assert.Equal(DiaDaSemana.Segunda, Assert.Single(repetido.Parametros!.Dias));
    }

    [Fact]
    public void A9_inicio_da_faixa_fora_da_grade_e_400()
    {
        // Decisão do projeto: o início da faixa precisa estar em :00/:30;
        // fora disso, 400 — e não arredondar para 18:30.
        var resultado = Validar(inicio: "18:15");

        Assert.False(resultado.Valido);
        Assert.Contains("meia hora", resultado.Erro!);
    }

    [Fact]
    public void A10_requisicao_totalmente_valida_e_aceita()
    {
        var resultado = Validar();

        Assert.True(resultado.Valido);
        Assert.NotNull(resultado.Parametros);
        Assert.Null(resultado.Erro);
    }
}
