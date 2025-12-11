using System.Net.Http.Json;                              // Extensões para (de)serializar JSON com HttpClient.
using System.Text;                                       // StringBuilder para montar querystring.

using Microsoft.Extensions.Caching.Memory;               // IMemoryCache para cache em memória (lists auxiliares).

using SaphiraTerror.Web.Models;                          // DTOs/ViewModels usados pelo cliente (FilmeDto, PagedResult, etc).

namespace SaphiraTerror.Web.Services;                    // Camada Web: serviços que consomem a API HTTP do backend.

// Cliente HTTP da API com cache leve para listas de apoio.
// Primary constructor injeta HttpClient (com BaseAddress configurada no DI) e IMemoryCache.
public sealed class ApiClient(HttpClient http, IMemoryCache cache) : IApiClient
{
    private readonly HttpClient _http = http;            // HttpClient configurado (BaseAddress, headers, etc).
    private readonly IMemoryCache _cache = cache;        // Cache em memória para reduzir round-trips em dados estáveis.

    public async Task<IReadOnlyList<(int Id, string Nome)>> GetGenerosAsync(CancellationToken ct = default)
    {
        // cache de 10 minutos
        if (_cache.TryGetValue("generos", out IReadOnlyList<(int, string)>? cached) && cached is not null)
            return cached;                                // Cache hit: retorna imediatamente.

        var data = await _http.GetFromJsonAsync<List<Item>>("api/generos", cancellationToken: ct)
                   ?? new List<Item>();                   // Tenta desserializar lista de itens; fallback para lista vazia.

        var list = data.Select(x => (x.Id, x.Nome))       // Projeta para tupla leve (Id, Nome) p/ select/combos.
                       .ToList()
                       .AsReadOnly();                     // Converte para ReadOnlyCollection (cumpre IReadOnlyList).

        _cache.Set("generos", list, TimeSpan.FromMinutes(10)); // Armazena no cache com expiração absoluta de 10 min.
        return list;
    }

    public async Task<IReadOnlyList<(int Id, string Nome)>> GetClassificacoesAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue("classificacoes", out IReadOnlyList<(int, string)>? cached) && cached is not null)
            return cached;                                // Cache hit.

        var data = await _http.GetFromJsonAsync<List<Item>>("api/classificacoes", cancellationToken: ct)
                   ?? new List<Item>();                   // Fallback seguro.

        var list = data.Select(x => (x.Id, x.Nome))       // Projeção leve (Id, Nome).
                       .ToList()
                       .AsReadOnly();                     // Evita mutação externa.

        _cache.Set("classificacoes", list, TimeSpan.FromMinutes(10)); // TTL de 10 minutos.
        return list;
    }

    public async Task<PagedResult<FilmeDto>> SearchFilmesAsync(CatalogFilterVm f, CancellationToken ct = default)
    {
        var qs = BuildQuery(f);                           // Constrói querystring com filtros/paginação/ordenação.
        var res = await _http.GetFromJsonAsync<PagedResult<FilmeDto>>($"api/filmes{qs}", cancellationToken: ct)
                  ?? new PagedResult<FilmeDto>(1, 12, 0, Array.Empty<FilmeDto>()); // Fallback determinístico.
        return res;
    }

    public Task<FilmeDto?> GetFilmeAsync(int id, CancellationToken ct = default)
        => _http.GetFromJsonAsync<FilmeDto>($"api/filmes/{id}", cancellationToken: ct); // Retorna null se 404.

    // Constrói a querystring com base no ViewModel de filtro da Web.
    private static string BuildQuery(CatalogFilterVm f)
    {
        var sb = new StringBuilder($"?page={f.Page}&pageSize={f.PageSize}"); // Base: paginação.

        if (f.GeneroId is int gid) sb.Append($"&generoId={gid}");            // Filtro opcional: gênero.
        if (f.ClassificacaoId is int cid) sb.Append($"&classificacaoId={cid}"); // Filtro opcional: classificação.
        if (f.Ano is int ano) sb.Append($"&ano={ano}");                       // Filtro opcional: ano.
        if (!string.IsNullOrWhiteSpace(f.Q))                                  // Busca textual (escapada).
            sb.Append($"&q={Uri.EscapeDataString(f.Q)}");
        if (!string.IsNullOrWhiteSpace(f.SortBy))                             // Campo de ordenação (texto livre; validado no server).
            sb.Append($"&sortBy={f.SortBy}");
        if (!f.Desc) sb.Append("&desc=false");                                // Padrão é true; só envia quando false.

        return sb.ToString();                                                 // Ex.: ?page=1&pageSize=12&generoId=3&desc=false
    }

    // Tipo auxiliar para desserializar os itens (Id, Nome) vindos da API.
    private record Item(int Id, string Nome);
}
