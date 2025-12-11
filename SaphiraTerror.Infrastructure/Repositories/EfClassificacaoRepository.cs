using Microsoft.EntityFrameworkCore;                      // EF Core: ToListAsync, AsNoTracking, IQueryable extensions.

using SaphiraTerror.Application.Abstractions.Repositories; // Contrato que esta classe implementa (DIP).
using SaphiraTerror.Domain.Entities;                      // Entidade 'Classificacao' retornada pelo repositório.
using SaphiraTerror.Infrastructure.Persistence;            // AppDbContext (Unit of Work/Session do EF Core).

namespace SaphiraTerror.Infrastructure.Repositories;        // Camada de Infra: implementações concretas de acesso a dados.

/*
 * Implementação concreta do repositório de Classificação usando Entity Framework Core.
 * 'sealed' evita herança acidental; favorece previsibilidade e micro-otimizações do runtime/JIT.
 */
public sealed class EfClassificacaoRepository : IClassificacaoRepository
{
    private readonly AppDbContext _ctx;                         // DbContext injetado (lifetime normalmente Scoped em apps web).

    public EfClassificacaoRepository(AppDbContext ctx) => _ctx = ctx; // DI via construtor: facilita testes/mocking.

    /// <summary>
    /// Retorna todas as classificações em ordem alfabética, sem rastreamento de mudanças.
    /// </summary>
    /// <param name="ct">Token de cancelamento para encerrar a operação assíncrona, se necessário.</param>
    /// <returns>Lista somente leitura de <see cref="Classificacao"/>.</returns>
    public async Task<IReadOnlyList<Classificacao>> GetAllAsync(CancellationToken ct = default) =>
        await _ctx.Classificacoes                // DbSet<Classificacao>: representa a tabela/consulta base.
            .AsNoTracking()                      // Desabilita tracking: melhora performance em cenários de leitura (read-only).
            .OrderBy(c => c.Nome)                // Ordenação no servidor por Nome (índice recomendado se consulta frequente).
            .ToListAsync(ct);                    // Materializa a consulta de forma assíncrona respeitando o CancellationToken.
}
