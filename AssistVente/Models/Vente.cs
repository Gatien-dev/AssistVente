using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Vente
    {
        public Guid ID { get; set; }
        public Guid ClientId { get; set; }
        public virtual Client Client { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public double Montant { get; set; }
        
    }
    public class DetailVente
    {
        public Guid ID { get; set; }
        [ForeignKey("Ventes")]
        public Guid VenteId { get; set; }
        [ForeignKey("Produits")]
        public Guid ProduitID { get; set; }
        public virtual Vente Vente { get; set; }
        public double QuantiteVendue { get; set; }
        public double QuantiteLivree { get; set; }
        
    }
}