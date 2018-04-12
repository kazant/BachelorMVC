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
using RestSharp.Deserializers;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

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

        //////////////////////////////////////////////////////////////////////////////
        List<string> list = new List<string>();
        public ActionResult getAuth0()
        {
            //var client = new RestClient("https://document.eu.auth0.com/api/v2/users?q=godkjent%3A%220%22&search_engine=v2");
            var client = new RestClient(" https://document.eu.auth0.com/api/v2/users?q=user_metadata%3Anickname%3D%220%22&search_engine=v2");
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik9UUTJOelJDT1VRNVF6Y3pRakk1TnpReFFUTkZOMEkwTmtZMU56YzBOa1V3TVVFMlJVUXlSQSJ9.eyJpc3MiOiJodHRwczovL2RvY3VtZW50LmV1LmF1dGgwLmNvbS8iLCJzdWIiOiJKbGk5SU0wQXF1QTdYZWlDcW5pcmhPd0FYRmcxSDY4UUBjbGllbnRzIiwiYXVkIjoiaHR0cHM6Ly9kb2N1bWVudC5ldS5hdXRoMC5jb20vYXBpL3YyLyIsImlhdCI6MTUyMzUyMzQyNiwiZXhwIjoxNTIzNjA5ODI2LCJhenAiOiJKbGk5SU0wQXF1QTdYZWlDcW5pcmhPd0FYRmcxSDY4USIsInNjb3BlIjoicmVhZDpjbGllbnRfZ3JhbnRzIGNyZWF0ZTpjbGllbnRfZ3JhbnRzIGRlbGV0ZTpjbGllbnRfZ3JhbnRzIHVwZGF0ZTpjbGllbnRfZ3JhbnRzIHJlYWQ6dXNlcnMgdXBkYXRlOnVzZXJzIGRlbGV0ZTp1c2VycyBjcmVhdGU6dXNlcnMgcmVhZDp1c2Vyc19hcHBfbWV0YWRhdGEgdXBkYXRlOnVzZXJzX2FwcF9tZXRhZGF0YSBkZWxldGU6dXNlcnNfYXBwX21ldGFkYXRhIGNyZWF0ZTp1c2Vyc19hcHBfbWV0YWRhdGEgY3JlYXRlOnVzZXJfdGlja2V0cyByZWFkOmNsaWVudHMgdXBkYXRlOmNsaWVudHMgZGVsZXRlOmNsaWVudHMgY3JlYXRlOmNsaWVudHMgcmVhZDpjbGllbnRfa2V5cyB1cGRhdGU6Y2xpZW50X2tleXMgZGVsZXRlOmNsaWVudF9rZXlzIGNyZWF0ZTpjbGllbnRfa2V5cyByZWFkOmNvbm5lY3Rpb25zIHVwZGF0ZTpjb25uZWN0aW9ucyBkZWxldGU6Y29ubmVjdGlvbnMgY3JlYXRlOmNvbm5lY3Rpb25zIHJlYWQ6cmVzb3VyY2Vfc2VydmVycyB1cGRhdGU6cmVzb3VyY2Vfc2VydmVycyBkZWxldGU6cmVzb3VyY2Vfc2VydmVycyBjcmVhdGU6cmVzb3VyY2Vfc2VydmVycyByZWFkOmRldmljZV9jcmVkZW50aWFscyB1cGRhdGU6ZGV2aWNlX2NyZWRlbnRpYWxzIGRlbGV0ZTpkZXZpY2VfY3JlZGVudGlhbHMgY3JlYXRlOmRldmljZV9jcmVkZW50aWFscyByZWFkOnJ1bGVzIHVwZGF0ZTpydWxlcyBkZWxldGU6cnVsZXMgY3JlYXRlOnJ1bGVzIHJlYWQ6cnVsZXNfY29uZmlncyB1cGRhdGU6cnVsZXNfY29uZmlncyBkZWxldGU6cnVsZXNfY29uZmlncyByZWFkOmVtYWlsX3Byb3ZpZGVyIHVwZGF0ZTplbWFpbF9wcm92aWRlciBkZWxldGU6ZW1haWxfcHJvdmlkZXIgY3JlYXRlOmVtYWlsX3Byb3ZpZGVyIGJsYWNrbGlzdDp0b2tlbnMgcmVhZDpzdGF0cyByZWFkOnRlbmFudF9zZXR0aW5ncyB1cGRhdGU6dGVuYW50X3NldHRpbmdzIHJlYWQ6bG9ncyByZWFkOnNoaWVsZHMgY3JlYXRlOnNoaWVsZHMgZGVsZXRlOnNoaWVsZHMgdXBkYXRlOnRyaWdnZXJzIHJlYWQ6dHJpZ2dlcnMgcmVhZDpncmFudHMgZGVsZXRlOmdyYW50cyByZWFkOmd1YXJkaWFuX2ZhY3RvcnMgdXBkYXRlOmd1YXJkaWFuX2ZhY3RvcnMgcmVhZDpndWFyZGlhbl9lbnJvbGxtZW50cyBkZWxldGU6Z3VhcmRpYW5fZW5yb2xsbWVudHMgY3JlYXRlOmd1YXJkaWFuX2Vucm9sbG1lbnRfdGlja2V0cyByZWFkOnVzZXJfaWRwX3Rva2VucyBjcmVhdGU6cGFzc3dvcmRzX2NoZWNraW5nX2pvYiBkZWxldGU6cGFzc3dvcmRzX2NoZWNraW5nX2pvYiByZWFkOmN1c3RvbV9kb21haW5zIGRlbGV0ZTpjdXN0b21fZG9tYWlucyBjcmVhdGU6Y3VzdG9tX2RvbWFpbnMgcmVhZDplbWFpbF90ZW1wbGF0ZXMgY3JlYXRlOmVtYWlsX3RlbXBsYXRlcyB1cGRhdGU6ZW1haWxfdGVtcGxhdGVzIiwiZ3R5IjoiY2xpZW50LWNyZWRlbnRpYWxzIn0.cuPbNZavS1gEWClVAhB4jpodm4-ijCqnJfXTERks3lFHrvdF8krbOMD5yD3LT4XIL5RKPam9odc-A8c7UbyC9yz6UkjuhPhYnggxj18QaL7wPGEqFJoSL6PjNoQRZUuLi1ozPvIzrJr3sgwDUL7B89BLROLydxkEBvLLwzu2IHK-UEk8mNH7eGQELS6VhIk0BWW24jDEQp5rq3TXnI8QPcAQbKOpp3zpio4hJFGq5OzuRr9r92Mj4OBQOU2Nbo83iqUebfIPH7m_pzhujfrPIPuiCjYsfbolDO1JmhRArNeDpQWEhBRY0ENUKS9aHE9gZ6baSeLojRJtQ9nx4_7XIQ");
            IRestResponse response = client.Execute(request);

            List<Testbruker> myobj = JsonConvert.DeserializeObject<List<Testbruker>>(response.Content);

            return View("about", myobj);

        }

        public ActionResult setGodkjent(string id)
        {
            //gi verdi til nickname
            var client = new RestClient("https://document.eu.auth0.com/api/v2/users/" + id);
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik9UUTJOelJDT1VRNVF6Y3pRakk1TnpReFFUTkZOMEkwTmtZMU56YzBOa1V3TVVFMlJVUXlSQSJ9.eyJpc3MiOiJodHRwczovL2RvY3VtZW50LmV1LmF1dGgwLmNvbS8iLCJzdWIiOiJKbGk5SU0wQXF1QTdYZWlDcW5pcmhPd0FYRmcxSDY4UUBjbGllbnRzIiwiYXVkIjoiaHR0cHM6Ly9kb2N1bWVudC5ldS5hdXRoMC5jb20vYXBpL3YyLyIsImlhdCI6MTUyMzU0MTk2NSwiZXhwIjoxMDUyMzU0MTk2NSwiYXpwIjoiSmxpOUlNMEFxdUE3WGVpQ3FuaXJoT3dBWEZnMUg2OFEiLCJzY29wZSI6InJlYWQ6Y2xpZW50X2dyYW50cyBjcmVhdGU6Y2xpZW50X2dyYW50cyBkZWxldGU6Y2xpZW50X2dyYW50cyB1cGRhdGU6Y2xpZW50X2dyYW50cyByZWFkOnVzZXJzIHVwZGF0ZTp1c2VycyBkZWxldGU6dXNlcnMgY3JlYXRlOnVzZXJzIHJlYWQ6dXNlcnNfYXBwX21ldGFkYXRhIHVwZGF0ZTp1c2Vyc19hcHBfbWV0YWRhdGEgZGVsZXRlOnVzZXJzX2FwcF9tZXRhZGF0YSBjcmVhdGU6dXNlcnNfYXBwX21ldGFkYXRhIGNyZWF0ZTp1c2VyX3RpY2tldHMgcmVhZDpjbGllbnRzIHVwZGF0ZTpjbGllbnRzIGRlbGV0ZTpjbGllbnRzIGNyZWF0ZTpjbGllbnRzIHJlYWQ6Y2xpZW50X2tleXMgdXBkYXRlOmNsaWVudF9rZXlzIGRlbGV0ZTpjbGllbnRfa2V5cyBjcmVhdGU6Y2xpZW50X2tleXMgcmVhZDpjb25uZWN0aW9ucyB1cGRhdGU6Y29ubmVjdGlvbnMgZGVsZXRlOmNvbm5lY3Rpb25zIGNyZWF0ZTpjb25uZWN0aW9ucyByZWFkOnJlc291cmNlX3NlcnZlcnMgdXBkYXRlOnJlc291cmNlX3NlcnZlcnMgZGVsZXRlOnJlc291cmNlX3NlcnZlcnMgY3JlYXRlOnJlc291cmNlX3NlcnZlcnMgcmVhZDpkZXZpY2VfY3JlZGVudGlhbHMgdXBkYXRlOmRldmljZV9jcmVkZW50aWFscyBkZWxldGU6ZGV2aWNlX2NyZWRlbnRpYWxzIGNyZWF0ZTpkZXZpY2VfY3JlZGVudGlhbHMgcmVhZDpydWxlcyB1cGRhdGU6cnVsZXMgZGVsZXRlOnJ1bGVzIGNyZWF0ZTpydWxlcyByZWFkOnJ1bGVzX2NvbmZpZ3MgdXBkYXRlOnJ1bGVzX2NvbmZpZ3MgZGVsZXRlOnJ1bGVzX2NvbmZpZ3MgcmVhZDplbWFpbF9wcm92aWRlciB1cGRhdGU6ZW1haWxfcHJvdmlkZXIgZGVsZXRlOmVtYWlsX3Byb3ZpZGVyIGNyZWF0ZTplbWFpbF9wcm92aWRlciBibGFja2xpc3Q6dG9rZW5zIHJlYWQ6c3RhdHMgcmVhZDp0ZW5hbnRfc2V0dGluZ3MgdXBkYXRlOnRlbmFudF9zZXR0aW5ncyByZWFkOmxvZ3MgcmVhZDpzaGllbGRzIGNyZWF0ZTpzaGllbGRzIGRlbGV0ZTpzaGllbGRzIHVwZGF0ZTp0cmlnZ2VycyByZWFkOnRyaWdnZXJzIHJlYWQ6Z3JhbnRzIGRlbGV0ZTpncmFudHMgcmVhZDpndWFyZGlhbl9mYWN0b3JzIHVwZGF0ZTpndWFyZGlhbl9mYWN0b3JzIHJlYWQ6Z3VhcmRpYW5fZW5yb2xsbWVudHMgZGVsZXRlOmd1YXJkaWFuX2Vucm9sbG1lbnRzIGNyZWF0ZTpndWFyZGlhbl9lbnJvbGxtZW50X3RpY2tldHMgcmVhZDp1c2VyX2lkcF90b2tlbnMgY3JlYXRlOnBhc3N3b3Jkc19jaGVja2luZ19qb2IgZGVsZXRlOnBhc3N3b3Jkc19jaGVja2luZ19qb2IgcmVhZDpjdXN0b21fZG9tYWlucyBkZWxldGU6Y3VzdG9tX2RvbWFpbnMgY3JlYXRlOmN1c3RvbV9kb21haW5zIHJlYWQ6ZW1haWxfdGVtcGxhdGVzIGNyZWF0ZTplbWFpbF90ZW1wbGF0ZXMgdXBkYXRlOmVtYWlsX3RlbXBsYXRlcyIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.Rr9Aza_GxIBBHBB1dbstPpFGrS_DFOIHQDIwY0fI1jyU0igQhDZxWBm9GQar5-MvCClWHoU9bOj5zz7NDRqKUMYIdXH99g3PyUld1B1jmIldDoe1nkim4vXg4VtU-YPW1ClpGBtHkoJsTlEOIYXGRQCp6bBkZmvsuJ-p0iceTe9SDQqHe4qod0T_8rCDjjR6kN5Tp14AdbkliQj97Vk23DVNF0eURUwT_8CS06cnSJ6XNfY3Q4cLLi6TwJQdVXhWRhrwNG5UHlFIwnCx1uiAsSDaMUYZQr_bVWqSUmJi7qSkQAKLVO2Q8_hQVa6g5SdID05Z5uMjOuH623jpSL7Vag");
            request.AddParameter("application/json", "{\"user_metadata\": {\"nickname\": \"1\"}}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);



            return getAuth0();
        }


        //////////////////////////////////////////////////////////////////////////////


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

        public void Remove()
        {

        }

    }

    }


