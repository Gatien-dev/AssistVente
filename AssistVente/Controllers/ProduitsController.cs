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
    [Authorize(Roles = "Admin,Produits")]
    public class ProduitsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();

        //TODO : index des produits afficher le nom du createur pas son ID ou le cacher

        // GET: Produits
        public ActionResult Index()
        {
            return View(db.Produits.ToList());
        }

        // GET: Produits/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produit produit = db.Produits.Find(id);
            if (produit == null)
            {
                return HttpNotFound();
            }
            return View(produit);
        }

        // GET: Produits/Create
        [Authorize(Roles = "Admin,Produits-edition")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Produits/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nom,PrixAchat,PrixVente,ALouer,StockDisponible,PrixLocationParDefaut,DureeDeLocationParDefaut,Description,DateCreation,CreatorId")] Produit produit)
        {
            if (ModelState.IsValid)
            {
                produit.ID = Guid.NewGuid();
                produit.DureeDeLocationParDefaut = 1;
                produit.DateCreation = DateTime.Now;
                produit.CreatorId = User.Identity.GetUserId();
                var stock = produit.StockDisponible;
                produit.StockDisponible = 0;
                db.Produits.Add(produit);
                db.SaveChanges();
                new StockManager(db).AddStock(produit.ID, stock, OperationType.Création);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(produit);
        }

        // GET: Produits/Edit/5
        [Authorize(Roles = "Admin,Produits-edition")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produit produit = db.Produits.Find(id);
            if (produit == null)
            {
                return HttpNotFound();
            }
            return View(produit);
        }

        // POST: Produits/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nom,PrixAchat,PrixVente,ALouer,PrixLocationParDefaut,DureeDeLocationParDefaut,Description,DateCreation,CreatorId")] Produit produit)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(produit).State = EntityState.Modified;
                var dbProd = db.Produits.Find(produit.ID);
                dbProd.Nom = produit.Nom;
                dbProd.PrixAchat = produit.PrixAchat;
                dbProd.PrixVente = produit.PrixVente;
                dbProd.PrixLocationParDefaut = produit.PrixLocationParDefaut;
                dbProd.Description = produit.Description;
                produit.CreatorId = User.Identity.GetUserId();
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(produit);
        }

        [Authorize(Roles = "Admin,Produits-edition")]
        public ActionResult Ajustement(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produit produit = db.Produits.Find(id);
            if (produit == null)
            {
                return HttpNotFound();
            }
            return View(produit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ajustement([Bind(Include = "ID,ecartStock")] Guid ID, double ecartStock)
        {
            if (ModelState.IsValid)
            {
                var produit = db.Produits.Find(ID);
                new StockManager(db).AddStock(ID, -ecartStock, OperationType.Ajustement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
                return RedirectToAction("Ajustement",new { id = ID });
        }


        // GET: Produits/Delete/5
        [Authorize(Roles = "Admin,Produits-suppression")]
        public ActionResult Delete(Guid? id)
        {
            ViewBag.currentPage = "Suppression de produit";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produit produit = db.Produits.Find(id);
            if (produit == null)
            {
                return HttpNotFound();
            }
            return View(produit);
        }

        // POST: Produits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Produit produit = db.Produits.Find(id);
            db.Produits.Remove(produit);
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
