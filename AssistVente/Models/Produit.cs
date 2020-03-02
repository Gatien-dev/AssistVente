using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class Produit
    {
        public Guid ID { get; set; }
        [Display(Name = "Désignation")]
        public string Nom { get; set; }
        [Display(Name = "Prix d'achat")]
        public double PrixAchat { get; set; }
        [Display(Name = "Prix de vente")]
        public double PrixVente { get; set; }
        [Display(Name = "Prix de location par défaut")]
        public double PrixLocationParDefaut { get; set; }
        [Display(Name = "Disponible en location?")]
        public bool ALouer { get; set; }
        [Display(Name = "Stock disponible")]
        public double StockDisponible { get; set; }
        [Display(Name = "Stock En Location")]
        public double StockEnLocation { get; set; }
        [Display(Name = "Durée de location par défaut")]
        public int DureeDeLocationParDefaut { get; set; } = 1;
        [Display(Name = "Informations complémentaires")]
        public string Description { get; set; }
        [Display(Name = "Date")]
        public DateTime DateCreation { get; set; }
        [Display(Name = "Createur")]
        public string CreatorId { get; set; }
        public List<DetailAchat> DetailsAchat { get; set; }
        public List<DetailVente> DetailsVente { get; set; }
        [ForeignKey("Categorie")]
        public int? CategorieProduitId { get; set; }
        public virtual CategorieProduit Categorie { get; set; }
    }

    public class CategorieProduit
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Display(Name="Catégorie")]
        public string Name { get; set; }
        public List<Produit> Produits { get; set; }
    }
}