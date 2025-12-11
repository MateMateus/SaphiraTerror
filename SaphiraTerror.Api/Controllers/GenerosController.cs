using Microsoft.AspNetCore.Mvc;                 // MVC: atributos para roteamento, ControllerBase, ActionResult, etc.

using SaphiraTerror.Application.Services;       // ICatalogLookupService: serviço de listas auxiliares (Application).

namespace SaphiraTerror.Api.Controllers;        // Camada Web/API.

[ApiController]                                  // Auto-binding/validação + ProblemDetails em 400.
[Route("api/[controller]")]                      // Base route: "api/generos" (nome do controller sem o sufixo "Controller").
public class GenerosController : ControllerBase
{
    private readonly ICatalogLookupService _lookup; // Serviço de consulta (DIP). Controller não conhece repositórios.

    public GenerosController(ICatalogLookupService lookup) => _lookup = lookup; // DI via construtor.

    [HttpGet]                                     // GET api/generos
    public async Task<ActionResult<object>> Get(CancellationToken ct)
    {
        var items = await _lookup.GetGenerosAsync(ct);          // Retorna projeções leves (Id, Nome) do serviço.
        // Tupla -> objeto forte (Id, Nome)                      // Controla o "shape" do payload HTTP explicitamente.
        return Ok(items.Select(x => new { x.Id, x.Nome }));     // 200 OK com coleção anônima contendo apenas o necessário.
    }
}
