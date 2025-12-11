using SaphiraTerror.Application.Abstractions.Repositories; // Abstrações de repositórios (Application) usadas como dependências.
using SaphiraTerror.Application.Services;                  // Contrato ICatalogLookupService (Application) a ser implementado.

namespace SaphiraTerror.Infrastructure.Services;            // Camada Infra: implementação concreta de serviços da aplicação.

/// <summary>
/// Fornece listas auxiliares (gêneros/classificações) em forma simples.
/// Implementa projeções leves (Id, Nome) para consumo direto pela UI/API sem expor entidades de domínio.
/// </summary>
public sealed class CatalogLookupService : ICatalogLookupService // 'sealed' evita herança acidental.
{
    private readonly IGeneroRepository _generos;                 // Repositório de leitura de Gênero.
    private readonly IClassificacaoRepository _classificacoes;   // Repositório de leitura de Classificação.

    // Injeção de dependências via construtor (DIP): a Infra fornece a implementação concreta dos repositórios.
    public CatalogLookupService(
        IGeneroRepository generos,
        IClassificacaoRepository classificacoes)
    {
        _generos = generos;                   // Armazena a dependência injetada (lifetime típico: Scoped).
        _classificacoes = classificacoes;     // Idem.
    }

    /// <summary>
    /// Retorna lista de gêneros como tuplas (Id, Nome) para popular selects/combos.
    /// </summary>
    /// <param name="ct">Token de cancelamento para interromper a operação assíncrona.</param>
    /// <returns>Coleção somente leitura de tuplas (Id, Nome).</returns>
    public async Task<IReadOnlyList<(int Id, string Nome)>> GetGenerosAsync(CancellationToken ct = default)
    {
        var list = await _generos.GetAllAsync(ct);          // Busca entidades sem tracking (no repo).
        return list.Select(g => (g.Id, g.Nome)).ToList();   // Projeta para (Id, Nome); ToList() satisfaz IReadOnlyList<T>.
    }

    /// <summary>
    /// Retorna lista de classificações como tuplas (Id, Nome).
    /// </summary>
    /// <param name="ct">Token de cancelamento para interromper a operação assíncrona.</param>
    /// <returns>Coleção somente leitura de tuplas (Id, Nome).</returns>
    public async Task<IReadOnlyList<(int Id, string Nome)>> GetClassificacoesAsync(CancellationToken ct = default)
    {
        var list = await _classificacoes.GetAllAsync(ct);       // Busca entidades sem tracking (no repo).
        return list.Select(c => (c.Id, c.Nome)).ToList();       // Projeta para (Id, Nome); coleção leve para a UI.
    }
}
