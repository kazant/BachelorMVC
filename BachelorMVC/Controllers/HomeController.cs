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
using System.Xml;
using signicat.documentservicev3;

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


        public static void Sign()
        {
            List<document> documents = new List<document>();
            List<subject> subjects = new List<subject>();
            List<task> tasks = new List<task>();
            List<notification> notices = new List<notification>();

            var methods = new method[]
            {
                new method
                 {
                handwritten = false
                }
            };

            var signature = new signature[]

            {
                new signature {
                method = methods

            }
            };
         
                
            
            var documentaction = new documentaction[] 
            {
              new documentaction 
              {
                  document = (document) documents[0],
                  //eller: documentref = string
                  orderindex = 0,
                  type = documentactiontype.sign,
                  optional = false,
                  //send-result-to-archive = boolean. Kan evt sendes til Signicat Archive
              }  
            };

            var notifications = new notification[] 
            
            { 
                new notification {
                    type = notificationtype.EMAIL,
                    notificationid = "id",
                    recipient = "email",
                    //sender = string,
                    //header = for å endre subject på e-posten
                    message = "Klar for signering",
                    //schedule = new schedule (spesifiserer når varslingen skal sendes)
            }
            };
                //Dokument(er)
                //Import fra SDS
                //Legger til 1 dokument -> utvid med løkke
                documents.Add(new sdsdocument {
                    id = "static",
                    refsdsid = "staticsdsref",
                    description = "beskrivelse",
                    sendtoarchive = false
                    //valgfri:
                    //form = Form data,
                    //external-reference = Ekstra informasjon om lagret objekt
                    //sign-text-entry = Ekstra informasjon som blir presentert ved signering (string) 
                });
                
                //Subjekt(er)
                subjects.Add(new subject {
                    id = "navn",
                    //Valgfri
                    //nationalid = "fødselsnummer valgfri",
                    //first-name = "Olaf",
                    //last_name = "Hansen",
                    //mobile = "tlf",
                    //email = email,
                    //username = username
                    //attribute = new Attribute (En liste med andre attributter som beskriver subjektet)
                });

                //Oppgave(r)
                tasks.Add(new task {
                    id = "id",
                    subject = (subject) subjects[0],
                    //eller subject-ref=string
                    //days-to-live = integer,
                    //dialog = new Dialog (en melding som vises til endebruker når hun er i ferd med å fullføre oppgaven)
                    documentaction = documentaction,
                    notification = notifications,
                    signature = signature,
                    // eller AuthenticationBasedSignature,
                    //profile = string (grafisk profil),
                    //language = string,
                    //configuration = navn på en responsiv signaturflyt,
                    ontaskcomplete = "url tilbake",
                    ontaskcancel = "url hvis bruker avbryter",
                    //ontaskpostpone = "url hvis bruker utsetter",
                    //depends-on-task = (hvis denne tasken må vente på en annen instans av en task (signeringsrekkefølge)),
                    //bundle = boolean (hvis flere dokumenter skal behandles sammen som en "bundle")

                });

           
                //Profil(?)
                // -> Stringparameter

                //Språk(?)
                // -> Stringparameter (nb for bokmål, nn for nynorsk)
                
                //Notification

                //Klientreferanse (dokumentpartner)
                string klientref = "dokumentpartner-sign";
                
                //Sender av request(?)
                //new sender {
                    //unique-id,
                    //first-name,
                    //last-name
                //};
        }

        


        /* 

        
        public static void CallWebService()
{
    var _url = "https://preprod.signicat.com/ws/documentservice-v3?wsdl";
    var _action = "http://xxxxxxxx/Service1.asmx?op=HelloWorld";

    XmlDocument soapEnvelopeXml = CreateSoapEnvelope();
    HttpWebRequest webRequest = CreateWebRequest(_url, _action);
    InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

    // begin async call to web request.
    IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

    // suspend this thread until call is complete. You might want to
    // do something usefull here like update your UI.
    asyncResult.AsyncWaitHandle.WaitOne();

    // get the response from the completed web request.
    string soapResult;
    using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
    {
        using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
        {
            soapResult = rd.ReadToEnd();
        }
        Console.Write(soapResult);        
    }
}

private static HttpWebRequest CreateWebRequest(string url, string action)
{
    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
    webRequest.Headers.Add("SOAPAction", action);
    webRequest.ContentType = "text/xml;charset=\"utf-8\"";
    webRequest.Accept = "text/xml";
    webRequest.Method = "POST";
    return webRequest;
}

private static XmlDocument CreateSoapEnvelope()
{
    XmlDocument soapEnvelopeDocument = new XmlDocument();
    soapEnvelopeDocument.LoadXml(@"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/1999/XMLSchema""><SOAP-ENV:Body><HelloWorld xmlns=""http://tempuri.org/"" SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><int1 xsi:type=""xsd:integer"">12</int1><int2 xsi:type=""xsd:integer"">32</int2></HelloWorld></SOAP-ENV:Body></SOAP-ENV:Envelope>");
    return soapEnvelopeDocument;
}

private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
{
    using (Stream stream = webRequest.GetRequestStream())
    {
        soapEnvelopeXml.Save(stream);
    }
}

         public async void UploadToSDS() 
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

                // For testing av respons
                using(StreamWriter sw = System.IO.File.CreateText("Controllers/output.txt"))
                {
                    sw.WriteLine(documentId);
                }
                
            }

            // Rut bruker til signicat
            Dokumenter dokument = new Dokumenter
            {
                dokumentID = "dokumentid",
                dokumentIDFraSDS = "fraSDS",
                Name = "navn",
                signert = false,
                beskrivelse = "beskrivelse"

            };

            var request = new CreateRequest
            {
                password = "Bond007",
                service = "demo",
                new request {
                    klientReferanse = "klientreferanse",
                    language = "nb",
                    profil = "profil",
                    dokument = dokument
                
                }
            };
                
        

        }
        */
    }
}
