using Microsoft.AspNetCore.Mvc;
using Luminis.Data; // DbContext
using System.Threading.Tasks; // Para async/await
using Microsoft.EntityFrameworkCore; // Para Include, Take, ToListAsync
using System.Linq; // Para OrderBy, Take
using System.Diagnostics; // Para ErrorViewModel 
using Luminis.Models; // Para ErrorViewModel 

namespace Luminis.Controllers
{
    public class HomeController : Controller
    {
        private readonly LuminisDbContext _context;

        public HomeController(LuminisDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Carrega algumas especialidades para a busca rápida na Home
            ViewBag.Especialidades = await _context.Especialidades
                                                   .OrderBy(e => e.Nome)
                                                   .Take(5)
                                                   .ToListAsync();

            // Carrega uma pequena amostra de psicólogos ativos para exibir na Home
            var psicologosAmostra = await _context.Psicologos
                                                  .Where(p => p.Ativo == true)
                                                  .Include(p => p.Especialidades)
                                                  .OrderByDescending(p => p.EmDestaque) // Prioriza os em destaque
                                                  .ThenBy(p => Guid.NewGuid()) // Ordena os demais aleatoriamente
                                                  .Take(3)
                                                  .ToListAsync();

            return View(psicologosAmostra);
        }

        public IActionResult Planos()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Terms()
        {
            return View("~/Views/Info/Terms.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}