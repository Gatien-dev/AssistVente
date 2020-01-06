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
        public Guid ProduitId { get; set; }
        [Display(Name ="Produit")]
        public string ProduitName { get; set; }
        public OperationType Type { get; set; }
        [Display(Name ="Quantité")]
        public double Amount { get; set; }
        public DateTime Date { get; set; }

    }
}