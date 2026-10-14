using Backend.Models;
using Backend.Repositories;

namespace Backend.Tests;

public class JsonProfessorRepositoryTests
{
    private static JsonProfessorRepository CriarRepositorio() =>
        new(Path.Combine(AppContext.BaseDirectory, "professores.json"));

    [Fact]
    public void Carrega_todos_os_professores_do_arquivo()
    {
        var repositorio = CriarRepositorio();

        var professores = repositorio.ObterTodos();

        Assert.Equal(8, professores.Count);
    }

    [Fact]
    public void Mapeia_dados_da_Ana_para_o_dominio()
    {
        var repositorio = CriarRepositorio();

        var ana = repositorio.ObterTodos().Single(p => p.Nome == "Ana");

        Assert.Equal(1, ana.Id);
        Assert.False(ana.AgendaBloqueada);
        Assert.Equal(new[] { "Inglês", "Espanhol" }, ana.Idiomas);
        Assert.Equal(
            new Disponibilidade(DiaDaSemana.Segunda, new TimeOnly(18, 0), new TimeOnly(22, 0)),
            ana.Disponibilidade[0]);
        Assert.Equal(
            new AulaMarcada(DiaDaSemana.Segunda, new TimeOnly(19, 0), 60),
            ana.AulasMarcadas[0]);
    }

    [Fact]
    public void Carrega_professor_com_agenda_bloqueada()
    {
        var repositorio = CriarRepositorio();

        var carla = repositorio.ObterTodos().Single(p => p.Nome == "Carla");

        Assert.True(carla.AgendaBloqueada);
    }
}
