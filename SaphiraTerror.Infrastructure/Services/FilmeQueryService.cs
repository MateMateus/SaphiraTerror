using SaphiraTerror.Application.Abstractions.Repositories; // Abstrações de repositório (Application) consumidas por este serviço.
using SaphiraTerror.Application.DTOs;                     // DTOs expostos para UI/API; aqui usaremos FilmeDto e PagedResult<T>.
using SaphiraTerror.Application.Filters;                  // Record FilmeFilter: filtros/ordenação/paginação.
using SaphiraTerror.Application.Services;                 // Contrato IFilmeQueryService implementado por esta classe.

namespace SaphiraTerror.Infrastructure.Services;           // Camada Infra: implementação concreta de serviços de aplicação.

/// <summary>
/// Serviço de consulta que conecta o repositório (Infra) às projeções em DTO (Application).
/// Orquestra filtros/paginação e projeta entidades de domínio em <see cref="FilmeDto"/>.
/// </summary>
public sealed class FilmeQueryService : IFilmeQueryService // 'sealed' evita herança acidental; favorece previsibilidade.
{
    private readonly IFilmeRepository _repo;               // Dependência: repositório de leitura de filmes.

    public FilmeQueryService(IFilmeRepository repo) => _repo = repo; // DI por construtor (DIP). Testável com mocks/fakes.

    /// <summary>
    /// Executa busca paginada de filmes usando <see cref="FilmeFilter"/> e retorna <see cref="PagedResult{T}"/> de <see cref="FilmeDto"/>.
    /// </summary>
    /// <param name="filter">Critérios de filtro (gênero, classificação, ano, texto), ordenação e paginação.</param>
    /// <param name="ct">Token de cancelamento para cooperação com o pipeline assíncrono.</param>
    public async Task<PagedResult<FilmeDto>> SearchAsync(FilmeFilter filter, CancellationToken ct = default)
    {
        var (items, total) = await _repo.SearchAsync(filter, ct); // Consulta repositório: retorna itens + total após filtros.

        var dtos = items.Select(f => new FilmeDto(           // Projeção de entidade → DTO (evita vazar domínio para UI).
            f.Id,                                            // Id do filme.
            f.Titulo,                                        // Título exibido na UI.
            f.Sinopse,                                       // Pode ser null; DTO deve aceitar.
            f.Ano,                                           // Ano de lançamento.
            f.ImagemCapaUrl,                                 // URL da imagem de capa (pode ser null/vazia conforme regra).
            f.GeneroId,                                      // FK do gênero (para usos de binding/edits).
            f.Genero.Nome,                                   // Nome do gênero (navegação incluída pelo repo).
            f.ClassificacaoId,                               // FK da classificação indicativa.
            f.Classificacao.Nome                             // Nome da classificação (navegação incluída pelo repo).
        )).ToList();                                         // Materializa para cumprir IReadOnlyList no PagedResult.

        // Saneamento de paginação igual ao repositório (mantém consistência de limites).
        var page = filter.Page < 1 ? 1 : filter.Page;        // Página mínima = 1.
        var pageSize = filter.PageSize < 1 ? 12 : filter.PageSize; // Tamanho padrão quando inválido/zero.
        if (pageSize > 48) pageSize = 48;                    // Limite superior para evitar payloads exagerados.

        return new PagedResult<FilmeDto>(page, pageSize, total, dtos); // Embala itens + metadados de paginação.
    }
}
