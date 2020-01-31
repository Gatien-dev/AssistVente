using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name ="Par défaut")]
        public bool ParDefaut { get; set; }
        public List<ReinitialisationCaisse> Reinitialisations { get; set; }
    }
    public class ReinitialisationCaisse
    {
        public Guid Id { get; set; }
        public Guid CaisseId { get; set; }
        public virtual Caisse Caisse { get; set; }
        [Display(Name ="Ancien Solde")]
        public double AncienSolde { get; set; }
        [Display(Name ="Nouveau Solde")]
        public double NouveauSolde { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
    }

    //TODO: Autoriser la modification de la caisse uniquement par un admin
    //TODO: Et penser aux autorisations sur les autres modules
}