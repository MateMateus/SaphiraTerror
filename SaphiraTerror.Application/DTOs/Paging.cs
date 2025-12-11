namespace SaphiraTerror.Application.DTOs;
// criado fase 1
/// <summary>
/// Pedido de paginação padrão (valores serão saneados no serviço).
/// </summary>
//public record PagedRequest(int Page = 1, int PageSize = 12);


// refatorado fase 03                                   // Tag informativa (histórico da fase). Não afeta compilação.

// Classe simples para o binder setar via query (?page=&pageSize=)  // Observação de uso: model binding pega da querystring.
public class PagedRequest
{
    public int Page { get; set; } = 1;                 // Página solicitada (1-based). Default = 1 para evitar 0/negativos.
    public int PageSize { get; set; } = 12;            // Tamanho da página. Default = 12 (equilíbrio entre payload e UX).
    // Observação: a validação/saneamento final (mín/max) foi tratado nos services/repos.
    // Em projetos maiores, pode-se adicionar data annotations (ex.: [Range(1, 48)]) para reforçar contrato.
}


/// <summary>
/// Resultado de paginação com metadados.
/// </summary>
public record PagedResult<T>(                         // 'record' é ótimo para DTOs imutáveis por valor (com equality por estado).
    int Page,                                         // Página atual (já normalizada pelo service).
    int PageSize,                                     // Tamanho da página efetivo (após saneamento de limites).
    int Total,                                        // Total de itens que satisfazem o filtro (sem paginação).
    IReadOnlyList<T> Items);                          // Itens da página corrente; IReadOnlyList comunica somente leitura.
// Observação: manter consistência semântica: Page e PageSize referem-se à resposta, não necessariamente ao que veio na requisição.
// Dica: quando útil para UI, é possível adicionar membros calculados (ex.: TotalPages) em um outro tipo ou método de extensão.




