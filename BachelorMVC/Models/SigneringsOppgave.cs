using signicat.documentservicev3;

namespace BachelorMVC.Models
{
    public class SigneringsOppgave 
    {
        public string id {get; set;}
        public string subjektRef {get; set;}
        public bool bundleSpecified {get; set;}
        public bool bundle {get; set;}
        public documentactiontype type {get; set;}
    }
}