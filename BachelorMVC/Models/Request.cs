using System.ComponentModel.DataAnnotations;

namespace BachelorMVC.Models
{
    public class Request
    {
        [Key]
        public string klientReferanse {get; set;}

        public string language {get; set;}
        public string profil {get; set;}
        public Dokumenter sdsDokument {get; set;}
    }
}