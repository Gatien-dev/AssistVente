using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Reglement:Operation
    {

        public Guid IdVente { get; set; }
        public double MontantRegle { get; set; }
        public double MontantRecu { get; set; }
        public double MontantRendu { get; set; }
        
    }
}