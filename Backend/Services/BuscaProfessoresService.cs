using Backend.Models;
using Backend.Repositories;

namespace Backend.Services;

public class BuscaProfessoresService : IBuscaProfessoresService
{
    private readonly IProfessorRepository _repository;

    public BuscaProfessoresService(IProfessorRepository repository)
    {
        _repository = repository;
    }

    public BuscaProfessoresResponse Buscar(BuscaProfessoresRequest request)
    {
        _ = _repository.ObterTodos();

        return new BuscaProfessoresResponse
        {
            Professores = [],
            NaoElegiveis = []
        };
    }
}
