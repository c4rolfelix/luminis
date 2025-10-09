using Microsoft.AspNetCore.Mvc;
using Luminis.Models.ViewModels;
using Luminis.Data;
using Luminis.Models;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using System.Linq; // Adicionado para consultas LINQ
using Microsoft.AspNetCore.WebUtilities; // Para WebEncoders
using System.Text;                  // Para Encoding

namespace Luminis.Controllers
{
    public class AccountController : Controller
    {
        private readonly LuminisDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
          LuminisDbContext context,
          UserManager<IdentityUser> userManager,
          SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // --- AÇÕES DE REGISTRO ---

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(PsicologoRegisterViewModel model)
        {
            // Omitido por brevidade: Lógica de validação de email/CRP e criação de IdentityUser
            if (ModelState.IsValid)
            {
                if (await _context.Psicologos.AnyAsync(p => p.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Este e-mail já está em uso.");
                    return View(model);
                }
                if (await _context.Psicologos.AnyAsync(p => p.CRP == model.CRP))
                {
                    ModelState.AddModelError("CRP", "Este CRP já está cadastrado.");
                    return View(model);
                }

                var identityUser = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
                var createIdentityResult = await _userManager.CreateAsync(identityUser, model.Senha);

                if (!createIdentityResult.Succeeded)
                {
                    foreach (var error in createIdentityResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                var psicologo = new Psicologo
                {
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    CRP = model.CRP,
                    Email = model.Email,
                    Biografia = null,
                    FotoUrl = null,
                    WhatsApp = CleanWhatsAppNumber(model.WhatsApp),
                    CPF = CleanCpf(model.CPF),
                    DataNascimento = model.DataNascimento,
                    Ativo = false,
                    DataCadastro = DateTime.Now
                };

                _context.Psicologos.Add(psicologo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Conta criada com sucesso! Faça o login.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // --- AÇÕES DE LOGIN ---

        public IActionResult Login()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // 🛑 CORREÇÃO NO LOGIN: Usa o método de mais alto nível com Email como UserName
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, // Email é o UserName no Identity
                    model.Senha,
                    model.LembrarMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Busca o objeto IdentityUser para verificação de função APÓS o login ser bem-sucedido
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("EditProfile", "Psicologo");
                    }
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Sua conta está bloqueada.");
                    return View(model);
                }
                else
                {
                    // Mensagem genérica para falha (E-mail não existe ou senha errada)
                    ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                    return View(model);
                }
            }

            return View(model);
        }

        // --- AÇÃO DE RECUPERAR SENHA ---

        [HttpGet]
        public IActionResult RecuperarSenha()
        {
            return View(new RecuperarSenhaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarSenha(RecuperarSenhaViewModel model)
        {
            var isSecondStep = model.Validado;

            // --- FLUXO 1: VALIDAR DADOS (Email e CPF) ---
            if (!isSecondStep)
            {
                // 1. CRUCIAL: Removemos a validação dos campos de senha na primeira etapa
                ModelState.Remove(nameof(model.NovaSenha));
                ModelState.Remove(nameof(model.ConfirmarSenha));

                if (ModelState.IsValid) // Verifica se Email e CPF são válidos (formato, obrigatoriedade)
                {
                    var cpfLimpo = CleanCPFNumber(model.CPF);
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    // Verifica se o IdentityUser existe E se o Psicólogo associado tem o CPF correto
                    if (user == null || (await _context.Psicologos.SingleOrDefaultAsync(p => p.Email == user.Email) is not Psicologo psicologo) || CleanCPFNumber(psicologo.CPF) != cpfLimpo)
                    {
                        ModelState.AddModelError(string.Empty, "E-mail ou CPF não encontrados em nossos registros.");
                        return View(model);
                    }

                    // SUCESSO NA VALIDAÇÃO DE IDENTIDADE
                    model.Validado = true;      // <--- ESSA LINHA ATIVA A ETAPA 2 NA VIEW
                    model.UserId = user.Id;     // <--- ESTA LINHA PASSA O ID PARA A PRÓXIMA ETAPA
                    ViewBag.SuccessMessage = "Dados validados com sucesso. Agora insira sua nova senha.";

                    return View(model);         // Retorna o modelo, que agora tem 'Validado = true'
                }

                // Se a validação falhou (ModelState.IsValid é false), retorna a View com o erro visível
                return View(model);
            }

            // --- FLUXO 2: ALTERAR SENHA (Só é alcançado se isSecondStep for true) ---
            else
            {
                // 🛑 AJUSTE CRUCIAL: Removemos os requisitos de validação para os campos da 1ª etapa.
                // Isso evita que o ModelState falhe porque os campos Email e CPF estão visíveis/ocultos, 
                // mas não preenchidos durante o post da segunda etapa.
                ModelState.Remove(nameof(model.Email));
                ModelState.Remove(nameof(model.CPF));

                // Agora o ModelState só valida a NovaSenha e a Confirmação de Senha.
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(model.UserId);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Erro de segurança: Usuário de sessão inválido.");
                        model.Validado = true; // Mantém a tela de senha visível
                        return View(model);
                    }

                    IdentityResult result;
                    result = await _userManager.RemovePasswordAsync(user);

                    if (result.Succeeded || result.Errors.Any(e => e.Code == "UserHasNoPassword"))
                    {
                        result = await _userManager.AddPasswordAsync(user, model.NovaSenha);
                    }

                    if (result.Succeeded)
                    {
                        // SUCESSO!
                        TempData["SuccessMessage"] = "Senha alterada com sucesso! Agora faça seu login.";
                        return RedirectToAction("Login", "Account");
                    }

                    // SE FALHAR: Adiciona erros ao ModelState
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                // Se a validação falhar (seja por erro de Complexidade ou Senhas diferentes), mantém o estado Validado = true
                model.Validado = true;
                return View(model);
            }
        }

            // --- AÇÃO DE LOGOUT ---

            [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // --- MÉTODOS AUXILIARES (CLEANERS) ---

        // Método auxiliar para Registro/Limpeza geral de CPF (nomeado CleanCpf)
        private string CleanCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                return string.Empty;
            }
            return Regex.Replace(cpf, "[^0-9]", "");
        }

        // Método auxiliar para Recuperar Senha (nomeado CleanCPFNumber, mas com mesma lógica)
        private string CleanCPFNumber(string? cpf)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                return string.Empty;
            }
            // Remove todos os caracteres que não são dígitos (0-9)
            return Regex.Replace(cpf, "[^0-9]", "");
        }

        private string CleanWhatsAppNumber(string? whatsApp)
        {
            if (string.IsNullOrEmpty(whatsApp))
            {
                return string.Empty;
            }
            return Regex.Replace(whatsApp, "[^0-9]", "");
        }
    }
}