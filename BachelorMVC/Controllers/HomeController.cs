using System;
using System.Collections.Generic;
using System.Linq;
using BachelorMVC.Models;
using BachelorMVC.Persistence;
using BachelorMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Assently.Client;
using Assently.ServiceModel;
using Assently.ServiceModel.Messages;
using RestSharp;
using Newtonsoft.Json;
using System.IO;



namespace BachelorMVC.Controllers
{
    public class HomeController : Controller
    {

        string id;
        string navn;
        int antallSign;
        private DBController DBController = new DBController ();

        public HomeController()
        {
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

            //_context.Dokumenter.Add(new Dokument {Navn ="testdokument"}); //lag dokument
            //_context.SaveChangesAsync();// lagre dokument
            //Dokument dokument = _context.Dokumenter.FirstOrDefault(x => x.Navn == "dwa" || x.Id == 2); // henter første dokument.
            //IEnumerable<Dokument> doc = _context.Dokumenter.Where(x => x.Navn == "dwa" || x.Id == 2); // henter alle dokumenter som er godtatt i spørringen.
            
            return View();
        }

        public IActionResult InspiserDokument() {

            return View();
        }

       
        public void OpprettCaseOgSendEpost(string epost, string caseNavn, string dokumentNavn, string signeringsmetode)
        {

            string[] emails;

            if(!(epost == null))
            {
                emails = epost.Split(',');
            } else
            {
                return; 
            }
            
            var client = new AssentlyClient("https://test.assently.com", "1ab291ce-7486-488a-a5dc-de81ae692eae", "OMw4uXqu1QgCX_ESA8XpI00Z7EKyIlypwgrlv-qu");

            //En CreateCaseModel skal bestå av et dokument, en eller flere brukere og annen info
            CreateCaseModel model = new CreateCaseModel();
            
            //Påkrevd
            model.Id = Guid.NewGuid();
            model.SendSignRequestEmailToParties = true;
            model.SendFinishEmailToParties = true;
            model.SendFinishEmailToCreator = true;
            model.Name = caseNavn;
            model.NameAlias = "TestAlias";

            //Kan gi valg mellom eID signatur eller signbyhand (på mobil). Påkrevd
            if(signeringsmetode == "electronicid") {
                model.AllowedSignatureTypes.Add(SignatureType.ElectronicId);
            } else {
                model.AllowedSignatureTypes.Add(SignatureType.Touch);
            }

            //PartyModel er en samling brukere. Påkrevd.
            //Skal flere brukere signere ett dokument, må denne kodebiten gjentas.
            for (var i = 0; i < emails.Length - 1; i++)
            {

                model.Parties.Add(new PartyModel
                {
                    
                    EmailAddress = emails[i]
                });
            }
            
            //En eller flere dokumenter angis til en Liste med dokumenter
            model.Documents.Add("./Persistence/Dokumenter/" + dokumentNavn);
            model.Metadata.Add("nøkkel","verdi");

            using (var fileStream = new FileStream("./Persistence/guid.txt", FileMode.Create)) {
                byte[] data = model.Id.ToByteArray();
                fileStream.Write(data, 0, data.Length);
            }

            string email = User.Claims.Where(c => c.Type == "name").FirstOrDefault().Value;
            DBController.WriteDocument(model.Id, dokumentNavn, emails.Length, caseNavn, epost);

            //CreateCaseModel objektet sendes til Assently
            client.CreateCase(model);

            //Her blir brukerene evt tilsendt en epost med signaturlink
            //Evt kan også SMS benyttes
            client.SendCase(model.Id);

            //Oppdater antallsignatur teller Auth0
            OppdaterAntallOppdragTeller();

        }


        [HttpPost]
       public void Upload()
        {
            
            for (int i = 0; i < Request.Form.Files.Count; i++)
            {

                var file = Request.Form.Files[i];
                var fileName = "./Persistence/Dokumenter/" + System.IO.Path.GetFileName(file.FileName);

                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    Console.Write(file);
                    file.CopyTo(fileStream);
                }
            }

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
        public void OppdaterAntallOppdragTeller()
        {

            string epost = User.Claims.Where(c => c.Type == "name").FirstOrDefault().Value;
            var client = new RestClient("https://document.eu.auth0.com/api/v2/users-by-email?email=" + epost);
            var requestGet = new RestRequest(Method.GET);
            requestGet.AddHeader("authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik9UUTJOelJDT1VRNVF6Y3pRakk1TnpReFFUTkZOMEkwTmtZMU56YzBOa1V3TVVFMlJVUXlSQSJ9.eyJpc3MiOiJodHRwczovL2RvY3VtZW50LmV1LmF1dGgwLmNvbS8iLCJzdWIiOiJKbGk5SU0wQXF1QTdYZWlDcW5pcmhPd0FYRmcxSDY4UUBjbGllbnRzIiwiYXVkIjoiaHR0cHM6Ly9kb2N1bWVudC5ldS5hdXRoMC5jb20vYXBpL3YyLyIsImlhdCI6MTUyMzYxMjg1MiwiZXhwIjoxMDUyMzYxMjg1MiwiYXpwIjoiSmxpOUlNMEFxdUE3WGVpQ3FuaXJoT3dBWEZnMUg2OFEiLCJzY29wZSI6InJlYWQ6Y2xpZW50X2dyYW50cyBjcmVhdGU6Y2xpZW50X2dyYW50cyBkZWxldGU6Y2xpZW50X2dyYW50cyB1cGRhdGU6Y2xpZW50X2dyYW50cyByZWFkOnVzZXJzIHVwZGF0ZTp1c2VycyBkZWxldGU6dXNlcnMgY3JlYXRlOnVzZXJzIHJlYWQ6dXNlcnNfYXBwX21ldGFkYXRhIHVwZGF0ZTp1c2Vyc19hcHBfbWV0YWRhdGEgZGVsZXRlOnVzZXJzX2FwcF9tZXRhZGF0YSBjcmVhdGU6dXNlcnNfYXBwX21ldGFkYXRhIGNyZWF0ZTp1c2VyX3RpY2tldHMgcmVhZDpjbGllbnRzIHVwZGF0ZTpjbGllbnRzIGRlbGV0ZTpjbGllbnRzIGNyZWF0ZTpjbGllbnRzIHJlYWQ6Y2xpZW50X2tleXMgdXBkYXRlOmNsaWVudF9rZXlzIGRlbGV0ZTpjbGllbnRfa2V5cyBjcmVhdGU6Y2xpZW50X2tleXMgcmVhZDpjb25uZWN0aW9ucyB1cGRhdGU6Y29ubmVjdGlvbnMgZGVsZXRlOmNvbm5lY3Rpb25zIGNyZWF0ZTpjb25uZWN0aW9ucyByZWFkOnJlc291cmNlX3NlcnZlcnMgdXBkYXRlOnJlc291cmNlX3NlcnZlcnMgZGVsZXRlOnJlc291cmNlX3NlcnZlcnMgY3JlYXRlOnJlc291cmNlX3NlcnZlcnMgcmVhZDpkZXZpY2VfY3JlZGVudGlhbHMgdXBkYXRlOmRldmljZV9jcmVkZW50aWFscyBkZWxldGU6ZGV2aWNlX2NyZWRlbnRpYWxzIGNyZWF0ZTpkZXZpY2VfY3JlZGVudGlhbHMgcmVhZDpydWxlcyB1cGRhdGU6cnVsZXMgZGVsZXRlOnJ1bGVzIGNyZWF0ZTpydWxlcyByZWFkOnJ1bGVzX2NvbmZpZ3MgdXBkYXRlOnJ1bGVzX2NvbmZpZ3MgZGVsZXRlOnJ1bGVzX2NvbmZpZ3MgcmVhZDplbWFpbF9wcm92aWRlciB1cGRhdGU6ZW1haWxfcHJvdmlkZXIgZGVsZXRlOmVtYWlsX3Byb3ZpZGVyIGNyZWF0ZTplbWFpbF9wcm92aWRlciBibGFja2xpc3Q6dG9rZW5zIHJlYWQ6c3RhdHMgcmVhZDp0ZW5hbnRfc2V0dGluZ3MgdXBkYXRlOnRlbmFudF9zZXR0aW5ncyByZWFkOmxvZ3MgcmVhZDpzaGllbGRzIGNyZWF0ZTpzaGllbGRzIGRlbGV0ZTpzaGllbGRzIHVwZGF0ZTp0cmlnZ2VycyByZWFkOnRyaWdnZXJzIHJlYWQ6Z3JhbnRzIGRlbGV0ZTpncmFudHMgcmVhZDpndWFyZGlhbl9mYWN0b3JzIHVwZGF0ZTpndWFyZGlhbl9mYWN0b3JzIHJlYWQ6Z3VhcmRpYW5fZW5yb2xsbWVudHMgZGVsZXRlOmd1YXJkaWFuX2Vucm9sbG1lbnRzIGNyZWF0ZTpndWFyZGlhbl9lbnJvbGxtZW50X3RpY2tldHMgcmVhZDp1c2VyX2lkcF90b2tlbnMgY3JlYXRlOnBhc3N3b3Jkc19jaGVja2luZ19qb2IgZGVsZXRlOnBhc3N3b3Jkc19jaGVja2luZ19qb2IgcmVhZDpjdXN0b21fZG9tYWlucyBkZWxldGU6Y3VzdG9tX2RvbWFpbnMgY3JlYXRlOmN1c3RvbV9kb21haW5zIHJlYWQ6ZW1haWxfdGVtcGxhdGVzIGNyZWF0ZTplbWFpbF90ZW1wbGF0ZXMgdXBkYXRlOmVtYWlsX3RlbXBsYXRlcyIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.GTGqI_wHVTD0x_KZKGV8rw6DaWYBdPafmxFSyG_ylVJ1M311g2mFPZJ63ojx4W19o64NMXbl4rU8w0i4erTyC4b2bL0lWrB441iIToLym_DhUFBnTpQsEHtOSsUBHSRpwXLQ6LLGEH6zMVptzYCv8-Zoa1LoMVgxi8HifGp37iCGt1hSl8oNO8g9vMb2xZxyT2ndN03LaRyJ9swGs8smT__DP9uktsgcDLu2stlo4eQAtVSsLHfeZHOQaZQXY_3FU7-7GQeT00aH7fluTDTpBvgKEYG1VeLYIIPg63arzVrG0OQiEYnDotifCJC6p_NbEPNrJgRCYMkX1kFNgxgPBg");
            IRestResponse response = client.Execute(requestGet);
            List<Testbruker> myobj = JsonConvert.DeserializeObject<List<Testbruker>>(response.Content);

            foreach(Testbruker bruker in myobj)
            {
                antallSign = bruker.user_metadata.antallSigneringer;
                id = bruker.user_id;
            }
            antallSign++;

            var clientPatch = new RestClient("https://document.eu.auth0.com/api/v2/users/" + id);

            var requestPatch = new RestRequest(Method.PATCH);
            requestPatch.AddHeader("content-type", "application/json");
            requestPatch.AddHeader("authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik9UUTJOelJDT1VRNVF6Y3pRakk1TnpReFFUTkZOMEkwTmtZMU56YzBOa1V3TVVFMlJVUXlSQSJ9.eyJpc3MiOiJodHRwczovL2RvY3VtZW50LmV1LmF1dGgwLmNvbS8iLCJzdWIiOiJKbGk5SU0wQXF1QTdYZWlDcW5pcmhPd0FYRmcxSDY4UUBjbGllbnRzIiwiYXVkIjoiaHR0cHM6Ly9kb2N1bWVudC5ldS5hdXRoMC5jb20vYXBpL3YyLyIsImlhdCI6MTUyMzU0MTk2NSwiZXhwIjoxMDUyMzU0MTk2NSwiYXpwIjoiSmxpOUlNMEFxdUE3WGVpQ3FuaXJoT3dBWEZnMUg2OFEiLCJzY29wZSI6InJlYWQ6Y2xpZW50X2dyYW50cyBjcmVhdGU6Y2xpZW50X2dyYW50cyBkZWxldGU6Y2xpZW50X2dyYW50cyB1cGRhdGU6Y2xpZW50X2dyYW50cyByZWFkOnVzZXJzIHVwZGF0ZTp1c2VycyBkZWxldGU6dXNlcnMgY3JlYXRlOnVzZXJzIHJlYWQ6dXNlcnNfYXBwX21ldGFkYXRhIHVwZGF0ZTp1c2Vyc19hcHBfbWV0YWRhdGEgZGVsZXRlOnVzZXJzX2FwcF9tZXRhZGF0YSBjcmVhdGU6dXNlcnNfYXBwX21ldGFkYXRhIGNyZWF0ZTp1c2VyX3RpY2tldHMgcmVhZDpjbGllbnRzIHVwZGF0ZTpjbGllbnRzIGRlbGV0ZTpjbGllbnRzIGNyZWF0ZTpjbGllbnRzIHJlYWQ6Y2xpZW50X2tleXMgdXBkYXRlOmNsaWVudF9rZXlzIGRlbGV0ZTpjbGllbnRfa2V5cyBjcmVhdGU6Y2xpZW50X2tleXMgcmVhZDpjb25uZWN0aW9ucyB1cGRhdGU6Y29ubmVjdGlvbnMgZGVsZXRlOmNvbm5lY3Rpb25zIGNyZWF0ZTpjb25uZWN0aW9ucyByZWFkOnJlc291cmNlX3NlcnZlcnMgdXBkYXRlOnJlc291cmNlX3NlcnZlcnMgZGVsZXRlOnJlc291cmNlX3NlcnZlcnMgY3JlYXRlOnJlc291cmNlX3NlcnZlcnMgcmVhZDpkZXZpY2VfY3JlZGVudGlhbHMgdXBkYXRlOmRldmljZV9jcmVkZW50aWFscyBkZWxldGU6ZGV2aWNlX2NyZWRlbnRpYWxzIGNyZWF0ZTpkZXZpY2VfY3JlZGVudGlhbHMgcmVhZDpydWxlcyB1cGRhdGU6cnVsZXMgZGVsZXRlOnJ1bGVzIGNyZWF0ZTpydWxlcyByZWFkOnJ1bGVzX2NvbmZpZ3MgdXBkYXRlOnJ1bGVzX2NvbmZpZ3MgZGVsZXRlOnJ1bGVzX2NvbmZpZ3MgcmVhZDplbWFpbF9wcm92aWRlciB1cGRhdGU6ZW1haWxfcHJvdmlkZXIgZGVsZXRlOmVtYWlsX3Byb3ZpZGVyIGNyZWF0ZTplbWFpbF9wcm92aWRlciBibGFja2xpc3Q6dG9rZW5zIHJlYWQ6c3RhdHMgcmVhZDp0ZW5hbnRfc2V0dGluZ3MgdXBkYXRlOnRlbmFudF9zZXR0aW5ncyByZWFkOmxvZ3MgcmVhZDpzaGllbGRzIGNyZWF0ZTpzaGllbGRzIGRlbGV0ZTpzaGllbGRzIHVwZGF0ZTp0cmlnZ2VycyByZWFkOnRyaWdnZXJzIHJlYWQ6Z3JhbnRzIGRlbGV0ZTpncmFudHMgcmVhZDpndWFyZGlhbl9mYWN0b3JzIHVwZGF0ZTpndWFyZGlhbl9mYWN0b3JzIHJlYWQ6Z3VhcmRpYW5fZW5yb2xsbWVudHMgZGVsZXRlOmd1YXJkaWFuX2Vucm9sbG1lbnRzIGNyZWF0ZTpndWFyZGlhbl9lbnJvbGxtZW50X3RpY2tldHMgcmVhZDp1c2VyX2lkcF90b2tlbnMgY3JlYXRlOnBhc3N3b3Jkc19jaGVja2luZ19qb2IgZGVsZXRlOnBhc3N3b3Jkc19jaGVja2luZ19qb2IgcmVhZDpjdXN0b21fZG9tYWlucyBkZWxldGU6Y3VzdG9tX2RvbWFpbnMgY3JlYXRlOmN1c3RvbV9kb21haW5zIHJlYWQ6ZW1haWxfdGVtcGxhdGVzIGNyZWF0ZTplbWFpbF90ZW1wbGF0ZXMgdXBkYXRlOmVtYWlsX3RlbXBsYXRlcyIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.Rr9Aza_GxIBBHBB1dbstPpFGrS_DFOIHQDIwY0fI1jyU0igQhDZxWBm9GQar5-MvCClWHoU9bOj5zz7NDRqKUMYIdXH99g3PyUld1B1jmIldDoe1nkim4vXg4VtU-YPW1ClpGBtHkoJsTlEOIYXGRQCp6bBkZmvsuJ-p0iceTe9SDQqHe4qod0T_8rCDjjR6kN5Tp14AdbkliQj97Vk23DVNF0eURUwT_8CS06cnSJ6XNfY3Q4cLLi6TwJQdVXhWRhrwNG5UHlFIwnCx1uiAsSDaMUYZQr_bVWqSUmJi7qSkQAKLVO2Q8_hQVa6g5SdID05Z5uMjOuH623jpSL7Vag");
            requestPatch.AddParameter("application/json", "{\"user_metadata\": {\"antallSigneringer\": \""+antallSign+"\"}}", ParameterType.RequestBody);

            IRestResponse respons = clientPatch.Execute(requestPatch);
        }

    }



}


