using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssistVente.Models.ViewModels
{
    public class AchatCreateVM
    {
        public string Fournisseur{ get; set; }
        public List<DetailAchatVM> Details { get; set; }
        [Display(Name = "Montant payé")]
        public double MontantPaye { get; set; }
        [Display(Name ="Numéro de la facture")]
        public string NumFactureFournisseur { get; set; }
    }
    public class DetailAchatVM
    {
        public Guid ProduitId { get; set; }
        public string NomProduit { get; set; }
        public double Prix { get; set; }
        public double Quantite { get; set; }
    }
}