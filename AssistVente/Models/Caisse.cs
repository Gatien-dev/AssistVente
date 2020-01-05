using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Caisse
    {
        public Guid ID { get; set; }
        public string Nom { get; set; }
        public List<Operation> Operations { get; set; }
        public List<Reglement> Reglements { get; set; }
        public double Solde { get; set; }
    }

    //TODO: Autoriser la modification de la caisse uniquement par un admin
    //TODO: Et penser aux autorisations su les autres modules
}