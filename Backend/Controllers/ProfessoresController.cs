using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/professores")]
public class ProfessoresController : ControllerBase
{
    private readonly IBuscaProfessoresService _service;

    public ProfessoresController(IBuscaProfessoresService service)
    {
        _service = service;
    }

    [HttpPost("busca")]
    public ActionResult<BuscaProfessoresResponse> Buscar([FromBody] BuscaProfessoresRequest request)
    {
        var resultado = _service.Buscar(request);
        return Ok(resultado);
    }
}
