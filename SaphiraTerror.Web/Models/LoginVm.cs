namespace SaphiraTerror.Web.Models;

// ViewModel simples para a tela de Login.
// É o objeto que o formulário POST vai “bindar”.
public class LoginVm
{
    // URL para voltar após logar. Normalmente vem como input hidden.
    public string? ReturnUrl { get; set; }

    // E-mail digitado pelo usuário. Aqui está como string livre (sem validação ainda).
    public string Email { get; set; } = "";

    // Senha digitada. Evite re-exibir esse valor em caso de erro de validação.
    public string Password { get; set; } = "";

    // “Lembrar-me” para cookie persistente.
    public bool RememberMe { get; set; }
}
