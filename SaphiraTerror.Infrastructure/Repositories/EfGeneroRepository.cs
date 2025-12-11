using Microsoft.EntityFrameworkCore;                       // EF Core: extensões como AsNoTracking, ToListAsync, OrderBy.

using SaphiraTerror.Application.Abstractions.Repositories; // Contrato IGeneroRepository (camada Application).
using SaphiraTerror.Domain.Entities;                      // Entidade de domínio 'Genero'.
using SaphiraTerror.Infrastructure.Persistence;            // AppDbContext (DbContext da aplicação).

namespace SaphiraTerror.Infrastructure.Repositories;       // Camada Infra: implementação concreta de repositório.

/*
 * Implementação do repositório de 'Genero' usando Entity Framework Core.
 * 'sealed' evita herança e ajuda a manter a classe enxuta e previsível.
 */
public sealed class EfGeneroRepository : IGeneroRepository
{
    private readonly AppDbContext _ctx;                    // DbContext injetado (lifetime típico: Scoped em apps web).

    public EfGeneroRepository(AppDbContext ctx) => _ctx = ctx; // DI via construtor: facilita testes/mocks.

    /// <summary>
    /// Retorna todos os gêneros, ordenados por nome, sem rastreamento (read-only).
    /// </summary>
    /// <param name="ct">Token de cancelamento para cooperação em I/O.</param>
    public async Task<IReadOnlyList<Genero>> GetAllAsync(CancellationToken ct = default) =>
        await _ctx.Generos                 // DbSet<Genero>: origem da consulta.
            .AsNoTracking()                // Desativa tracking: mais leve para leituras (não haverá update dessas entidades).
            .OrderBy(g => g.Nome)          // Ordena no servidor por Nome (avalie índice se consulta frequente).
            .ToListAsync(ct);              // Materializa de forma assíncrona respeitando o CancellationToken.
}
