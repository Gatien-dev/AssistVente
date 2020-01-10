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

namespace AssistVente.Controllers
{
    [Authorize(Roles = "Admin,Forfaits")]
    [LogFilter]
    public class ForfaitsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();

        // GET: Forfaits
        public ActionResult Index()
        {
            return View(db.Forfaits.ToList());
        }

        // GET: Forfaits/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forfait forfait = db.Forfaits.Find(id);
            if (forfait == null)
            {
                return HttpNotFound();
            }
            return View(forfait);
        }

        // GET: Forfaits/Create
        [Authorize(Roles = "Admin,Forfaits-edition")]
        public ActionResult Create()
        {

            ViewBag.GroupeForfaitId = new SelectList(db.GroupeForfaits, "ID", "Nom");
            return View();
        }

        // POST: Forfaits/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nom,Description,Duree,Montant,GroupeForfaitId")] Forfait forfait)
        {
            if (ModelState.IsValid)
            {
                forfait.Id = Guid.NewGuid();
                db.Forfaits.Add(forfait);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(forfait);
        }

        // GET: Forfaits/Edit/5
        [Authorize(Roles = "Admin,Forfaits-edition")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forfait forfait = db.Forfaits.Find(id);
            if (forfait == null)
            {
                return HttpNotFound();
            }
            return View(forfait);
        }

        // POST: Forfaits/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nom,Description,Duree,Montant,GroupeForfaitId")] Forfait forfait)
        {
            if (ModelState.IsValid)
            {
                db.Entry(forfait).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(forfait);
        }

        // GET: Forfaits/Delete/5
        [Authorize(Roles = "Admin,Forfaits-suppression")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forfait forfait = db.Forfaits.Find(id);
            if (forfait == null)
            {
                return HttpNotFound();
            }
            return View(forfait);
        }

        // POST: Forfaits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Forfait forfait = db.Forfaits.Find(id);
            db.Forfaits.Remove(forfait);
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
