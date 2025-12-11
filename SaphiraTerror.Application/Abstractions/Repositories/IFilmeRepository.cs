using SaphiraTerror.Application.Filters; // Importa o tipo FilmeFilter (filtros + paginação/ordenação) usado na consulta.
using SaphiraTerror.Domain.Entities;     // Importa a entidade de domínio Filme para tipar retornos e parâmetros.

namespace SaphiraTerror.Application.Abstractions.Repositories; // Camada Application: apenas contratos (DIP), sem detalhes de persistência.

/// <summary>
/// Repositório de leitura/escrita de filmes.
/// Nesta fase usamos só leitura com filtros/paginação.
/// </summary>
public interface IFilmeRepository
{
    /// <summary>
    /// Pesquisa filmes aplicando filtros, ordenação e paginação.
    /// </summary>
    /// <param name="filter">Critérios de filtro (gênero, classificação, ano, busca textual), ordenação e paginação.</param>
    /// <param name="ct">Token de cancelamento para interromper a operação assíncrona.</param>
    /// <returns>
    /// Uma tupla contendo:
    /// <list type="bullet">
    /// <item><description><c>Items</c>: página de filmes materializada (somente leitura).</description></item>
    /// <item><description><c>Total</c>: total de registros que batem com o filtro, sem paginação.</description></item>
    /// </list>
    /// </returns>
    Task<(IReadOnlyList<Filme> Items, int Total)> SearchAsync(
        FilmeFilter filter,                // Record com filtros e paginação; padroniza a entrada e facilita evolução.
        CancellationToken ct = default);   // Padrão em I/O bound para cancelamento gracioso (ex.: requisições HTTP abortadas).

    /// <summary>
    /// Obtém um filme por identificador (chave primária).
    /// </summary>
    /// <param name="id">Identificador do filme.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Um <see cref="Filme"/> ou <c>null</c> se não encontrado.</returns>
    Task<Filme?> GetByIdAsync(int id, CancellationToken ct = default); // Retorna null quando não encontrado (fluxo comum).
}
