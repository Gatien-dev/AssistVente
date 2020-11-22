using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public enum OperationType
    {
        Achat, Vente, Location, Ajustement, Création
    }

    public class StockManager
    {
        public AssistVenteContext db = new AssistVenteContext();

        public StockManager(AssistVenteContext db)
        {
            this.db = db;
        }

        public void AddStock(Guid ProdId, double Amount, OperationType type)
        {
            if (Amount == 0) return;
            var allProduits = db.Produits.ToList();
            var produit = db.Produits.First(p => p.ID == ProdId);
            //Log operation in stockHistory
            db.StockLogs.Add(new StockLog() { Amount = Amount, ProduitId = ProdId, Date = DateTime.Now, Id = Guid.NewGuid(), OldStock = produit.StockDisponible, NewStock = produit.StockDisponible + Amount, Type = type });
            if (type == OperationType.Location)
            {
                produit.StockEnLocation-= Amount;
            }
            produit.StockDisponible += Amount;
            db.SaveChanges();
        }

        public void RemoveStock(Guid ProdId, double Amount, OperationType type)
        {
            if (Amount == 0) return;
            var produit = db.Produits.First(p => p.ID == ProdId);
            //Log operation in stockHistory
            db.StockLogs.Add(new StockLog() { Amount = -Amount, ProduitId = ProdId, Date = DateTime.Now, Id = Guid.NewGuid(),OldStock=produit.StockDisponible,NewStock=produit.StockDisponible-Amount, Type = type });
            if (type == OperationType.Location)
            {
                produit.StockEnLocation += Amount;
            }
            produit.StockDisponible -= Amount;
            db.SaveChanges();
        }
    }
    public class StockLog
    {
        public Guid Id { get; set; }
        public Guid ProduitId { get; set; }
        public OperationType Type { get; set; }
        public double Amount { get; set; }
        public double NewStock { get; set; }
        public double OldStock { get; set; }
        public DateTime Date { get; set; }

    }
}
