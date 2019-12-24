using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Achat:Operation
    {

        public string Fournisseur { get; set; }
        public string NumFacture { get; set; }

    }
    public class DetailAchat
    {
        public Guid ID { get; set; }
        [ForeignKey("Achats")]
        public Guid AchatId { get; set; }
        public virtual Achat Achat { get; set; }
        [ForeignKey("Produits")]
        public Guid ProduitID { get; set; }
        public virtual Produit Produit { get; set; }
        public double PrixAchat { get; set; }
        public double QuantiteAchetee { get; set; }
       // public double QuantiteLivree { get; set; }

    }
}