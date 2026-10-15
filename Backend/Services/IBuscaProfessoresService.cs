using Backend.Models;

namespace Backend.Services;

public interface IBuscaProfessoresService
{
    BuscaProfessoresResponse Buscar(ParametrosBusca parametros);
}
