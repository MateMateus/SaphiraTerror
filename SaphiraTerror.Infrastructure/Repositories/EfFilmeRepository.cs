using Microsoft.EntityFrameworkCore;                         // EF Core: IQueryable, Include, AsNoTracking, CountAsync, ToListAsync, etc.

using SaphiraTerror.Application.Abstractions.Repositories;   // Contrato IFilmeRepository (camada Application).
using SaphiraTerror.Application.Filters;                     // Record FilmeFilter (filtros + paginação/ordenação).
using SaphiraTerror.Domain.Entities;                         // Entidade de domínio Filme.
using SaphiraTerror.Infrastructure.Persistence;               // AppDbContext (DbContext do EF Core).

namespace SaphiraTerror.Infrastructure.Repositories;          // Camada Infra: implementação concreta de repositório.

/// <summary>
/// Implementação EF Core do repositório de filmes.
/// </summary>
public sealed class EfFilmeRepository : IFilmeRepository      // 'sealed' evita herança acidental e ajuda o JIT.
{
    private readonly AppDbContext _ctx;                       // DbContext injetado (lifetime típico: Scoped em ASP.NET Core).

    public EfFilmeRepository(AppDbContext ctx) => _ctx = ctx; // DI por construtor: facilita testes e substituição por mocks.

    public async Task<(IReadOnlyList<Filme> Items, int Total)> SearchAsync(
        FilmeFilter filter, CancellationToken ct = default)
    {
        // Saneamento de paginação
        var page = filter.Page < 1 ? 1 : filter.Page;         // Garante página mínima = 1.
        var pageSize = filter.PageSize < 1 ? 12 : filter.PageSize; // Garante pageSize padrão quando inválido/zero.
        if (pageSize > 48) pageSize = 48;                     // Limite superior para evitar requests exagerados (throttling básico).

        var q = _ctx.Filmes
            .AsNoTracking()                                   // Leitura somente; desativa ChangeTracker → melhor performance.
            .Include(f => f.Genero)                           // Carrega navegação 'Genero' (JOIN) para evitar N+1.
            .Include(f => f.Classificacao)                    // Carrega navegação 'Classificacao'.
            .AsQueryable();                                   // Mantém composição de query (adiar execução).

        // Filtros
        if (filter.GeneroId is int gid)                       // Se veio filtro de gênero (valor presente)
            q = q.Where(f => f.GeneroId == gid);              //   aplica filtro por chave estrangeira.

        if (filter.ClassificacaoId is int cid)                // Se veio filtro de classificação
            q = q.Where(f => f.ClassificacaoId == cid);       //   aplica filtro correspondente.

        if (filter.Ano is int ano)                            // Se veio filtro de ano
            q = q.Where(f => f.Ano == ano);                   //   aplica igualdade (considera ano exato).

        if (!string.IsNullOrWhiteSpace(filter.Q))             // Busca textual simples (título/sinopse) quando houver termo.
        {
            var like = $"%{filter.Q.Trim()}%";                // Prepara padrão LIKE (contém). EF parameteriza → seguro contra SQL injection.
            q = q.Where(f =>
                EF.Functions.Like(f.Titulo, like) ||          // LIKE no título.
                (f.Sinopse != null && EF.Functions.Like(f.Sinopse, like))); // LIKE na sinopse quando não nula.
        }

        // Ordenação
        q = (filter.SortBy?.ToLowerInvariant()) switch        // Normaliza chave de ordenação para cultura invariável.
        {
            "titulo" => filter.Desc ? q.OrderByDescending(f => f.Titulo) : q.OrderBy(f => f.Titulo),
            "ano" => filter.Desc ? q.OrderByDescending(f => f.Ano) : q.OrderBy(f => f.Ano),
            "createdat" => filter.Desc ? q.OrderByDescending(f => f.CreatedAt) : q.OrderBy(f => f.CreatedAt),
            _ => q.OrderByDescending(f => f.CreatedAt)               // Padrão: mais recentes primeiro.
        };

        var total = await q.CountAsync(ct);                   // Conta total após filtros (para paginação).

        var items = await q
            .Skip((page - 1) * pageSize)                      // Desloca para o início da página.
            .Take(pageSize)                                   // Limita ao tamanho da página.
            .ToListAsync(ct);                                 // Materializa resultados.

        return (items, total);                                // Retorna tupla nomeada: itens da página + total filtrado.
    }

    public Task<Filme?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _ctx.Filmes
            .AsNoTracking()                                   // Leitura somente.
            .Include(f => f.Genero)                           // Inclui navegação 'Genero'.
            .Include(f => f.Classificacao)                    // Inclui navegação 'Classificacao'.
            .FirstOrDefaultAsync(f => f.Id == id, ct);        // Retorna o primeiro ou null se não houver.
}
