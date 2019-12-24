using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Produit
    {
        public Guid ID { get; set; }
        public string Nom { get; set; }
        public double PrixAchat { get; set; }
        public double PrixVente { get; set; }
        public bool ALouer { get; set; }
        public double StockDisponible { get; set; }
        public TimeSpan DureeDeLocationParDefaut { get; set; }
        public string Description { get; set; }
        public DateTime DateCreation { get; set; }
        public string CreatorId { get; set; }

    }
}