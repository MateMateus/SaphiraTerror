namespace SaphiraTerror.Web.Models;                     // Namespace da camada Web (MVC/Razor) para modelos de View.

// ViewModel de filtros vindos da UI (querystring/form) para consultar o catálogo.
public class CatalogFilterVm
{
    public int? GeneroId { get; set; }                 // Filtro opcional por gênero (FK). null => ignora.
    public int? ClassificacaoId { get; set; }          // Filtro opcional por classificação indicativa (FK).
    public int? Ano { get; set; }                      // Filtro opcional por ano de lançamento (YYYY).
    public string? Q { get; set; }                     // Termo de busca (aplica em Título/Sinopse).
    public string? SortBy { get; set; } = "CreatedAt"; // Campo de ordenação padrão: "CreatedAt" (recentes primeiro).
    public bool Desc { get; set; } = true;             // true = ordem decrescente (padrão); false = crescente.
    public int Page { get; set; } = 1;                 // Página atual (1-based). Default = 1.
    public int PageSize { get; set; } = 12;            // Tamanho da página (itens por página). Default = 12.
}

// ViewModel da página do catálogo: agrega filtros, dados paginados e listas auxiliares para os selects.
public class CatalogPageVm
{
    public CatalogFilterVm Filter { get; set; } = new(); // Filtros atuais aplicados na tela (usa default do tipo).
    public PagedResult<FilmeDto>? PageResult { get; set; } // Resultado paginado (DTOs). null enquanto não houver consulta.
    public IReadOnlyList<(int Id, string Nome)> Generos { get; set; } = Array.Empty<(int, string)>(); // Lista p/ <select>.
    public IReadOnlyList<(int Id, string Nome)> Classificacoes { get; set; } = Array.Empty<(int, string)>(); // Lista p/ <select>.
    // Observação: usar tuplas (Id, Nome) mantém o payload leve para combos.
    // Dica: se a UI precisar de mais metadados (ordem, ícone, slug), considere um DTO leve em vez de tuplas.
}
