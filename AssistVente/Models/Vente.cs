using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Vente:Operation
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NumVente { get; set; }
        [Display(Name ="Montant Réglé")]
        public double MontantRegle { get; set; }
        [Display(Name = "Reste à payer")]
        public double MontantRestant { get; set; }
        [Display(Name ="Date d'opération")]
        public DateTime DateOperation { get; set; }
        public List<DetailVente> Details { get; set; }

    }
    public class DetailVente
    {
        public Guid ID { get; set; }
        public Guid? VenteId { get; set; }
        public virtual Vente Vente { get; set; }
        public Guid ProduitID { get; set; }
        public virtual Produit Produit { get; set; }
        public double QuantiteVendue { get; set; }
        //public double QuantiteLivree { get; set; }

    }
}
