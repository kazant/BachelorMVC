using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Signicat.Support.SDS.Test
{

public class Signicat 
{

    public void How_to_create_a_simple_document_order_with_one_subject_and_one_document_using_Danish_NemID()
{
    // The document id is what you get in response when uploading a document to the SDS
    string documentId = "04092013551868wie4tdlw9n8e6s834f3iwm92yq5i8d3gkgqit3vpm6ed";
    var request = new createrequestrequest
    {
        password = "Bond007",
        service = "demo",


        request = new request[]
         {
             new request
             {
                 clientreference = "klient-referanse",
                 language = "nb",
                 profile = "demo",
                 document = new document[]
                 {
                     new sdsdocument
                     {
                         id = "dokumentid",
                         refsdsid = documentId,
                         description = "Terms and conditions"
                     }
                 },
                 subject = new subject[]
                 {
                     new subject
                     {
                         id = "subjektid",
                         nationalid = "1909740939"
                     }
                 },
                 task = new task[]
                 {
                     new task
                     {
                         id = "task_1",
                         subjectref = "subjektid",
                         bundleSpecified = true,
                         bundle = false,
                         documentaction = new documentaction[]
                         {
                              new documentaction
                              {
                                  type = documentactiontype.sign,
                                  documentref = "dokumentid"
                              }
                         },
                         signature = new signature[]
                         {
                             new signature
                             {
                                 responsiveSpecified=true,
                                 responsive = true,
                                 method = new method[]
                                 {
                                     new method
                                         {
                                            value = "nemid-sign"
                                         }
                                 }
                             }
                         }
                     }
                 }
             }
         }
    };
    createrequestresponse response;
    using (var client = new DocumentEndPointClient())
    {
        response = client.createRequest(request);
    }
    String signHereUrl =
        String.Format("https://preprod.signicat.com/std/docaction/demo?request_id={0}&task_id={1}", response.requestid[0], request.request[0].task[0].id);

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
                HttpContent content = new ByteArrayContent(File.ReadAllBytes("vedlegg1.pdf"));
                
                // Mulighet for flere filtyper og validering
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                
                // Dokumentet blir lastet opp til demo-SDS og responsen lagres for videre bruk
                HttpResponseMessage response = 
                    await client.PostAsync("https://preprod.signicat.com/doc/demo/sds", content);

                string documentId = await response.Content.ReadAsStringAsync();
            }
        

        }
    }
}