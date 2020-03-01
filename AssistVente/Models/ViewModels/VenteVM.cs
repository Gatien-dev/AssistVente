using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssistVente.Models.ViewModels
{
    public class VenteVM
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public List<DetailVenteVM> Details { get; set; }
        [Display(Name ="Montant payé")]
        public double MontantPaye { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name ="Date")]
        public DateTime DateOperation { get; set; }
    }
    public class DetailVenteVM
    {
        public Guid ProduitId { get; set; }
        public string NomProduit { get; set; }
        public double PU { get; set; }
        public double Quantite { get; set; }
    }
}