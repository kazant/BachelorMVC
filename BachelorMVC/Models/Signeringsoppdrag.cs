using System.Collections.Generic;
using BachelorMVC.Models;

namespace BachelorMVC.Models {
    public class Signeringsoppdrag {
        public Testbruker oppretter { get; set; }
        public Testbruker[] signatører { get; set; }
        public Dokument dokument { get; set; }

    }
}