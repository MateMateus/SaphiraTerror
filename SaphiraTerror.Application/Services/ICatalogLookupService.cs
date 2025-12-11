namespace SaphiraTerror.Application.Services; // Camada Application: serviços de orquestração/regra de aplicação (sem dependência de infraestrutura).

/// <summary>
/// Serviço para consultar listas auxiliares do catálogo (gêneros e classificações).
/// Retorna projeções leves (Id, Nome) para uso em combos/dropdowns sem trafegar entidades inteiras.
/// </summary>
public interface ICatalogLookupService
{
    /// <summary>
    /// Obtém a lista de gêneros em forma de tupla leve (Id, Nome), adequada para UI (select/combobox).
    /// </summary>
    /// <param name="ct">Token de cancelamento para a operação assíncrona.</param>
    /// <returns>
    /// Coleção somente leitura de tuplas nomeadas <c>(int Id, string Nome)</c>.
    /// </returns>
    Task<IReadOnlyList<(int Id, string Nome)>> GetGenerosAsync(CancellationToken ct = default);
    // Observação: usar projeção evita expor entidade de domínio e reduz payload para camadas superiores.

    /// <summary>
    /// Obtém a lista de classificações em forma de tupla leve (Id, Nome), adequada para UI (select/combobox).
    /// </summary>
    /// <param name="ct">Token de cancelamento para a operação assíncrona.</param>
    /// <returns>
    /// Coleção somente leitura de tuplas nomeadas <c>(int Id, string Nome)</c>.
    /// </returns>
    Task<IReadOnlyList<(int Id, string Nome)>> GetClassificacoesAsync(CancellationToken ct = default);
    // Observação: padroniza a saída para a UI e preserva o encapsulamento do domínio.
}
