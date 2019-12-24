using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Reglement
    {
        public Guid ID { get; set; }
        public Guid IdVente { get; set; }
        public double MontantRegle { get; set; }
        public double MontantRecu { get; set; }
        public double MontantRendu { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
    }
}