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
using Microsoft.AspNet.Identity;

namespace AssistVente.Controllers
{
    [Authorize(Roles = "Admin,Ventes")]
    public class VentesController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();

        // GET: Ventes
        public ActionResult Index()
        {
            var operations = db.Operations.OfType<Vente>().Include(v => v.Client).OrderByDescending(v => v.Date);
            ViewBag.caisseDefined = db.Caisses.Any();
            return View(operations.ToList());
        }

        // GET: Ventes/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vente vente = db.Operations.OfType<Vente>().Include(v => v.Details).First(v => v.Id == id);
            if (vente == null)
            {
                return HttpNotFound();
            }
            return View(vente);
        }

        // GET: Ventes/Create
        public ActionResult Create()
        {
            if (!db.Caisses.Any()) return RedirectToAction("Index");
            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom");
            var venteVM = new VenteVM()
            {
                Details = new List<DetailVenteVM>()
            };
            var produits = db.Produits.ToList();
            foreach (var produit in produits)
            {
                venteVM.Details.Add(new DetailVenteVM()
                {
                    NomProduit = produit.Nom,
                    ProduitId = produit.ID,
                    PU = produit.PrixVente,
                    Quantite = 0
                });
            }
            return View(venteVM);
        }

        // POST: Ventes/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VenteVM vente, Guid clientId)
        {
            if (ModelState.IsValid)
            {
                StockManager stockManager = new StockManager();
                Vente newVente = new Vente()
                {
                    Details = new List<DetailVente>(),
                    Client = db.Clients.Find(clientId),
                    Date = DateTime.Now,
                    Id = Guid.NewGuid(),
                    ClientId = clientId,
                    MontantRegle = 0,
                    UserId = User.Identity.GetUserId()
                };
                double total = 0;
                foreach (var detail in vente.Details)
                {
                    if (detail.Quantite > 0)
                        newVente.Details.Add(new DetailVente()
                        {
                            Produit = db.Produits.Find(detail.ProduitId),
                            QuantiteVendue = detail.Quantite,
                            ProduitID = detail.ProduitId,
                            ID = Guid.NewGuid()
                        });
                    stockManager.RemoveStock(detail.ProduitId, detail.Quantite, OperationType.Vente);
                    total += detail.Quantite * db.Produits.Find(detail.ProduitId).PrixVente;
                }
                newVente.Montant = total;
                newVente.MontantRestant = total;

                db.Operations.Add(newVente);
                db.SaveChanges();
                new CaisseManager(db).reglerVente(vente.MontantPaye, newVente);
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom", vente.ClientId);
            return View(vente);
        }

        // GET: Ventes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vente vente = (Vente)db.Operations.Find(id);
            if (vente == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom", vente.ClientId);
            return View(vente);
        }

        // POST: Ventes/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Montant,Date,UserId,ClientId,MontantRegle,MontantRestant")] Vente vente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom", vente.ClientId);
            return View(vente);
        }

        // GET: Ventes/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Vente vente = db.Operations.OfType<Vente>().Include(v => v.Details).FirstOrDefault(o => o.Id == id);
            if (vente == null)
            {
                return HttpNotFound();
            }

            return View(vente);
        }
        public ActionResult Reglement(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vente vente = (Vente)db.Operations.Find(id);
            if (vente == null)
            {
                return HttpNotFound();
            }
            return View(vente);
        }
        [HttpPost, ActionName("Reglement")]
        [ValidateAntiForgeryToken]
        public ActionResult ReglementConfirmed(Guid id, double mtRegle)
        {
            Vente vente = (Vente)db.Operations.Find(id);
            new CaisseManager(db).reglerVente(mtRegle, vente);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // POST: Ventes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            StockManager manager = new StockManager();
            Vente vente = db.Operations.OfType<Vente>().Include(v => v.Details).FirstOrDefault(o => o.Id == id);
            foreach (var detail in vente.Details)
            {
                manager.AddStock(detail.ProduitID, detail.QuantiteVendue, OperationType.Vente);
            }
            db.Operations.Remove(vente);
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
