using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public enum OperationType
    {
        Achat, Vente, Location
    }

    public class StockManager
    {
        public AssistVenteContext db = new AssistVenteContext();

        public StockManager()
        {
            
        }

        public void AddStock(Guid ProdId, double Amount, OperationType type)
        {
            db.Produits.Find(ProdId).StockDisponible += Amount;
            //Log operation in stockHistory
            db.StockLogs.Add(new StockLog() { Amount = Amount, Date = DateTime.Now, Id = Guid.NewGuid(), Type = type });
            db.SaveChanges();
        }

        public void RemoveStock(Guid ProdId, double Amount, OperationType type)
        {
            db.Produits.Find(ProdId).StockDisponible -= Amount;
            //Log operation in stockHistory
            db.StockLogs.Add(new StockLog() { Amount = -Amount, Date = DateTime.Now, Id = Guid.NewGuid(), Type = type });
            db.SaveChanges();
        }
    }
    public class StockLog
    {
        public Guid Id { get; set; }
        public OperationType Type { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }

    }
}