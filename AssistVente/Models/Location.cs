using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Location:Operation
    {

        public Guid ProduitId { get; set; }
        public virtual Produit Produit { get; set; }
        public DateTime DateLocation { get; set; }
        public DateTime DateFinLocation { get; set; }
        //En cas de suspension momentanee de la location
        public DateTime DateSuspensionLocation { get; set; }
        //En cas d'arret de la location avant l'echeance
        public DateTime DateArretLocation { get; set; }
        //Indique si le stock alloue est rendu
        public bool LocationRendue { get; set; }
        public double QuantitePrise { get; set; }
        public double QuantiteRendue { get; set; }

    }
}