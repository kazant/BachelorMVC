using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BachelorMVC.Models
{
    public class Bruker
    {
        public string Email { get; set; }
        public string UserID { get; set; }
        public Usermetadata UserMetadata { get; set; }

    }
}
