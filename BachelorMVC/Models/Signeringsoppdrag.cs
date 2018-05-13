using BachelorMVC.Models;
using System.Collections.Generic;

namespace BachelorMVC.Models
{
    public class Signeringsoppdrag
    {
        public Bruker oppretter { get; set; }
        public Testbruker[] signatører { get; set;}
        public Dokument dokument { get; set;}
        public string test {get; set;}
    }
}