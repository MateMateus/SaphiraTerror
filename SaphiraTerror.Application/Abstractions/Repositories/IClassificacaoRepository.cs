using SaphiraTerror.Domain.Entities; // Importa a entidade de domínio 'Classificacao' usada nas assinaturas abaixo.

namespace SaphiraTerror.Application.Abstractions.Repositories; // Camada de Application → contrato (abstração) do repositório.

/*
 * Este repositório define a fronteira de acesso a dados para a entidade Classificacao.
 * Mantém a aplicação desacoplada da implementação concreta (EF Core, Dapper, API externa, etc.).
 */
public interface IClassificacaoRepository
{
    /// <summary>
    /// Obtém todas as classificações de filme em modo somente leitura.
    /// </summary>
    /// <param name="ct">
    /// Token de cancelamento para interromper a operação assíncrona (útil em requisições HTTP, shutdown gracioso, etc.).
    /// </param>
    /// <returns>
    /// Lista imutável (<see cref="IReadOnlyList{T}"/>) de <see cref="Classificacao"/>; 
    /// a imutabilidade reforça o contrato de consulta (consulta não deve alterar estado da coleção retornada).
    /// </returns>
    Task<IReadOnlyList<Classificacao>> GetAllAsync(CancellationToken ct = default);
    // ↑ Task representa operação assíncrona (I/O bound). IReadOnlyList evita alterações da coleção fora do repositório.
}
