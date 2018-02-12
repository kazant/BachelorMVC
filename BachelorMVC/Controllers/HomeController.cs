using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BachelorMVC.Models;
using BachelorMVC.Persistence;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BachelorMVC.Controllers
{
    public class HomeController : Controller
    {
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

        public IActionResult Error(BachelorDbContext context)
        {

            context.Dokumenter.Add(new Dokumenter {Name ="testdokument"}); //lag dokument
            context.SaveChangesAsync();// lagre dokument
            Dokumenter dokument = context.Dokumenter.FirstOrDefault(x => x.Name == "dwa" || x.DokumentID == 2); // henter første dokument.
            IEnumerable<Dokumenter> doc = context.Dokumenter.Where(x => x.Name == "dwa" || x.DokumentID == 2); // henter alle dokumenter som er godtatt i spørringen.
            
            return View();
        }

        // The Authorize attribute requires the user to be authenticated and will
        // kick off the OIDC authentication flow 
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Innlogget()
        {
            return View();
        }

         public async Task UploadToSDS() 
        {
            
            var httpClientHandler = new HttpClientHandler 
            {
                Credentials = new NetworkCredential("demo", "Bond007")
            };

            using (var client = new HttpClient(httpClientHandler)) 
            {
                // todo: HTML File som parameter
                Byte[] file = System.IO.File.ReadAllBytes("Controllers/vedlegg1.pdf");
                HttpContent content = new ByteArrayContent(file);
                
                // Mulighet for flere filtyper og validering
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                
                // Dokumentet blir lastet opp til demo-SDS og responsen lagres for videre bruk
                HttpResponseMessage response = 
                    await client.PostAsync("https://preprod.signicat.com/doc/demo/sds", content);

                string documentId = await response.Content.ReadAsStringAsync();

                // For testing av responsen
                using(StreamWriter sw = System.IO.File.CreateText("Controllers/output.txt"))
                {
                    sw.WriteLine(documentId);
                }
                
            }

            
                
        

        }
    }
}
