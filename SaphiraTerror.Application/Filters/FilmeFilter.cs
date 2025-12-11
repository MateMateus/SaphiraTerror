//using SaphiraTerror.Application.DTOs; // Importa o namespace onde está PagedRequest (classe/record base de paginação).

namespace SaphiraTerror.Application.Filters; // Define o namespace do filtro (organização por camada/pacote).

///// <summary>
///// Filtros do catálogo + paginação/ordenação.
///// É um 'record' para herdar de PagedRequest (que também é record).
///// </summary>
//public sealed record FilmeFilter : PagedRequest // 'record' imutável por padrão; 'sealed' evita herança; herda de PagedRequest (contém Page e PageSize).
//{
//    public int? GeneroId { get; init; }         // Filtro opcional por gênero (nullable: pode não filtrar).
//    public int? ClassificacaoId { get; init; }  // Filtro opcional por classificação indicativa.
//    public int? Ano { get; init; }              // Filtro opcional por ano de lançamento.
//    public string? Q { get; init; }             // Texto de busca (Título/Sinopse); nullable para ser opcional.
//    public string? SortBy { get; init; }        // Campo para ordenação (ex.: "Titulo", "Ano", "CreatedAt").
//    public bool Desc { get; init; } = true;     // Se true, ordena descendente por padrão (útil para "mais recentes primeiro").

//    // Construtor primário do record nominal (com corpo), repassando paginação para a base.
//    public FilmeFilter(
//        int? generoId = null,                   // Parâmetros opcionais com default = null, para filtros não aplicados.
//        int? classificacaoId = null,
//        int? ano = null,
//        string? q = null,
//        string? sortBy = null,
//        bool desc = true,                       // Ordenação descendente como padrão.
//        int page = 1,                           // Página padrão = 1 (convencional em APIs).
//        int pageSize = 12                       // Tamanho de página padrão (ex.: 12 cards por grid).
//    ) : base(page, pageSize)                    // Chama o construtor de PagedRequest para setar paginação.
//    {
//        GeneroId = generoId;                    // Inicializa as propriedades 'init' (imutáveis após construção).
//        ClassificacaoId = classificacaoId;      // Corrigido possível typo: mantém consistência com o nome da prop.
//        Ano = ano;
//        Q = q;
//        SortBy = sortBy;
//        Desc = desc;                            // Define a direção da ordenação.
//    }
//}


// refatorado fase 03                                        // Tag informativa da fase atual (não afeta compilação).

// Classe (não record) para o binder preencher pelas props   // Usamos 'class' para permitir set mutável via model binding.
using SaphiraTerror.Application.DTOs;


public sealed class FilmeFilter : PagedRequest              // Herda paginação básica (Page, PageSize) de PagedRequest.
{
    public int? GeneroId { get; set; }                      // Filtro opcional por gênero (FK). null = não filtra.
    public int? ClassificacaoId { get; set; }               // Filtro opcional por classificação indicativa (FK).
    public int? Ano { get; set; }                           // Filtro opcional por ano de lançamento (YYYY).
    public string? Q { get; set; }        // busca em Título/Sinopse  // Termo de busca textual (LIKE em Título/Sinopse).
    public string? SortBy { get; set; }   // "Titulo" | "Ano" | "CreatedAt" // Campo de ordenação aceito pelo repositório.
    public bool Desc { get; set; } = true;                  // true = ordem decrescente; false = crescente. Default: desc.
    // Observação: normalização (limites de paginação e mapeamento de SortBy) é tratada no service/repo.
}
