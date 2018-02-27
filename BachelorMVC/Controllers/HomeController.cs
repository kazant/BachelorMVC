﻿using Assently.Client;
using Assently.ServiceModel;
using Assently.ServiceModel.Messages;
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
using System.Xml;

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

        public IActionResult test()
        {
            

            var client = new AssentlyClient("https://test.assently.com", "1ab291ce-7486-488a-a5dc-de81ae692eae", "E76l9Vt91QiU6AJTZPX4vzXXjloVWpVa4vib4mio");

            CreateCaseModel model = new CreateCaseModel();
            model.Id = Guid.NewGuid();
            Console.WriteLine(model.Id);
            model.AllowedSignatureTypes.Add(SignatureType.ElectronicId);
            model.SendSignRequestEmailToParties = true;
            model.SendFinishEmailToParties = true;
            model.Name = "sign me pl0x";
            model.NameAlias = "testAlias";

            model.Parties.Add(new PartyModel
            {
                EmailAddress = "ErlendAHall@gmail.com",
                Name = "Erlend Andreas Hall"
            });
           

            model.Documents.Add("Controllers\\vedlegg1.pdf");
            model.Metadata.Add("test","test");
            client.CreateCase(model);
            client.SendCase(model.Id);
           
            CaseModel caseModel = client.GetCase(model.Id);

            if (caseModel.Status == CaseStatus.Sent)
            {

                var receipt = caseModel.Documents.Where(d => d.Type == DocumentType.Original).Single();
                client.GetDocumentData(Guid.Parse(receipt.Id), "Controllers\\test");
            }


            return View();
        }
        
        }

    }



