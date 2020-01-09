using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AssistVente.Models;

namespace AssistVente.Controllers
{
    public class GroupeAbonnementsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();

        // GET: GroupeAbonnements
        public ActionResult Index()
        {
            return View(db.GroupeAbonnements.ToList());
        }

        // GET: GroupeAbonnements/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupeAbonnements groupeAbonnements = db.GroupeAbonnements.Find(id);
            if (groupeAbonnements == null)
            {
                return HttpNotFound();
            }
            return View(groupeAbonnements);
        }

        // GET: GroupeAbonnements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GroupeAbonnements/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,DateCreation")] GroupeAbonnements groupeAbonnements)
        {
            if (ModelState.IsValid)
            {
                groupeAbonnements.Id = Guid.NewGuid();
                groupeAbonnements.DateCreation = DateTime.Now;
                db.GroupeAbonnements.Add(groupeAbonnements);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(groupeAbonnements);
        }

        // GET: GroupeAbonnements/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupeAbonnements groupeAbonnements = db.GroupeAbonnements.Find(id);
            if (groupeAbonnements == null)
            {
                return HttpNotFound();
            }
            return View(groupeAbonnements);
        }

        // POST: GroupeAbonnements/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,DateCreation")] GroupeAbonnements groupeAbonnements)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupeAbonnements).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(groupeAbonnements);
        }

        // GET: GroupeAbonnements/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupeAbonnements groupeAbonnements = db.GroupeAbonnements.Find(id);
            if (groupeAbonnements == null)
            {
                return HttpNotFound();
            }
            return View(groupeAbonnements);
        }

        // POST: GroupeAbonnements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            GroupeAbonnements groupeAbonnements = db.GroupeAbonnements.Find(id);
            if(!groupeAbonnements.Abonnements.Any())
            db.GroupeAbonnements.Remove(groupeAbonnements);
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
