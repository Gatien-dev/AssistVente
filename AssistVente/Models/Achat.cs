using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Achat
    {
        public Guid ID { get; set; }
        public string Fournisseur { get; set; }
        public string NumFacture { get; set; }

        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public double Montant { get; set; }
    }
    public class DetailAchat
    {
        public Guid ID { get; set; }
        [ForeignKey("Achats")]
        public Guid VenteId { get; set; }
        [ForeignKey("Produits")]
        public Guid ProduitID { get; set; }
        public virtual Vente vente { get; set; }
        public virtual Vente Vente { get; set; }
        public double QuantiteVendue { get; set; }
        public double QuantiteLivree { get; set; }

    }
}