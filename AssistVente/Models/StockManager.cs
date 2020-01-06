using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public enum OperationType
    {
        Achat, Vente, Location, Ajustement
    }

    public class StockManager
    {
        AssistVenteContext db;

        public StockManager(AssistVenteContext db)
        {
            this.db = db;
        }

        public void AddStock(Guid ProdId, double Amount, OperationType type)
        {
            var allProduits = db.Produits.ToList();
            db.Produits.First(p=>p.ID==ProdId).StockDisponible += Amount;
            //Log operation in stockHistory
            db.StockLogs.Add(new StockLog() { Amount = Amount,ProduitId=ProdId, Date = DateTime.Now, Id = Guid.NewGuid(), Type = type });
            db.SaveChanges();
        }

        public void RemoveStock(Guid ProdId, double Amount, OperationType type)
        {
            db.Produits.First(p=>p.ID==ProdId).StockDisponible -= Amount;
            //Log operation in stockHistory
            db.StockLogs.Add(new StockLog() { Amount = -Amount,ProduitId=ProdId, Date = DateTime.Now, Id = Guid.NewGuid(), Type = type });
            db.SaveChanges();
        }
    }
    public class StockLog
    {
        public Guid Id { get; set; }
        public Guid ProduitId { get; set; }
        public OperationType Type { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }

    }
}