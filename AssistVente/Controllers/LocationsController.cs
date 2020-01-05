using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AssistVente.Models;
using Microsoft.AspNet.Identity;

namespace AssistVente.Controllers
{
    [Authorize]
    public class LocationsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();

        // GET: Locations
        public ActionResult Index()
        {
            var locations = db.Operations.OfType<Location>().Include(l => l.Produit);
            return View(locations.ToList());
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
        public ActionResult Create()
        {
            ViewBag.ProduitId = new SelectList(db.Produits, "ID", "Nom");
            return View();
        }

        // POST: Locations/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProduitId,DateLocation,DateFinLocation,DateSuspensionLocation,DateArretLocation,LocationRendue,QuantitePrise,QuantiteRendue,Montant,Date,UserId")] Location location)
        {
            if (ModelState.IsValid)
            {
                location.Id = Guid.NewGuid();
                location.UserId = User.Identity.GetUserId();
                db.Operations.Add(location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProduitId = new SelectList(db.Produits, "ID", "Nom", location.ProduitId);
            return View(location);
        }

        // GET: Locations/Edit/5
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

        // GET: Locations/Delete/5
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
