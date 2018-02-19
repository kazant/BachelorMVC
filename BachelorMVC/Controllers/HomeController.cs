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
            List<request> requests = new List<request>();

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

                requests.Add
                (
                    new request 
                        {
                            //profile,
                            //language,
                            subject = subjects.ToArray(),
                            document = documents.ToArray(),
                            task = tasks.ToArray(),
                            clientreference = klientref,
                            //sender = Senderobjekt,
                            //notification = Notificationobjekt,
                            //daysuntildeletion = int,
                            //account = string 
                            //packagingtask = //
                        }
                );

                var create = new createrequestrequest
                {
                    password = "Bond007",
                    service = "demo",
                    request = requests.ToArray()
                };
               
               var request = new createRequest(create);

               
                
     
                createrequestresponse response;
                createRequestResponse1 response1;

                using (var client = new signicat.documentservicev3.DocumentEndPointClient())
                {
                    response = client.createRequest(create);
                }
                
                
                
                
                
                 

                
               
                
        }

    }


}
