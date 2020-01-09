using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Abonnement:Operation
    {

        //public Guid ClientId{ get; set; }
        //public virtual Client Client { get; set; }
        public Guid ForfaitId { get; set; }
        public virtual Forfait Forfait { get; set; }
        [Display(Name ="Début")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }
        [Display(Name ="Fin")]
        [DataType(DataType.Date)]
        public DateTime DateFin { get; set; }
        [Display(Name ="Date de Suspension")]
        public DateTime DateSuspension { get; set; }
        [Display(Name ="Suspendu?")]
        public bool Suspendu { get; set; }
        public bool Termine { get; set; }
        [Display(Name = "Montant restant")]
        public double ResteAPayer { get; set; } = 0;
        [Display(Name = "Montant payé")]
        public double SommePaye { get; set; } = 0;
        public GroupeAbonnements Groupe { get; set; }
        [ForeignKey("Groupe")]
        public virtual Guid GroupeAbonnementsId { get; set; }
    }
    public class Forfait
    {
        public Guid Id { get; set; }
        public String Nom { get; set; }
        public string Description { get; set; }
        [Display(Name="Durée (Jours)")]
        public int Duree { get; set; }
        public List<Abonnement> Abonnements { get; set; }
        public double Montant { get; set; }
    }
}
