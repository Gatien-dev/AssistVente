using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Vente
    {
        public Guid ID { get; set; }
        public Guid ClientId { get; set; }
        public virtual Client Client { get; set; }
        public Guid VendeurId { get; set; }
        public Guid ProduitId { get; set; }
        public virtual Produit Produit { get; set; }
        public Guid MagasinSortieId { get; set; }
        public virtual Magasin Magasin { get; set; }
        
    }
}