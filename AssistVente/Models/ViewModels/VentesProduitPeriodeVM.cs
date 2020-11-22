using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssistVente.Models.ViewModels
{
    public class VenteProduitPeriodeVM
    {
        public int ID { get; set; }
        public Vente Vente { get; set; }
        [Display(Name ="Quantité")]
        public double QteVendueProduit { get; set; }
        public Produit Produit { get; set; }
        public  Client Client { get; set; }
        public double Montant { get; set; }
        public double Prix { get; set; }
        public DateTime Date { get; set; }
    }
}