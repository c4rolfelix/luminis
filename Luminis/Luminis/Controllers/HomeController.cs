using Microsoft.AspNetCore.Mvc;
using Luminis.Data; 
using System.Threading.Tasks; 
using Microsoft.EntityFrameworkCore; 
using System.Linq; 
using System.Diagnostics; 
using Luminis.Models; 
using System; 
using Microsoft.Extensions.Configuration; 

namespace Luminis.Controllers
{
    public class HomeController : Controller
    {
        private readonly LuminisDbContext _context;
        private readonly IConfiguration _configuration; 

        // Construtor atualizado para injetar IConfiguration
        public HomeController(LuminisDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

        public async Task<IActionResult> Planos()
        {
            ViewData["AdminWhatsApp"] = _configuration.GetValue<string>("SiteSettings:AdminWhatsApp");

            var planos = await _context.Planos.OrderBy(p => p.Preco).ToListAsync();
            return View(planos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
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