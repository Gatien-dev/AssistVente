using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AssistVente.Models;
using AssistVente.Models.ViewModels;

namespace AssistVente.Controllers
{
    public class StockLogsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();

        // GET: StockLogs
        public ActionResult Index(Guid id)
        {
            var produit = db.Produits.Find(id);
            if (produit!= null)
            {
                ViewBag.produitName = produit.Nom;
            }
            if (id == Guid.Empty || id == null)
            {
                var data = db.StockLogs.Select(s => new stockLogIndexVM()
                {
                    ProduitId = s.ProduitId,
                    Amount = s.Amount,
                    Date = s.Date,
                    Id = s.Id,
                    Type = s.Type
                }).ToList();
                foreach (var item in data)
                {
                    item.ProduitName = db.Produits.Find(item.ProduitId).Nom;
                }
                return View(data.OrderByDescending(s => s.Date).ToList());
            }
            else
            //return View(db.StockLogs.Where(p => p.ProduitId == id).OrderByDescending(s => s.Date).ToList());
            {

                var data = db.StockLogs.Where(p => p.ProduitId == id).Select(s => new stockLogIndexVM()
                {
                    ProduitId = s.ProduitId,
                    Amount = s.Amount,
                    Date = s.Date,
                    Id = s.Id,
                    Type = s.Type
                }).ToList();
                foreach (var item in data)
                {
                    item.ProduitName = db.Produits.Find(item.ProduitId).Nom;
                }
                return View(data.OrderByDescending(s => s.Date).ToList());
            }
        }

        // GET: StockLogs/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockLog stockLog = db.StockLogs.Find(id);
            if (stockLog == null)
            {
                return HttpNotFound();
            }
            return View(stockLog);
        }

        // GET: StockLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StockLogs/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProduitId,Type,Amount,Date")] StockLog stockLog)
        {
            if (ModelState.IsValid)
            {
                stockLog.Id = Guid.NewGuid();
                db.StockLogs.Add(stockLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stockLog);
        }

        // GET: StockLogs/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockLog stockLog = db.StockLogs.Find(id);
            if (stockLog == null)
            {
                return HttpNotFound();
            }
            return View(stockLog);
        }

        // POST: StockLogs/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProduitId,Type,Amount,Date")] StockLog stockLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stockLog);
        }

        // GET: StockLogs/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockLog stockLog = db.StockLogs.Find(id);
            if (stockLog == null)
            {
                return HttpNotFound();
            }
            return View(stockLog);
        }

        // POST: StockLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            StockLog stockLog = db.StockLogs.Find(id);
            db.StockLogs.Remove(stockLog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
