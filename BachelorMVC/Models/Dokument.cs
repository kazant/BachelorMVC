using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BachelorMVC.Models
{
    public class Dokument
    {
        public int AntallSignaturer { get; set; }
        public string Filnavn { get; set; }
        public string Id { get; set; }
    }
}
