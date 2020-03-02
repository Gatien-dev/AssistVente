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
    public class CategorieProduitsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();

        // GET: CategorieProduits
        public ActionResult Index()
        {
            return View(db.CategorieProduits.ToList());
        }

        // GET: CategorieProduits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategorieProduit categorieProduit = db.CategorieProduits.Find(id);
            if (categorieProduit == null)
            {
                return HttpNotFound();
            }
            return View(categorieProduit);
        }

        // GET: CategorieProduits/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategorieProduits/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] CategorieProduit categorieProduit)
        {
            if (ModelState.IsValid)
            {
                db.CategorieProduits.Add(categorieProduit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categorieProduit);
        }

        // GET: CategorieProduits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategorieProduit categorieProduit = db.CategorieProduits.Find(id);
            if (categorieProduit == null)
            {
                return HttpNotFound();
            }
            return View(categorieProduit);
        }

        // POST: CategorieProduits/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] CategorieProduit categorieProduit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categorieProduit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categorieProduit);
        }

        // GET: CategorieProduits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategorieProduit categorieProduit = db.CategorieProduits.Find(id);
            if (categorieProduit == null)
            {
                return HttpNotFound();
            }
            return View(categorieProduit);
        }

        // POST: CategorieProduits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategorieProduit categorieProduit = db.CategorieProduits.Find(id);
            db.CategorieProduits.Remove(categorieProduit);
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
