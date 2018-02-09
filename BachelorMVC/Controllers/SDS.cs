//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
 
namespace Signicat.Support.SDS.Test
{
    //[TestClass]
    public class SDSTest
    {
        //[TestMethod]
        public async Task Uploading_a_PDF_document_to_SDS()
        {
            var httpClientHandler = new HttpClientHandler { Credentials = new NetworkCredential("demo", "Bond007") };
            using (var client = new HttpClient(httpClientHandler))
            {
                HttpContent content = new ByteArrayContent(File.ReadAllBytes("mydocument.pdf"));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                HttpResponseMessage response =
                    await client.PostAsync("https://preprod.signicat.com/doc/demo/sds", content);
                string documentId = await response.Content.ReadAsStringAsync();
 
                //Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                //Assert.IsTrue(documentId.Length > 0);
            }
        }
    }
}