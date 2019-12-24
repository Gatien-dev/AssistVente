using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Abonnement
    {
        public Guid Id { get; set; }
        public Guid ClientId{ get; set; }
        public virtual Client Client { get; set; }
        public Guid ForfaitId { get; set; }
        public virtual Forfait Forfait { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public DateTime DateSuspension { get; set; }
        public DateTime Suspendu { get; set; }
    }
    public class Forfait
    {
        public Guid Id { get; set; }
        public String Nom { get; set; }
        public TimeSpan Duree { get; set; }
        public List<Abonnement> Abonnements { get; set; }
    }
}