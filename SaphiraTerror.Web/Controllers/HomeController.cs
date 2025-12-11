//using Microsoft.AspNetCore.Mvc;

//namespace SaphiraTerror.Web.Controllers;

//public class HomeController : Controller
//{
//    public IActionResult Index() => View();
//    public IActionResult About() => View();
//    public IActionResult Catalog() => View();
//    public IActionResult Genre() => View();
//    public IActionResult Logout() => RedirectToAction("Index"); // placeholder Fase 5
//}



//refatorado fase 4

using Microsoft.AspNetCore.Mvc;

using SaphiraTerror.Web.Models;                // ViewModels/DTOs usados na camada Web (CatalogFilterVm, CatalogPageVm, etc).
using SaphiraTerror.Web.Services;             // IApiClient: cliente HTTP para consumir a API do backend.

namespace SaphiraTerror.Web.Controllers;      // Namespace da camada Web (MVC).

// Controller com primary-constructor: injeta IApiClient e ILogger<HomeController>.
public class HomeController(IApiClient api, ILogger<HomeController> log) : Controller
{
    private readonly IApiClient _api = api;   // Mantém referência ao cliente da API para chamadas de dados.
    // Observação: 'log' está disponível via parâmetro, mas não está armazenado em um campo; use diretamente se necessário.

    // Monta o ViewModel da página do catálogo, normalizando paginação e preenchendo listas auxiliares.
    private async Task<CatalogPageVm> BuildVm(CatalogFilterVm filter, CancellationToken ct)
    {
        if (filter.Page < 1) filter.Page = 1;                         // Saneamento: página mínima é 1.
        if (filter.PageSize < 1 || filter.PageSize > 48) filter.PageSize = 12; // Saneamento: tamanho válido (1..48), padrão 12.

        return new CatalogPageVm
        {
            Filter = filter,                                           // Retém filtros atuais (para reflexão na UI).
            Generos = await _api.GetGenerosAsync(ct),                  // Preenche <select> de gêneros (com cache no ApiClient).
            Classificacoes = await _api.GetClassificacoesAsync(ct),    // Preenche <select> de classificações (cache idem).
            PageResult = await _api.SearchFilmesAsync(filter, ct)      // Consulta paginada para o grid/catálogo.
        };
    }

    // ONE-PAGE: /  (aceita filtros via querystring)
    [HttpGet("")]                                                      // Rota raiz do site.
    public async Task<IActionResult> Index([FromQuery] CatalogFilterVm filter, string? section, CancellationToken ct)
    {
        var vm = await BuildVm(filter, ct);                            // Constrói o ViewModel com dados + filtros.
        ViewBag.Section = section; // "about" | "genres" | "catalog"   // Indica qual seção rolar/ativar na One-Page.
        return View(vm);                                               // Renderiza a View fortemente tipada.
    }

    // Compat: /Home/Catalog -> Index em #catalog
    [HttpGet]                                                          // GET /Home/Catalog
    public Task<IActionResult> Catalog([FromQuery] CatalogFilterVm filter, CancellationToken ct)
        => Index(filter, "catalog", ct);                               // Reusa Index e ativa seção "catalog".

    // Compat: /Home/Genre -> Index em #genres
    [HttpGet]                                                          // GET /Home/Genre
    public Task<IActionResult> Genre(CancellationToken ct)
        => Index(new CatalogFilterVm(), "genres", ct);                 // Reusa Index com filtros default e seção "genres".
}
