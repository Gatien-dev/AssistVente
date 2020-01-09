using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class GroupeAbonnements
    {
        public Guid Id { get; set; }
        [Display(Name="Désignation")]
        public string Name { get; set; }
        public List<Abonnement> Abonnements { get; set; }
        public string Description { get; set; }
        [Display(Name="Date de Création")]
        public DateTime DateCreation { get; set; }
    }
}