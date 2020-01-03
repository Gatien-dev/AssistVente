using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Location:Operation
    {
        [Display(Name ="Produit")]
        public Guid ProduitId { get; set; }
        [Display(Name ="Produit")]
        public virtual Produit Produit { get; set; }
        [Display(Name ="Date de Location")]
        [DataType(DataType.Date)]
        public DateTime DateLocation { get; set; }
        [DataType(DataType.Date)]
        [Display(Name ="Date de Retour")]
        public DateTime DateFinLocation { get; set; }
        //En cas de suspension momentanee de la location
        //public DateTime DateSuspensionLocation { get; set; }
        //En cas d'arret de la location avant l'echeance
        [Display(Name ="Arret")]
        [DataType(DataType.Date)]
        public DateTime DateArretLocation { get; set; }
        //Indique si le stock alloue est rendu
        public bool LocationRendue { get; set; }
        [Display(Name ="Qte Prise")]
        public double QuantitePrise { get; set; }
        [Display(Name ="Qte Rendue")]
        public double QuantiteRendue { get; set; }

    }
}