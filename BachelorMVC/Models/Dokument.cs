using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BachelorMVC.Models
{
    public class Dokument
    {
        public int antallSignaturer {get; set;}
        public string filnavn { get; set; }
        public string Id { get; set; }
    }
}
