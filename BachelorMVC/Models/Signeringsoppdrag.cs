using System.Collections.Generic;
using BachelorMVC.Models;

namespace BachelorMVC.Models
{
    public class Signeringsoppdrag
    {
        public Bruker Oppretter { get; set; }
        public Bruker[] Signatører { get; set; }
        public Dokument Dokument { get; set; }
    }
}