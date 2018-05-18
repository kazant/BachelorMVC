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
        public string HttpHeaderValue = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik9UUTJOelJDT1VRNVF6Y3pRakk1TnpReFFUTkZOMEkwTmt" +
                "ZMU56YzBOa1V3TVVFMlJVUXlSQSJ9.eyJpc3MiOiJodHRwczovL2RvY3VtZW50LmV1LmF1dGgwLmNvbS8iLCJzdWIiOiJKbGk5SU0wQXF1QTdYZWlDcW5pcmhPd0FYRmcxSDY" +
                "4UUBjbGllbnRzIiwiYXVkIjoiaHR0cHM6Ly9kb2N1bWVudC5ldS5hdXRoMC5jb20vYXBpL3YyLyIsImlhdCI6MTUyMzYxMjg1MiwiZXhwIjoxMDUyMzYxMjg1MiwiYXpwIjoiSmx" +
                "pOUlNMEFxdUE3WGVpQ3FuaXJoT3dBWEZnMUg2OFEiLCJzY29wZSI6InJlYWQ6Y2xpZW50X2dyYW50cyBjcmVhdGU6Y2xpZW50X2dyYW50cyBkZWxldGU6Y2xpZW50X2dyYW50cyB1cG" +
                "RhdGU6Y2xpZW50X2dyYW50cyByZWFkOnVzZXJzIHVwZGF0ZTp1c2VycyBkZWxldGU6dXNlcnMgY3JlYXRlOnVzZXJzIHJlYWQ6dXNlcnNfYXBwX21ldGFkYXRhIHVwZGF0ZTp1c2Vyc19hc" +
                "HBfbWV0YWRhdGEgZGVsZXRlOnVzZXJzX2FwcF9tZXRhZGF0YSBjcmVhdGU6dXNlcnNfYXBwX21ldGFkYXRhIGNyZWF0ZTp1c2VyX3RpY2tldHMgcmVhZDpjbGllbnRzIHVwZGF0ZTpjbGllbnR" +
                "zIGRlbGV0ZTpjbGllbnRzIGNyZWF0ZTpjbGllbnRzIHJlYWQ6Y2xpZW50X2tleXMgdXBkYXRlOmNsaWVudF9rZXlzIGRlbGV0ZTpjbGllbnRfa2V5cyBjcmVhdGU6Y2xpZW50X2tleXMgcmVhZDpjb" +
                "25uZWN0aW9ucyB1cGRhdGU6Y29ubmVjdGlvbnMgZGVsZXRlOmNvbm5lY3Rpb25zIGNyZWF0ZTpjb25uZWN0aW9ucyByZWFkOnJlc291cmNlX3NlcnZlcnMgdXBkYXRlOnJlc291cmNlX3NlcnZlcnMgZGV" +
                "sZXRlOnJlc291cmNlX3NlcnZlcnMgY3JlYXRlOnJlc291cmNlX3NlcnZlcnMgcmVhZDpkZXZpY2VfY3JlZGVudGlhbHMgdXBkYXRlOmRldmljZV9jcmVkZW50aWFscyBkZWxldGU6ZGV2aWNlX2NyZWRlbnRpY" +
                "WxzIGNyZWF0ZTpkZXZpY2VfY3JlZGVudGlhbHMgcmVhZDpydWxlcyB1cGRhdGU6cnVsZXMgZGVsZXRlOnJ1bGVzIGNyZWF0ZTpydWxlcyByZWFkOnJ1bGVzX2NvbmZpZ3MgdXBkYXRlOnJ1bGVzX2NvbmZpZ3MgZGV" +
                "sZXRlOnJ1bGVzX2NvbmZpZ3MgcmVhZDplbWFpbF9wcm92aWRlciB1cGRhdGU6ZW1haWxfcHJvdmlkZXIgZGVsZXRlOmVtYWlsX3Byb3ZpZGVyIGNyZWF0ZTplbWFpbF9wcm92aWRlciBibGFja2xpc3Q6dG9rZW5zIHJlY" +
                "WQ6c3RhdHMgcmVhZDp0ZW5hbnRfc2V0dGluZ3MgdXBkYXRlOnRlbmFudF9zZXR0aW5ncyByZWFkOmxvZ3MgcmVhZDpzaGllbGRzIGNyZWF0ZTpzaGllbGRzIGRlbGV0ZTpzaGllbGRzIHVwZGF0ZTp0cmlnZ2VycyByZWFkOn" +
                "RyaWdnZXJzIHJlYWQ6Z3JhbnRzIGRlbGV0ZTpncmFudHMgcmVhZDpndWFyZGlhbl9mYWN0b3JzIHVwZGF0ZTpndWFyZGlhbl9mYWN0b3JzIHJlYWQ6Z3VhcmRpYW5fZW5yb2xsbWVudHMgZGVsZXRlOmd1YXJkaWFuX2Vucm9sbG1" +
                "lbnRzIGNyZWF0ZTpndWFyZGlhbl9lbnJvbGxtZW50X3RpY2tldHMgcmVhZDp1c2VyX2lkcF90b2tlbnMgY3JlYXRlOnBhc3N3b3Jkc19jaGVja2luZ19qb2IgZGVsZXRlOnBhc3N3b3Jkc19jaGVja2luZ19qb2IgcmVhZDpjdXN0b21fZ" +
                "G9tYWlucyBkZWxldGU6Y3VzdG9tX2RvbWFpbnMgY3JlYXRlOmN1c3RvbV9kb21haW5zIHJlYWQ6ZW1haWxfdGVtcGxhdGVzIGNyZWF0ZTplbWFpbF90ZW1wbGF0ZXMgdXBkYXRlOmVtYWlsX3RlbXBsYXRlcyIsImd0eSI6ImNsaWVudC1jcmV" +
                "kZW50aWFscyJ9.GTGqI_wHVTD0x_KZKGV8rw6DaWYBdPafmxFSyG_ylVJ1M311g2mFPZJ63ojx4W19o64NMXbl4rU8w0i4erTyC4b2bL0lWrB441iIToLym_DhUFBnTpQsEHtOSsUBHSRpwXLQ6LLGEH6zMVptzYCv8-Zoa1LoMVgxi8HifGp37iCGt" +
                "1hSl8oNO8g9vMb2xZxyT2ndN03LaRyJ9swGs8smT__DP9uktsgcDLu2stlo4eQAtVSsLHfeZHOQaZQXY_3FU7-7GQeT00aH7fluTDTpBvgKEYG1VeLYIIPg63arzVrG0OQiEYnDotifCJC6p_NbEPNrJgRCYMkX1kFNgxgPBg";

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
            return View();
        }

        public IActionResult InspiserDokument() {

            return View();
        }

       
        public void OpprettCaseOgSendEpost(string epost, string caseNavn, string dokumentNavn, string signeringsmetode)
        {

            string[] Invitasjonsemails;

            if(!(epost == null))
            {
                Invitasjonsemails = epost.Split(',');
            } else
            {
                return; 
            }
            
            var client = new AssentlyClient("https://test.assently.com", "1ab291ce-7486-488a-a5dc-de81ae692eae", "OMw4uXqu1QgCX_ESA8XpI00Z7EKyIlypwgrlv-qu");

            //En CreateCaseModel skal bestå av et dokument, en eller flere brukere og annen info
            CreateCaseModel model = new CreateCaseModel();
            List<CaseEvent> events = new List<CaseEvent>();
            events.Add(CaseEvent.SignatureAdded);
            //Påkrevd
            model.Id = Guid.NewGuid();
            model.SendSignRequestEmailToParties = true;
            model.SendFinishEmailToParties = true;
            model.SendFinishEmailToCreator = true;
            model.Name = caseNavn;
            model.NameAlias = "TestAlias";
            model.EventCallback = new CaseEventSubscription {
                Events = events,
                Url = "http://158.36.13.131:52817/DBController/WriteNewSignature"
            };

            //Kan gi valg mellom eID signatur eller signbyhand (på mobil). Påkrevd
            if(signeringsmetode == "electronicid") {
                model.AllowedSignatureTypes.Add(SignatureType.ElectronicId);
            } else {
                model.AllowedSignatureTypes.Add(SignatureType.Touch);
            }

            //PartyModel er en samling brukere. Påkrevd.
            //Skal flere brukere signere ett dokument, må denne kodebiten gjentas.
            for (var i = 0; i < Invitasjonsemails.Length - 1; i++)
            {

                model.Parties.Add(new PartyModel
                {
                    Name = "Erlend",
                    EmailAddress = Invitasjonsemails[i]
                });
            }
            
            //En eller flere dokumenter angis til en Liste med dokumenter
            model.Documents.Add("./Persistence/Dokumenter/" + dokumentNavn);
            model.Metadata.Add("nøkkel","verdi");

            using (var fileStream = new FileStream("./Persistence/guid.txt", FileMode.Create)) {
                byte[] data = model.Id.ToByteArray();
                fileStream.Write(data, 0, data.Length);
            }

            string oppretterEmail = User.Claims.Where(c => c.Type == "name").FirstOrDefault().Value;
            DBController.WriteDocument(model.Id, dokumentNavn, Invitasjonsemails.Length, caseNavn, oppretterEmail);
            DBController.WriteKunde(Invitasjonsemails);
            
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
            requestGet.AddHeader("authorization",  HttpHeaderValue);
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
            requestPatch.AddHeader("authorization", HttpHeaderValue);
            requestPatch.AddParameter("application/json", "{\"user_metadata\": {\"antallSigneringer\": \""+antallSign+"\"}}", ParameterType.RequestBody);

            IRestResponse respons = clientPatch.Execute(requestPatch);
        }

        

    }



}


