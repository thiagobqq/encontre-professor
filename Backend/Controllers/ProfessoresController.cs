using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/professores")]
public class ProfessoresController : ControllerBase
{
    private readonly ValidadorBusca _validador;
    private readonly IBuscaProfessoresService _service;

    public ProfessoresController(ValidadorBusca validador, IBuscaProfessoresService service)
    {
        _validador = validador;
        _service = service;
    }

    [HttpPost("busca")]
    public ActionResult<BuscaProfessoresResponse> Buscar([FromBody] BuscaProfessoresRequest request)
    {
        var validacao = _validador.Validar(request);
        if (!validacao.Valido)
            return BadRequest(new ErroValidacaoResponse(validacao.Erro!));

        return Ok(_service.Buscar(validacao.Parametros!));
    }
}
