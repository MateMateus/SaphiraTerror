using SaphiraTerror.Domain.Entities; // Traz a entidade de domínio 'Genero' usada no retorno do repositório.

namespace SaphiraTerror.Application.Abstractions.Repositories; // Camada Application: contrato/abstração, sem detalhes de persistência.

/*
 * Repositório de leitura de Gêneros.
 * Mantém a aplicação desacoplada da tecnologia de dados (EF Core, Dapper, API externa, etc.).
 */
public interface IGeneroRepository
{
    /// <summary>
    /// Retorna todos os gêneros em modo somente leitura.
    /// </summary>
    /// <param name="ct">Token de cancelamento para interromper a operação assíncrona.</param>
    /// <returns>
    /// Coleção imutável (<see cref="IReadOnlyList{T}"/>) de <see cref="Genero"/>, 
    /// preservando a intenção de consulta (sem mutação externa).
    /// </returns>
    Task<IReadOnlyList<Genero>> GetAllAsync(CancellationToken ct = default);
    // Observação: IReadOnlyList comunica intenção clara de não permitir alterações fora do repositório.
}
