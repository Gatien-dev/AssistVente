using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Reglement
    {
        public Guid Id { get; set; }
        public Guid CaisseId { get; set; }
        public Caisse Caisse { get; set; }
        public Guid IdOperation { get; set; }
        public DateTime Date { get; set; }
        public double MontantRegle { get; set; }
        public double MontantRecu { get; set; }
        public double MontantRendu { get; set; }

    }
}