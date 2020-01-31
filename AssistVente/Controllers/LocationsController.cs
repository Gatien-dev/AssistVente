using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AssistVente.Filters;
using AssistVente.Models;
using Microsoft.AspNet.Identity;

namespace AssistVente.Controllers
{
    [LogFilter]
    [Authorize(Roles = "Admin,Locations")]
    public class LocationsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();
        //TODO: Rendre les locations et reglement des locations
        // GET: Locations
        public ActionResult Index()
        {
            var locations = db.Operations.OfType<Location>().Include(l => l.Produit).OrderByDescending(l => l.LocationRendue).OrderByDescending(l => l.Date);
            return View(locations);
        }

        // GET: Locations/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = (Location)db.Operations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // GET: Locations/Create
        [Authorize(Roles = "Admin,Locations-edition")]
        public ActionResult Create()
        {
            ViewBag.ProduitId = new SelectList(db.Produits.Where(p => p.ALouer).Select(p => new { id = p.ID, Nom = p.Nom + " (" + p.StockDisponible + " disponibles)" }), "ID", "Nom");
            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom");
            ViewBag.editAmount = User.IsInRole("Modifier les montants de location") || User.IsInRole("Admin");
            return View();
        }

        // POST: Locations/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProduitId,ClientId,DateLocation,DateFinLocation,MontantPaye,DateSuspensionLocation,DateArretLocation,LocationRendue,QuantitePrise,QuantiteRendue,Montant,Date,UserId")] Location location)
        {
            if (ModelState.IsValid)
            {
                location.Id = Guid.NewGuid();
                location.UserId = User.Identity.GetUserId();
                var produit = db.Produits.Find(location.ProduitId);
                if (produit == null || produit.StockDisponible < location.QuantitePrise)
                {
                    ModelState.AddModelError("QuantitePrise", "La quantité de produit à louer n'est pas disponible");
                    ViewBag.ProduitId = new SelectList(db.Produits, "ID", "Nom", location.ProduitId);
                    ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom");
                    return View(location);
                }
                new StockManager(db).RemoveStock(location.ProduitId, location.QuantitePrise, OperationType.Location);
                location.Date = DateTime.Now;
                location.DateArretLocation = location.DateFinLocation;
                location.LocationRendue = false;
                location.UserId = User.Identity.GetUserId();
                if (!User.IsInRole("Modifier les montants de location"))
                {
                    location.Montant = produit.PrixLocationParDefaut * (location.DateFinLocation - location.DateLocation).TotalDays * location.QuantitePrise;
                }
                location.QuantiteRendue = 0;
                location.MontantRestant = location.Montant - location.MontantPaye;

                if (location.MontantRestant < 0)
                {
                    location.MontantRestant = 0;
                }
                db.Operations.Add(location);
                db.SaveChanges();
                new CaisseManager(db).reglerLocation(location.Montant, location, "Paiement de location de " + location.QuantitePrise + " " + produit.Nom, "Especes");
                return RedirectToAction("Confirmer", new { id = location.Id });
            }

            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom");
            ViewBag.ProduitId = new SelectList(db.Produits, "ID", "Nom", location.ProduitId);
            return View(location);
        }


        // GET: Locations/Edit/5
        [Authorize(Roles = "Admin,Locations-edition")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = (Location)db.Operations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProduitId = new SelectList(db.Produits, "ID", "Nom", location.ProduitId);
            return View(location);
        }

        // POST: Locations/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProduitId,DateLocation,DateFinLocation,DateSuspensionLocation,DateArretLocation,LocationRendue,QuantitePrise,QuantiteRendue,Montant,Date,UserId")] Location location)
        {
            if (ModelState.IsValid)
            {
                db.Entry(location).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProduitId = new SelectList(db.Produits, "ID", "Nom", location.ProduitId);
            return View(location);
        }

        [Authorize(Roles = "Admin,Locations-edition")]
        public ActionResult Rendre(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = (Location)db.Operations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        [HttpPost, ActionName("Rendre")]
        [ValidateAntiForgeryToken]
        public ActionResult RendreConfirmed(Guid id, double qteRendue)
        {
            Location location = (Location)db.Operations.Find(id);
            if (qteRendue >= location.QuantitePrise - location.QuantiteRendue)
            {
                location.LocationRendue = true;
            }
            location.QuantiteRendue += qteRendue;
            //Restitution du stock qui a ete pris
            new StockManager(db).AddStock(location.ProduitId, location.QuantitePrise, OperationType.Location);

            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Confirmer(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location Location = db.Operations.OfType<Location>().First(v => v.Id == id);
            if (Location == null)
            {
                return HttpNotFound();
            }
            return View(Location);
        }
        [Authorize(Roles = "Admin,Locations-edition")]
        public ActionResult Reglement(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = (Location)db.Operations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        [HttpPost, ActionName("Reglement")]
        [ValidateAntiForgeryToken]
        public ActionResult ReglementConfirmed(Guid id, double montantPaye, string reglement)
        {
            Location location = (Location)db.Operations.Find(id);
            location.MontantPaye += montantPaye;
            if (location.MontantPaye > location.Montant)
            {

                location.MontantPaye = location.Montant;
            }
            location.MontantRestant = location.Montant - location.MontantPaye;
            if (location.MontantRestant < 0)
            {
                location.MontantRestant = 0;
            }
            new CaisseManager(db).reglerLocation(montantPaye, location, "Paiement de location de " + location.QuantitePrise + " " + location.Produit.Nom, reglement);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Locations/Delete/5
        [Authorize(Roles = "Admin,Locations-suppression")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = (Location)db.Operations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Location location = (Location)db.Operations.Find(id);
            //Restitution du stock qui a ete pris
            if (!location.LocationRendue)
            {
                new StockManager(db).AddStock(location.ProduitId, location.QuantitePrise, OperationType.Location);
                new CaisseManager(db).reglerLocation(-location.MontantPaye, location, "Suppression de location " + location.QuantitePrise + " " + location.Produit.Nom, "especes");
            }
            db.Operations.Remove(location);
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
