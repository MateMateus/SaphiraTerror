using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using SaphiraTerror.Infrastructure.Entities;   // ✅ ApplicationUser do mesmo namespace do DbContext
using SaphiraTerror.Web.Models;

namespace SaphiraTerror.Web.Controllers;

// ✅ Controller “fino”: injeta serviços Identity já prontos (SignInManager/UserManager)
//    Usa o novo recurso de "primary constructor" em classes (C# 12/.NET 8+).
public class AuthController(
    SignInManager<ApplicationUser> signIn,
    UserManager<ApplicationUser> users) : Controller
{
    // 🔒 Campos somente-leitura apontando para os serviços injetados
    private readonly SignInManager<ApplicationUser> _signIn = signIn;
    private readonly UserManager<ApplicationUser> _users = users;

    // GET /Auth/Login?returnUrl=...
    // 🔓 Anônimo: precisa estar disponível para quem ainda não está autenticado
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
        // ✅ Preenche o VM com o ReturnUrl (será usado no POST para redirecionar)
        => View(new LoginVm { ReturnUrl = returnUrl });

    // POST /Auth/Login
    // 🔓 Anônimo (óbvio), e com AntiForgery para evitar CSRF em POSTs
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm model, CancellationToken ct)
    {
        // ❗ Se o formulário veio inválido (validações do VM/DataAnnotations), retorna a própria View
        if (!ModelState.IsValid) return View(model);

        // 🔎 Procura o usuário pelo e-mail
        var user = await _users.FindByEmailAsync(model.Email);

        // 🚧 Checa se existe e se está ativo
        // ⚠️ Aqui você usa "dynamic" para acessar IsActive; isso compila mas perde segurança de tipos.
        //    Ideal: ApplicationUser ter bool IsActive { get; set; } e checar fortemente tipado.
        if (user is null || (user is { } && (user as dynamic).IsActive == false))
        {
            ModelState.AddModelError("", "Usuário inválido ou inativo.");
            return View(model);
        }

        // 🔐 Tenta autenticar com cookie (SignInManager resolve hashing, lockout, etc.)
        //     lockoutOnFailure=false => não incrementa tentativas (você pode querer true com políticas)
        var res = await _signIn.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

        // ❌ Falha de credencial
        if (!res.Succeeded)
        {
            ModelState.AddModelError("", "Credenciais inválidas.");
            return View(model);
        }

        // 🔁 Se veio ReturnUrl local, redireciona pra lá (previne open redirect com Url.IsLocalUrl)
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        // ✅ Senão manda para a Home com um param (ex.: âncora de gêneros)
        return RedirectToAction("Index", "Home", new { section = "genres" });
    }

    // POST /Auth/Logout
    // 🔐 Usuário logado posta um formulário de logout com AntiForgery
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync(); // ✅ remove o cookie de autenticação
        return RedirectToAction("Index", "Home");
    }

    // GET /Auth/Denied
    // 🚫 Página simples para AccessDeniedPath do Cookie
    [HttpGet]
    public IActionResult Denied() => Content("Acesso negado.");
}
