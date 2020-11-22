using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssistVente.Models.ViewModels
{
    public class stockLogIndexVM
    {
        public Guid Id { get; set; }
        [Display(Name ="N°")]
        public int Number { get; set; }
        [Display(Name ="SI")]
        public double OldStock { get; set; }
        [Display(Name ="SF")]
        public double NewStock { get; set; }
        public Guid ProduitId { get; set; }
        [Display(Name ="Produit")]
        public string ProduitName { get; set; }
        public OperationType Type { get; set; }
        [Display(Name ="Quantité")]
        public double Amount { get; set; }
        public DateTime Date { get; set; }

    }
}