using Backend.Models;
using Backend.Repositories;

namespace Backend.Tests;

public class JsonProfessorRepositoryTests
{
    private static JsonProfessorRepository CriarRepositorio() =>
        new(Path.Combine(AppContext.BaseDirectory, "professores.json"));

    [Fact]
    public void Carrega_o_json_e_mapeia_todos_os_professores_para_o_dominio()
    {
        var professores = CriarRepositorio().ObterTodos();

        Assert.Equal(8, professores.Count);

        var ana = professores.Single(p => p.Nome == "Ana");
        Assert.Equal(1, ana.Id);
        Assert.False(ana.AgendaBloqueada);
        Assert.Equal(new[] { "Inglês", "Espanhol" }, ana.Idiomas);
        Assert.Equal(
            new Disponibilidade(DiaDaSemana.Segunda, new TimeOnly(18, 0), new TimeOnly(22, 0)),
            ana.Disponibilidade[0]);
        Assert.Equal(
            new AulaMarcada(DiaDaSemana.Segunda, new TimeOnly(19, 0), 60),
            ana.AulasMarcadas[0]);

        Assert.True(professores.Single(p => p.Nome == "Carla").AgendaBloqueada);
    }
}
