using SaphiraTerror.Web.Models;                 // DTOs/ViewModels usados como contratos na Web (PagedResult<FilmeDto>, CatalogFilterVm, etc).

namespace SaphiraTerror.Web.Services;           // Camada Web: serviços para consumir a API HTTP do backend.

public interface IApiClient
{
    // Retorna lista leve (Id, Nome) para popular <select> de gêneros.
    Task<IReadOnlyList<(int Id, string Nome)>> GetGenerosAsync(CancellationToken ct = default);

    // Retorna lista leve (Id, Nome) para popular <select> de classificações.
    Task<IReadOnlyList<(int Id, string Nome)>> GetClassificacoesAsync(CancellationToken ct = default);

    // Executa consulta paginada/filtrada de filmes na API (espelha contrato do backend).
    Task<PagedResult<FilmeDto>> SearchFilmesAsync(CatalogFilterVm filter, CancellationToken ct = default);

    // Obtém o detalhe de um filme por Id (ou null se não encontrado).
    Task<FilmeDto?> GetFilmeAsync(int id, CancellationToken ct = default);
}
