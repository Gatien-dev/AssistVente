using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Caisse
    {
        public Guid ID { get; set; }
        public List<Operation> Operations { get; set; }
        public double Solde { get; set; }
    }
}