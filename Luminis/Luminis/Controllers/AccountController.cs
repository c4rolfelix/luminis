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
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

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

        // cadastro
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(PsicologoRegisterViewModel model)
        {
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

        // login
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
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Senha,
                    model.LembrarMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
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
                    ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                    return View(model);
                }
            }

            return View(model);
        }

        // recuperar senja

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

            // etapa 1: verifica email ecpf
            if (!isSecondStep)
            {
                ModelState.Remove(nameof(model.NovaSenha));
                ModelState.Remove(nameof(model.ConfirmarSenha));

                if (ModelState.IsValid) 
                {
                    var cpfLimpo = CleanCPFNumber(model.CPF);
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user == null || (await _context.Psicologos.SingleOrDefaultAsync(p => p.Email == user.Email) is not Psicologo psicologo) || CleanCPFNumber(psicologo.CPF) != cpfLimpo)
                    {
                        ModelState.AddModelError(string.Empty, "E-mail ou CPF não encontrados em nossos registros.");
                        return View(model);
                    }

                    model.Validado = true;
                    model.UserId = user.Id;
                    ViewBag.SuccessMessage = "Dados validados com sucesso. Agora insira sua nova senha.";

                    return View(model);
                }

                return View(model);
            }

            // etapa 2
            else
            {
                ModelState.Remove(nameof(model.Email));
                ModelState.Remove(nameof(model.CPF));

                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(model.UserId);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Erro de segurança: Usuário de sessão inválido.");
                        model.Validado = true; 
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
                        TempData["SuccessMessage"] = "Sua senha foi alterada com sucesso! Faça o login.";
                        return RedirectToAction("Login", "Account");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                model.Validado = true;
                return View(model);
            }
        }

        // logout

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private string CleanCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                return string.Empty;
            }
            return Regex.Replace(cpf, "[^0-9]", "");
        }

        private string CleanCPFNumber(string? cpf)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                return string.Empty;
            }
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