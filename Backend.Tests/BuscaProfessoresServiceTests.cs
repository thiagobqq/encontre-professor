using Backend.Models;

namespace Backend.Tests;

/// <summary>
/// Casos de teste da regra de busca (seções B a K). Todos exercitam o
/// <see cref="Backend.Services.BuscaProfessoresService"/> diretamente, com um
/// repositório em memória, sem subir a API.
/// </summary>
public class BuscaProfessoresServiceTests
{
    private const string NaoEnsina = Cenario.NaoEnsinaIdioma;
    private const string Bloqueada = Cenario.AgendaBloqueada;
    private const string SemHorario = Cenario.SemHorarioEmComum;

    private static Professor AnaSegunda(string inicio = "14:00", string fim = "22:00", AulaMarcada[]? aulas = null) =>
        Cenario.Prof(1, "Ana", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", inicio, fim) },
            aulasMarcadas: aulas);

    private static Professor AnaComAulaDePoMeia() =>
        AnaSegunda(aulas: new[] { Cenario.Aula("segunda", "19:00", 60) });

    // =====================================================================
    // B. Regra 1 — Idioma
    // =====================================================================

    [Fact]
    public void B1_so_quem_ensina_o_idioma_aparece_e_os_demais_recebem_o_motivo()
    {
        var ana = Cenario.Prof(1, "Ana", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "14:00", "22:00") });
        var bruno = Cenario.Prof(2, "Bruno", new[] { "Inglês" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "14:00", "22:00") });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "18:00", "20:00", 60), ana, bruno);

        Assert.Equal(new[] { "Ana" }, resposta.Professores.Select(p => p.Nome));
        Assert.Equal(NaoEnsina, Cenario.MotivoDe(resposta, "Bruno"));
    }

    [Fact]
    public void B2_idioma_e_comparado_sem_diferenciar_caixa()
    {
        var ana = Cenario.Prof(1, "Ana", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("espanhol", new[] { "segunda" }, "18:00", "20:00", 60), ana);

        Assert.Equal(new[] { "Ana" }, resposta.Professores.Select(p => p.Nome));
    }

    // =====================================================================
    // C. Regra 2 — Agenda bloqueada
    // =====================================================================

    [Fact]
    public void C1_professor_com_agenda_bloqueada_fica_fora_com_motivo()
    {
        var carla = Cenario.Prof(1, "Carla", new[] { "Espanhol" }, agendaBloqueada: true,
            disponibilidade: new[] { Cenario.Bloco("segunda", "14:00", "22:00") });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "18:00", "20:00", 60), carla);

        Assert.Empty(resposta.Professores);
        Assert.Equal(Bloqueada, Cenario.MotivoDe(resposta, "Carla"));
    }

    [Fact]
    public void C2_nao_ensina_tem_prioridade_sobre_agenda_bloqueada()
    {
        var bruno = Cenario.Prof(1, "Bruno", new[] { "Inglês" }, agendaBloqueada: true,
            disponibilidade: new[] { Cenario.Bloco("segunda", "14:00", "22:00") });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "18:00", "20:00", 60), bruno);

        Assert.Equal(NaoEnsina, Cenario.MotivoDe(resposta, "Bruno"));
    }

    // =====================================================================
    // D. Regra 4 — A aula cabe inteira no bloco e na faixa
    // =====================================================================

    public static TheoryData<string, string, int, string[]> CasosD => new()
    {
        { "18:00", "20:00", 60, new[] { "18:00", "18:30", "19:00" } }, // cabe com folga
        { "18:30", "19:30", 60, new[] { "18:30" } },                  // encosta nos dois limites
        { "18:00", "18:45", 60, Array.Empty<string>() },              // não cabe na faixa
    };

    [Theory]
    [MemberData(nameof(CasosD))]
    public void D1_a_aula_precisa_caber_no_bloco_e_na_faixa(string inicio, string fim, int duracao, string[] esperado)
    {
        var ana = Cenario.Prof(1, "Ana", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "20:00") });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, inicio, fim, duracao), ana);

        if (esperado.Length == 0)
        {
            Assert.False(Cenario.EstaElegivel(resposta, "Ana"));
            Assert.Equal(SemHorario, Cenario.MotivoDe(resposta, "Ana"));
        }
        else
        {
            Assert.Equal(esperado, Cenario.HorariosDe(resposta, "Ana"));
        }
    }

    [Fact]
    public void D2_dois_blocos_no_mesmo_dia_somam_candidatos()
    {
        var ana = Cenario.Prof(1, "Ana", new[] { "Espanhol" },
            disponibilidade: new[]
            {
                Cenario.Bloco("segunda", "08:00", "12:00"),
                Cenario.Bloco("segunda", "14:00", "18:00")
            });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "11:00", "15:00", 60), ana);

        // A aula não atravessa o intervalo entre os blocos: 11:00 (fim do
        // primeiro) e 14:00 (início do segundo) são válidos, 12:00/13:00 não.
        Assert.Equal(new[] { "11:00", "14:00" }, Cenario.HorariosDe(resposta, "Ana"));
    }

    // =====================================================================
    // E. Regra 5 — Aulas já marcadas (encostar é permitido)
    // =====================================================================

    [Fact]
    public void E1_aula_marcada_permite_encostar_mas_bloqueia_sobrepor()
    {
        // Aula marcada 19:00–20:00: 18:00 encosta no início? Não — 18:00–19:00
        // apenas encosta às 19:00; 20:00 começa quando ela termina.
        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "18:00", "21:00", 60),
            AnaComAulaDePoMeia());

        Assert.Equal(new[] { "18:00", "20:00" }, Cenario.HorariosDe(resposta, "Ana"));
    }

    [Fact]
    public void E2_aula_marcada_em_outro_dia_nao_interfere()
    {
        var ana = AnaSegunda(aulas: new[] { Cenario.Aula("terca", "19:00", 60) });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "18:00", "21:00", 60), ana);

        Assert.Contains("19:00", Cenario.HorariosDe(resposta, "Ana"));
    }

    // =====================================================================
    // F. Regra 6 — Início só em :00 ou :30
    // =====================================================================

    [Fact]
    public void F1_bloco_fora_da_grade_o_primeiro_inicio_e_a_proxima_meia_hora()
    {
        var fabio = Cenario.Prof(1, "Fábio", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:15", "20:00") });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "18:00", "20:00", 60), fabio);

        Assert.Equal(new[] { "18:30", "19:00" }, Cenario.HorariosDe(resposta, "Fábio"));
    }

    // =====================================================================
    // G. Regra 3 — Horário fixo em todos os dias (interseção, nunca união)
    // =====================================================================

    [Fact]
    public void G1_a_resposta_e_a_intersecao_entre_os_dias_e_nunca_a_uniao()
    {
        var ana = Cenario.Prof(1, "Ana", new[] { "Espanhol" },
            disponibilidade: new[]
            {
                Cenario.Bloco("segunda", "18:00", "20:00"),
                Cenario.Bloco("quarta", "19:00", "21:00")
            });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda", "quarta" }, "18:00", "21:00", 60), ana);

        Assert.Equal(new[] { "19:00" }, Cenario.HorariosDe(resposta, "Ana"));
    }

    [Fact]
    public void G2_dia_pedido_sem_bloco_derruba_o_professor()
    {
        var ana = Cenario.Prof(1, "Ana", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "20:00") });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda", "quarta" }, "18:00", "20:00", 60), ana);

        Assert.False(Cenario.EstaElegivel(resposta, "Ana"));
        Assert.Equal(SemHorario, Cenario.MotivoDe(resposta, "Ana"));
    }

    // =====================================================================
    // H. Regra 7 — Ordenação (menos aulas, depois alfabética)
    // =====================================================================

    [Fact]
    public void H1_menos_aulas_primeiro_e_alfabetica_no_empate()
    {
        var ana = Cenario.Prof(1, "Ana", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") },
            aulasMarcadas: new[]
            {
                Cenario.Aula("terca", "08:00", 30),
                Cenario.Aula("terca", "09:00", 30),
                Cenario.Aula("terca", "10:00", 30)
            });
        var bruno = Cenario.Prof(2, "Bruno", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") });
        var carla = Cenario.Prof(3, "Carla", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") },
            aulasMarcadas: new[] { Cenario.Aula("terca", "08:00", 30) });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "18:00", "20:00", 60), ana, bruno, carla);

        Assert.Equal(new[] { "Bruno", "Carla", "Ana" }, resposta.Professores.Select(p => p.Nome));
    }

    [Fact]
    public void H2_empate_desempatado_alfabeticamente_levando_acento_em_conta()
    {
        var alvaro = Cenario.Prof(1, "Álvaro", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") },
            aulasMarcadas: new[] { Cenario.Aula("terca", "08:00", 30) });
        var bruno = Cenario.Prof(2, "Bruno", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") },
            aulasMarcadas: new[] { Cenario.Aula("terca", "08:00", 30) });
        var aline = Cenario.Prof(3, "Aline", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") },
            aulasMarcadas: new[] { Cenario.Aula("terca", "08:00", 30) });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "18:00", "20:00", 60), alvaro, bruno, aline);

        Assert.Equal(new[] { "Aline", "Álvaro", "Bruno" }, resposta.Professores.Select(p => p.Nome));
    }

    // =====================================================================
    // I. Contagem de aulas (aulasNaSemana)
    // =====================================================================

    [Fact]
    public void I1_aulasNaSemana_conta_a_semana_toda_inclusive_dias_fora_da_busca()
    {
        var ana = AnaSegunda(aulas: new[] { Cenario.Aula("terca", "08:00", 60) });
        var bruno = Cenario.Prof(2, "Bruno", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "18:00", "19:00", 60), ana, bruno);

        Assert.Equal(0, Cenario.ProfessorElegivel(resposta, "Bruno").AulasNaSemana);
        Assert.Equal(1, Cenario.ProfessorElegivel(resposta, "Ana").AulasNaSemana);
    }

    // =====================================================================
    // J. naoElegiveis — motivo e prioridade
    // =====================================================================

    [Fact]
    public void J1_cada_professor_aparece_em_uma_unica_lista()
    {
        var ana = Cenario.Prof(1, "Ana", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") });
        var bruno = Cenario.Prof(2, "Bruno", new[] { "Inglês" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") });
        var carla = Cenario.Prof(3, "Carla", new[] { "Espanhol" }, agendaBloqueada: true,
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") });
        var diego = Cenario.Prof(4, "Diego", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("terca", "08:00", "12:00") });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "18:00", "20:00", 60),
            ana, bruno, carla, diego);

        var idsElegiveis = resposta.Professores.Select(p => p.Id).ToHashSet();
        var idsNaoElegiveis = resposta.NaoElegiveis.Select(p => p.Id).ToHashSet();

        Assert.Empty(idsElegiveis.Intersect(idsNaoElegiveis));
        Assert.Equal(4, resposta.Professores.Count + resposta.NaoElegiveis.Count);
        Assert.Equal(NaoEnsina, Cenario.MotivoDe(resposta, "Bruno"));
        Assert.Equal(Bloqueada, Cenario.MotivoDe(resposta, "Carla"));
        Assert.Equal(SemHorario, Cenario.MotivoDe(resposta, "Diego"));
    }

    [Fact]
    public void J2_sem_elegiveis_as_listas_vem_vazias_nao_null_e_em_ordem_alfabetica()
    {
        var carla = Cenario.Prof(1, "Carla", new[] { "Inglês" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") });
        var ana = Cenario.Prof(2, "Ana", new[] { "Inglês" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") });
        var bruno = Cenario.Prof(3, "Bruno", new[] { "Inglês" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "18:00", "22:00") });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "18:00", "20:00", 60), carla, ana, bruno);

        Assert.NotNull(resposta.Professores);
        Assert.Empty(resposta.Professores);
        Assert.Equal(new[] { "Ana", "Bruno", "Carla" }, resposta.NaoElegiveis.Select(p => p.Nome));
    }

    // =====================================================================
    // K. Casos de borda gerais
    // =====================================================================

    [Fact]
    public void K1_faixa_terminando_23h30_nao_vira_meia_noite()
    {
        var ana = Cenario.Prof(1, "Ana", new[] { "Espanhol" },
            disponibilidade: new[] { Cenario.Bloco("segunda", "20:00", "23:59") });

        var resposta = Cenario.Buscar(
            Cenario.Parametros("Espanhol", new[] { "segunda" }, "20:00", "23:30", 30), ana);

        var horarios = Cenario.HorariosDe(resposta, "Ana");

        Assert.Equal("23:00", horarios[^1]);
        Assert.DoesNotContain("00:00", horarios);
    }
}
