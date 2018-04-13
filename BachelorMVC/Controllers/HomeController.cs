﻿using System;
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


    }

    }


