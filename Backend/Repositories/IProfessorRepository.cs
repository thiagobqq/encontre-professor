using Backend.Models;

namespace Backend.Repositories;

public interface IProfessorRepository
{
    IReadOnlyList<Professor> ObterTodos();
}
