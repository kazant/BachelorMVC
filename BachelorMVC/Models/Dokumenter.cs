using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BachelorMVC.Models
{
    public class Dokumenter
    {

        [Key]
        public int DokumentID { get; set; }
        
        public string dokumentIDFraSDS {get; set;}
        public string Name { get; set; }
        public bool signert { get; set; }
        public string beskrivelse {get; set;}
    }
}
