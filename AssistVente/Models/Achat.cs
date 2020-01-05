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
        public virtual List<DetailAchat> Details { get; set; }
    }

    public class DetailAchat
    {
        public Guid ID { get; set; }
        [ForeignKey("Achat")]
        public Guid AchatId { get; set; }
        public virtual Achat Achat { get; set; }
        [ForeignKey("Produit")]
        public Guid ProduitID { get; set; }
        public virtual Produit Produit { get; set; }
        public double PrixAchat { get; set; }
        public double QuantiteAchetee { get; set; }
       // public double QuantiteLivree { get; set; }

    }
}