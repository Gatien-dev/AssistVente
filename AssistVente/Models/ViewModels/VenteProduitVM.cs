using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssistVente.Models.ViewModels
{
    public class VenteProduitVM
    {
        public Guid IDProduit { get; set; }
        [Display(Name ="Nom")]
        public string NomProduit { get; set; }
        [Display(Name = "P.U")]
        public double PrixVente { get; set; }
        [Display(Name = "Quantité")]
        public double QteVendue { get; set; }
        [Display(Name = "Total")]
        public double TotalVente { get; set; }
    }
}