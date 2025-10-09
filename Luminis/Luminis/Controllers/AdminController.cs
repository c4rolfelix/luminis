using Microsoft.AspNetCore.Mvc;
using Luminis.Data;
using Luminis.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Luminis.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly LuminisDbContext _context;

        public AdminController(LuminisDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Index (Listagem de Psicólogos para o Admin)
        public async Task<IActionResult> Index(string searchString)
        {
            // O admin vê TODOS os psicólogos, ativos ou inativos
            var psicologos = from p in _context.Psicologos
                             .Include(p => p.Especialidades)
                             .OrderByDescending(p => p.DataCadastro)
                             select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                psicologos = psicologos.Where(s => s.Nome.Contains(searchString)
                                                || s.Sobrenome.Contains(searchString)
                                                || s.Email.Contains(searchString)
                                                || s.CRP.Contains(searchString));
            }

            return View(await psicologos.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var psicologo = await _context.Psicologos.FindAsync(id);

            if (psicologo == null)
            {
                return NotFound();
            }

            psicologo.Ativo = !psicologo.Ativo;

            _context.Update(psicologo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Perfil de {psicologo.Nome} {(psicologo.Ativo ? "ativado" : "desativado")} com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var psicologo = await _context.Psicologos.FindAsync(id);

            if (psicologo != null)
            {
                _context.Psicologos.Remove(psicologo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Perfil de {psicologo.Nome} excluído com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleDestaque(int id)
        {
            var psicologo = await _context.Psicologos.FindAsync(id);

            if (psicologo == null)
            {
                return NotFound();
            }

            psicologo.EmDestaque = !psicologo.EmDestaque;

            _context.Update(psicologo);
            await _context.SaveChangesAsync();

            // Retorna um sucesso para a requisição AJAX, sem redirecionar
            return Ok(new { success = true, isEmDestaque = psicologo.EmDestaque });
        }
    }
}

