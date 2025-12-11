using Microsoft.AspNetCore.Mvc;                 // ASP.NET Core MVC: atributos, ControllerBase, ActionResult, etc.

using SaphiraTerror.Application.Services;       // ICatalogLookupService: serviço de lookup para listas auxiliares (Application).

namespace SaphiraTerror.Api.Controllers;        // Namespace da API (camada Web).

[ApiController]                                  // Habilita binding/validação automática, 400 problem details, etc.
[Route("api/[controller]")]                      // Rota base: "api/classificacoes" (usa o nome do controller sem "Controller").
public class ClassificacoesController : ControllerBase
{
    private readonly ICatalogLookupService _lookup; // Dependência: serviço de consulta de listas (DIP).

    public ClassificacoesController(ICatalogLookupService lookup) => _lookup = lookup; // DI via construtor.

    [HttpGet]                                     // GET api/classificacoes
    public async Task<ActionResult<object>> Get(CancellationToken ct)
    {
        var items = await _lookup.GetClassificacoesAsync(ct);   // Busca projeções leves (Id, Nome) no serviço.
        return Ok(items.Select(x => new { x.Id, x.Nome }));     // Retorna 200 OK com coleção anônima (shape explícito).
        // Observação: como o serviço já retorna (Id, Nome), o select aqui garante o "shape" no contrato HTTP.
        // Se no futuro o serviço mudar, o controller ainda controla o payload exposto.
    }
}
