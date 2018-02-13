using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BachelorMVC.Models;
using BachelorMVC.Persistence;
using BachelorMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BachelorMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly BachelorDbContext _context;
        private readonly IbrukerService _brukerService;

        public HomeController(BachelorDbContext contex, IbrukerService brukerService)
        {
            _brukerService = brukerService; 
            _context = contex;
        }
        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {

            _context.Dokumenter.Add(new Dokumenter {Name ="testdokument"}); //lag dokument
            _context.SaveChangesAsync();// lagre dokument
            Dokumenter dokument = _context.Dokumenter.FirstOrDefault(x => x.Name == "dwa" || x.DokumentID == 2); // henter første dokument.
            IEnumerable<Dokumenter> doc = _context.Dokumenter.Where(x => x.Name == "dwa" || x.DokumentID == 2); // henter alle dokumenter som er godtatt i spørringen.
            
            return View();
        }

        // The Authorize attribute requires the user to be authenticated and will
        // kick off the OIDC authentication flow 
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Innlogget()
        {
            string id = User.Claims.Where(c => c.Type == "socialno").FirstOrDefault().Value;
            string navn = User.Claims.Where(c => c.Type == "name").FirstOrDefault().Value;
            string[] fulltnavn = navn.Split(',');
            string fornavn = fulltnavn[1];
            string etternavn = fulltnavn[0];
            var result = _brukerService.findbruker(navn,id);


            if (result != null)
            {                
                ViewBag.testtext = result.Fornavn;
                return View();
            }
            else
            {
                Bruker nybruker =_brukerService.addbruker(fornavn, etternavn, id);
                ViewBag.testtext = nybruker.Fornavn;
                return View();
            }

           
        }

    }
}
