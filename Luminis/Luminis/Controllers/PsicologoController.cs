using Microsoft.AspNetCore.Mvc;
using Luminis.Data;
using Luminis.Models;
using Luminis.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; 
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.RegularExpressions;

namespace Luminis.Controllers
{
    public class PsicologoController : Controller
    {
        private readonly LuminisDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SignInManager<IdentityUser> _signInManager;

        public PsicologoController(
            LuminisDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(string searchSpecialty)
        {
            ViewBag.Especialidades = await _context.Especialidades
                                                 .OrderBy(e => e.Nome)
                                                 .ToListAsync();

            IQueryable<Psicologo> psicologosQuery = _context.Psicologos
                                                            .Where(p => p.Ativo == true)
                                                            .Include(p => p.Especialidades);

            if (!string.IsNullOrEmpty(searchSpecialty))
            {
                psicologosQuery = psicologosQuery.Where(p =>
                    p.Especialidades.Any(e => e.Nome.Contains(searchSpecialty))
                );
            }

            var psicologos = await psicologosQuery.ToListAsync();

            return View(psicologos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var psicologo = await _context.Psicologos
                                            .Include(p => p.Especialidades)
                                            .FirstOrDefaultAsync(m => m.Id == id && m.Ativo == true);

            if (psicologo == null)
            {
                return NotFound();
            }

            return View(psicologo);
        }

        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var loggedInUserEmail = User.FindFirstValue(ClaimTypes.Email);
            var psicologo = await _context.Psicologos
                                            .Include(p => p.Especialidades)
                                            .SingleOrDefaultAsync(p => p.Email == loggedInUserEmail);

            if (psicologo == null)
            {
                return NotFound("Perfil de psicólogo não encontrado.");
            }

            ViewBag.AllEspecialidades = await _context.Especialidades.OrderBy(e => e.Nome).ToListAsync();

            var viewModel = new PsicologoEditViewModel
            {
                Id = psicologo.Id,
                Nome = psicologo.Nome,
                Sobrenome = psicologo.Sobrenome,
                CRP = psicologo.CRP,
                Email = psicologo.Email,
                Biografia = psicologo.Biografia,
                FotoUrl = psicologo.FotoUrl,
                WhatsApp = psicologo.WhatsApp,
                WhatsAppUrl = psicologo.WhatsAppUrl,
                CPF = psicologo.CPF,
                DataNascimento = psicologo.DataNascimento,
                EspecialidadesSelecionadasIds = psicologo.Especialidades?.Select(e => e.Id).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditProfile(PsicologoEditViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var loggedInUserEmail = User.FindFirstValue(ClaimTypes.Email);
            var psicologo = await _context.Psicologos
                                            .Include(p => p.Especialidades)
                                            .SingleOrDefaultAsync(p => p.Id == model.Id);

            if (psicologo == null)
            {
                return NotFound("Perfil de psicólogo não encontrado.");
            }

            if (psicologo.CRP != model.CRP && await _context.Psicologos.AnyAsync(p => p.CRP == model.CRP && p.Id != model.Id))
            {
                ModelState.AddModelError("CRP", "Este CRP já está cadastrado para outro profissional.");
            }


            if (ModelState.IsValid)
            {
                const string DEFAULT_AVATAR_URL = "/images/default-avatar.png";
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "uploads");
                bool isNewFileUpload = model.ProfileImageFile != null && model.ProfileImageFile.Length > 0;
                bool isRemoveAction = model.FotoUrl == DEFAULT_AVATAR_URL && !string.IsNullOrEmpty(psicologo.FotoUrl) && psicologo.FotoUrl != DEFAULT_AVATAR_URL;


                if ((isNewFileUpload || isRemoveAction) && !string.IsNullOrEmpty(psicologo.FotoUrl) && psicologo.FotoUrl != DEFAULT_AVATAR_URL)
                {
                    if (psicologo.FotoUrl.StartsWith("/images/uploads/"))
                    {
                        string existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, psicologo.FotoUrl.TrimStart('/'));

                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }
                }

                if (isRemoveAction)
                {
                    psicologo.FotoUrl = null;
                }

                else if (isNewFileUpload)
                {
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImageFile.CopyToAsync(fileStream);
                    }

                    psicologo.FotoUrl = "/images/uploads/" + uniqueFileName;
                }
               
                psicologo.Nome = model.Nome;
                psicologo.Sobrenome = model.Sobrenome;
                psicologo.CRP = model.CRP;
                psicologo.Email = model.Email;
                psicologo.Biografia = model.Biografia;
                psicologo.WhatsApp = CleanWhatsAppNumber(model.WhatsApp);
                psicologo.CPF = model.CPF;
                psicologo.DataNascimento = model.DataNascimento;
                psicologo.WhatsAppUrl = model.WhatsAppUrl;

                // Lógica de Especialidades
                psicologo.Especialidades.Clear();
                if (model.EspecialidadesSelecionadasIds != null)
                {
                    var selectedEspecialidades = await _context.Especialidades
                                                                .Where(e => model.EspecialidadesSelecionadasIds.Contains(e.Id))
                                                                .ToListAsync();
                    foreach (var esp in selectedEspecialidades)
                    {
                        psicologo.Especialidades.Add(esp);
                    }
                }

                _context.Update(psicologo);
                await _context.SaveChangesAsync(); 

                TempData["SuccessMessage"] = "Seu perfil foi atualizado com sucesso! Ele aparecerá na página inicial e nas buscas após a autorização do administrador.";
                return RedirectToAction(nameof(EditProfile));
            }

            ViewBag.AllEspecialidades = await _context.Especialidades.OrderBy(e => e.Nome).ToListAsync();
            return View(model);
        }

        private string CleanWhatsAppNumber(string? whatsApp)
        {
            if (string.IsNullOrEmpty(whatsApp))
            {
                return string.Empty;
            }
            return Regex.Replace(whatsApp, "[^0-9]", "");
        }

        // excluir conta
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var identityUser = await _userManager.FindByIdAsync(userId);
            if (identityUser == null)
            {
                return NotFound("Usuário de login não encontrado.");
            }

            var psicologo = await _context.Psicologos
                                            .SingleOrDefaultAsync(p => p.Email == identityUser.Email);

            if (psicologo != null && !string.IsNullOrEmpty(psicologo.FotoUrl))
            {
                const string DEFAULT_AVATAR_URL = "/images/default-avatar.png";

                if (psicologo.FotoUrl != DEFAULT_AVATAR_URL && psicologo.FotoUrl.StartsWith("/images/uploads/"))
                {
                    string existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, psicologo.FotoUrl.TrimStart('/'));

                    if (System.IO.File.Exists(existingFilePath))
                    {
                        System.IO.File.Delete(existingFilePath); 
                    }
                }
            }

            if (psicologo != null)
            {
                _context.Psicologos.Remove(psicologo);
                await _context.SaveChangesAsync();
            }

            var result = await _userManager.DeleteAsync(identityUser);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Ocorreu um erro ao excluir a conta de login. Por favor, tente novamente.";
                return RedirectToAction(nameof(EditProfile));
            }

            await _signInManager.SignOutAsync(); 

            TempData["SuccessMessage"] = "Sua conta foi excluída com sucesso. Sentiremos sua falta!";

            return RedirectToAction("Index", "Home");
        }
    }
}