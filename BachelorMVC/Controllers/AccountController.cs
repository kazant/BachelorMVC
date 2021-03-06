﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BachelorMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using Claim = System.Security.Claims.Claim;

namespace BachelorMVC.Controllers
{

    public class AccountController : Controller
    {
        private String autString = "";
        private DBController DBController = new DBController();
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
        // The Authorize attribute requires the user to be authenticated and will
        // kick off the OIDC authentication flow 
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult LoginForm()
        {

            /* Når bruker logger inn sjekker vi 'Roles', og dirigerer deretter */
            foreach (Claim claim in User.Claims)
            {
                if (claim.Value == "admin")
                {
                    return RedirectToAction("AdminFormPage");
                }
                else if (claim.Value == "user")
                {
                    return RedirectToAction("DokumentOversikt");
                }

            }

            return RedirectToAction("Index", "Home");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult AdminFormPage()
        {

            if (SjekkAutentisering() == "admin")
            {
                return View();
            }

            return View("NotAuthorized");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult LagOppdrag()
        {
            if (SjekkAutentisering() == "user")
            {
                return View();
            }

            return View("NotAuthorized");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult DokumentOversikt()
        {

            string epost = User.Claims.Where(c => c.Type == "name").FirstOrDefault().Value;
            List<Signeringsoppdrag> oppdrag = DBController.OppdragModellerer(epost);

            if (SjekkAutentisering() == "user")
            {
                return View("DokumentOversikt", oppdrag);
            }

            return View("NotAuthorized");
        }

        //Henter alle brukere
        public JsonResult getAlleBrukere()
        {
            var client = new RestClient(" https://document.eu.auth0.com/api/v2/users?q=user_metadata%3Anickname%3D%221%22&search_engine=v2");
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", HttpHeaderValue);
            IRestResponse response = client.Execute(request);
            List<Bruker> myobj = JsonConvert.DeserializeObject<List<Bruker>>(response.Content);
            return Json(response.Content);

        }

        //Henter JSON Resultat utifra spørring (nickname = 0)
        public JsonResult GetIkkeGodkjenteKunder()
        {
            var client = new RestClient(" https://document.eu.auth0.com/api/v2/users?q=user_metadata%3Anickname%3D%220%22&search_engine=v2");
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", HttpHeaderValue);
            IRestResponse response = client.Execute(request);
            List<Bruker> myobj = JsonConvert.DeserializeObject<List<Bruker>>(response.Content);
            return Json(response.Content);
        }

        //ActionResult for UserList view'et
        [Microsoft.AspNetCore.Authorization.Authorize]
        public ActionResult AdminUserListForm()
        {
            if (SjekkAutentisering() == "admin")
            {
                JsonResult myobjs = GetIkkeGodkjenteKunder();
                List<Bruker> myobj = JsonConvert.DeserializeObject<List<Bruker>>(myobjs.Value.ToString());
                return View("AdminUserListForm", myobj);
            }

            return View("NotAuthorized");
        }

        //ActionResult for Alle Brukere view
        public ActionResult AdminAlleBrukere()
        {

            if (SjekkAutentisering() == "admin")
            {
                JsonResult myobjs = getAlleBrukere();
                List<Bruker> myobj = JsonConvert.DeserializeObject<List<Bruker>>(myobjs.Value.ToString());
                return View("AdminAlleBrukere", myobj);

            }

            return View("NotAuthorized");

        }

        //Setter godkjent (Nickname = 1) på brukere
        public async Task<ActionResult> SetGodkjent(string id)
        {
            string parameter = "{\"user_metadata\": {\"nickname\": \"1\"}}";
            PatchAuth0(id, parameter);
            DBController.SetGodkjent(id);
            await Task.Run(() => waitTimer());

            return RedirectToAction("AdminUserListForm");
        }

        //Sletter valgt bruker
        public async Task<ActionResult> DeleteUser(string id, string view)
        {
            //gi verdi til nickname
            var client = new RestClient("https://document.eu.auth0.com/api/v2/users/" + id);
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", HttpHeaderValue);
            IRestResponse response = client.Execute(request);
            DBController.SlettOppretter(id);
            await Task.Run(() => waitTimer());

            return RedirectToAction(view);
        }

        //Metode for å oppdatere firma
        [HttpPost]
        public async Task<ActionResult> OppdaterFirma(string idBruker, string firmaNavn)
        {
            string parameter = "{\"user_metadata\": {\"Firma\": \"" + firmaNavn + "\"}}";
            PatchAuth0(idBruker, parameter);
            await Task.Run(() => waitTimer());

            return RedirectToAction("AdminAlleBrukere");
        }

        //Metode for patching (type oppdatering som parameter med id på bruker)
        public async void PatchAuth0(string idBruker, string parameter)
        {
            var client = new RestClient("https://document.eu.auth0.com/api/v2/users/" + idBruker);
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", HttpHeaderValue);
            request.AddParameter("application/json", parameter, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
        //Bruker for å sette en await (auth0 er treig)
        public void waitTimer()
        {
            Thread.Sleep(2000);
        }

        public String SjekkAutentisering()
        {

            foreach (Claim claim in User.Claims)
            {
                string rolle = User.Claims.FirstOrDefault(c => c.Type == "https://example.com/roles")?.Value;

                if (rolle == "admin")
                {
                    autString = "admin";
                }
                else if (rolle == "user")
                {
                    autString = "user";
                }
            }

            return autString;
        }

        public IActionResult LogoutAuth0()
        {
            //Finnes sikkert noen bedre måte. Dette er bare en quick-fix som vi kan evt endre på senere
            /*foreach (var cookie in Request.Cookies.Keys) {
                Response.Cookies.Delete (cookie);
            }

            return RedirectToAction ("Index", "Home");*/

            //valid URL eller server IP
            string returnTo = "158.36.13.133:52817";
            return Redirect("https://document.eu.auth0.com/v2/logout?returnTo=http%3A%2F%2Fwww.example.com");
        }

    }

}