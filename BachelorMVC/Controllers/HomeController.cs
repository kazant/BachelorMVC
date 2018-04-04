using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BachelorMVC.Models;
using BachelorMVC.Persistence;
using BachelorMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Assently.Client;
using Assently.ServiceModel;
using Assently.ServiceModel.Messages;
using RestSharp;
using RestSharp.Authenticators;
using System.IO;

namespace BachelorMVC.Controllers
{
    public class HomeController : Controller
    {

        string id;
        string navn;

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
            //id = User.Claims.Where(c => c.Type == "socialno").FirstOrDefault().Value;
            //navn = User.Claims.Where(c => c.Type == "name").FirstOrDefault().Value;
            //string[] fulltnavn = navn.Split(',');
            //string fornavn = fulltnavn[1];
            //string etternavn = fulltnavn[0];
            //var result = _brukerService.findbruker(navn,id);


            //if (result != null)
            //{                
            //    ViewBag.testtext = result.Fornavn;
            //    return View();
            //}
            //else
            //{
            //    Bruker nybruker =_brukerService.addbruker(fornavn, etternavn, id);
            //    ViewBag.testtext = nybruker.Fornavn;
                return View();
            //}

           
        }



        public IActionResult LagDokumentGrid()
        {



            return View();
        }

       

        public IActionResult InspiserDokument() {

            return View();
        }

        public IActionResult OpprettCaseOgSendEpost()
        {

            //Hent info om bruker
            //Bruker bruker = _brukerService.findbruker(id, navn);

            //Hent info om brukerens dokument
            // Trenger en kobling mellom klassen Bruker og klassen Dokument



            var client = new AssentlyClient("https://test.assently.com", "1ab291ce-7486-488a-a5dc-de81ae692eae", "E76l9Vt91QiU6AJTZPX4vzXXjloVWpVa4vib4mio");

            //En CreateCaseModel skal bestå av et dokument, en eller flere brukere og annen info (se UML)
            CreateCaseModel model = new CreateCaseModel();
            
            //Påkrevd
            model.Id = Guid.NewGuid();
 
            //Påkrevd
            model.SendSignRequestEmailToParties = true;
            model.SendFinishEmailToParties = true;
            model.SendFinishEmailToCreator = true;
            model.Name = "Test";
            model.NameAlias = "TestAlias";

            //Kan gi valg mellom eID signatur eller signbyhand (på mobil). Påkrevd
            model.AllowedSignatureTypes.Add(SignatureType.ElectronicId);


            //PartyModel er en samling brukere. Påkrevd.
            //Skal flere brukere signere ett dokument, må denne kodebiten gjentas.
            model.Parties.Add(new PartyModel
            {
                //Her kan info hentes fra klassen Bruker
                EmailAddress = "HailTheUser@gmail.com",
                Name = "Erlend Andreas Hall"
            });

            //En eller flere dokumenter angis til en Liste med dokumenter
            //I prinsippet er det nok med en filsti til dokumentet. Påkrevd
            string statiskFilsti = "Controllers\\debug_attest.pdf";
            model.Documents.Add(statiskFilsti);
            model.Metadata.Add("nøkkel","verdi");


            //CreateCaseModel objektet sendes til Assently
            client.CreateCase(model);

            //Her blir brukerene evt tilsendt en epost med signaturlink
            //Evt kan også SMS benyttes
            client.SendCase(model.Id);

            return View();
        }

        public void LastNedSignertDokument()
        {
            var client = new AssentlyClient("https://test.assently.com", "1ab291ce-7486-488a-a5dc-de81ae692eae", "E76l9Vt91QiU6AJTZPX4vzXXjloVWpVa4vib4mio");

            //Må her hente en eksisterende CaseModel fra Assently ved hjelp av et GUID
            CaseModel caseModel = null;  //client.GetCase(Guid.Parse(caseId guid));

            if (caseModel.Status == CaseStatus.Sent)
            {

                var receipt = caseModel.Documents.Where(d => d.Type == DocumentType.Original).Single();
                var stream = client.GetDocumentData(Guid.Parse(receipt.Id), ".\\Controllers\\debug_attest.pfd");


            }

        }

        public ActionResult getAuth0()
    {
        var client = new RestClient("https://document.eu.auth0.com/api/v2/users/auth0|5ab7af1b4d9ffe30cb1334aa");
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer " +
                                               "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik9UUTJOelJDT1VRNVF6Y3pRakk1TnpReFFUTkZOMEkwTmtZMU56YzBOa1V3TVVFMlJVUXlSQSJ9.eyJpc3MiOiJodHRwczovL2RvY3VtZW50LmV1LmF1dGgwLmNvbS8iLCJzdWIiOiJKbGk5SU0wQXF1QTdYZWlDcW5pcmhPd0FYRmcxSDY4UUBjbGllbnRzIiwiYXVkIjoiaHR0cHM6Ly9kb2N1bWVudC5ldS5hdXRoMC5jb20vYXBpL3YyLyIsImlhdCI6MTUyMjc0NDQ3MywiZXhwIjoxNTIyODMwODczLCJhenAiOiJKbGk5SU0wQXF1QTdYZWlDcW5pcmhPd0FYRmcxSDY4USIsInNjb3BlIjoicmVhZDpjbGllbnRfZ3JhbnRzIGNyZWF0ZTpjbGllbnRfZ3JhbnRzIGRlbGV0ZTpjbGllbnRfZ3JhbnRzIHVwZGF0ZTpjbGllbnRfZ3JhbnRzIHJlYWQ6dXNlcnMgdXBkYXRlOnVzZXJzIGRlbGV0ZTp1c2VycyBjcmVhdGU6dXNlcnMgcmVhZDp1c2Vyc19hcHBfbWV0YWRhdGEgdXBkYXRlOnVzZXJzX2FwcF9tZXRhZGF0YSBkZWxldGU6dXNlcnNfYXBwX21ldGFkYXRhIGNyZWF0ZTp1c2Vyc19hcHBfbWV0YWRhdGEgY3JlYXRlOnVzZXJfdGlja2V0cyByZWFkOmNsaWVudHMgdXBkYXRlOmNsaWVudHMgZGVsZXRlOmNsaWVudHMgY3JlYXRlOmNsaWVudHMgcmVhZDpjbGllbnRfa2V5cyB1cGRhdGU6Y2xpZW50X2tleXMgZGVsZXRlOmNsaWVudF9rZXlzIGNyZWF0ZTpjbGllbnRfa2V5cyByZWFkOmNvbm5lY3Rpb25zIHVwZGF0ZTpjb25uZWN0aW9ucyBkZWxldGU6Y29ubmVjdGlvbnMgY3JlYXRlOmNvbm5lY3Rpb25zIHJlYWQ6cmVzb3VyY2Vfc2VydmVycyB1cGRhdGU6cmVzb3VyY2Vfc2VydmVycyBkZWxldGU6cmVzb3VyY2Vfc2VydmVycyBjcmVhdGU6cmVzb3VyY2Vfc2VydmVycyByZWFkOmRldmljZV9jcmVkZW50aWFscyB1cGRhdGU6ZGV2aWNlX2NyZWRlbnRpYWxzIGRlbGV0ZTpkZXZpY2VfY3JlZGVudGlhbHMgY3JlYXRlOmRldmljZV9jcmVkZW50aWFscyByZWFkOnJ1bGVzIHVwZGF0ZTpydWxlcyBkZWxldGU6cnVsZXMgY3JlYXRlOnJ1bGVzIHJlYWQ6cnVsZXNfY29uZmlncyB1cGRhdGU6cnVsZXNfY29uZmlncyBkZWxldGU6cnVsZXNfY29uZmlncyByZWFkOmVtYWlsX3Byb3ZpZGVyIHVwZGF0ZTplbWFpbF9wcm92aWRlciBkZWxldGU6ZW1haWxfcHJvdmlkZXIgY3JlYXRlOmVtYWlsX3Byb3ZpZGVyIGJsYWNrbGlzdDp0b2tlbnMgcmVhZDpzdGF0cyByZWFkOnRlbmFudF9zZXR0aW5ncyB1cGRhdGU6dGVuYW50X3NldHRpbmdzIHJlYWQ6bG9ncyByZWFkOnNoaWVsZHMgY3JlYXRlOnNoaWVsZHMgZGVsZXRlOnNoaWVsZHMgdXBkYXRlOnRyaWdnZXJzIHJlYWQ6dHJpZ2dlcnMgcmVhZDpncmFudHMgZGVsZXRlOmdyYW50cyByZWFkOmd1YXJkaWFuX2ZhY3RvcnMgdXBkYXRlOmd1YXJkaWFuX2ZhY3RvcnMgcmVhZDpndWFyZGlhbl9lbnJvbGxtZW50cyBkZWxldGU6Z3VhcmRpYW5fZW5yb2xsbWVudHMgY3JlYXRlOmd1YXJkaWFuX2Vucm9sbG1lbnRfdGlja2V0cyByZWFkOnVzZXJfaWRwX3Rva2VucyBjcmVhdGU6cGFzc3dvcmRzX2NoZWNraW5nX2pvYiBkZWxldGU6cGFzc3dvcmRzX2NoZWNraW5nX2pvYiByZWFkOmN1c3RvbV9kb21haW5zIGRlbGV0ZTpjdXN0b21fZG9tYWlucyBjcmVhdGU6Y3VzdG9tX2RvbWFpbnMgcmVhZDplbWFpbF90ZW1wbGF0ZXMgY3JlYXRlOmVtYWlsX3RlbXBsYXRlcyB1cGRhdGU6ZW1haWxfdGVtcGxhdGVzIiwiZ3R5IjoiY2xpZW50LWNyZWRlbnRpYWxzIn0.BnZ5RH2R0Yh5bV26JUIitgvraKPxZ3Iibu5gQD_DnEzQ9BZMnCPDOqjd32m8iCQPLWLdP7JZgP_mNlQViABKYdJsFYV4kPRrOPJ820UOrybCXRlV24GPFmP65V6Nusrq5k9RPsddIiTLB2FUDEI5JSQ1N-5ybNOnEmg8735nvz4Li9V3MvXKi2pIZ-hL9XOeYVMJHe4Wcsxu6J8yURoP2PzwfoNNiEzsV1HGVzgHG1t8GgoS71gBkbSai3ag4nryXRoaFRXLp_i0OJdCXEexSgNTscmh0WPgkuDhtLeRpdJbhEu3vFMl0jBecTUhTwkWX6TynpGDS6NGsWQAG--BMQ");
            request.AddParameter("application/json", "{\"user_metadata\": {\"godkjent\": \"1\"}}", ParameterType.RequestBody);
            //request.AddParameter("application/json", "{\"user_metadata\": {\"addresses\": {\"home\": \"123 Main Streetssss, Anytown, ST 12345\"}}}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
         
            return View("Innlogget");
    }

    }

    }


