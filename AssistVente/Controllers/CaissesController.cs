﻿using System;
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
    public class CaissesController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();

        // GET: Caisses
        public ActionResult Index()
        {
            return View(db.Caisses.ToList());
        }

        // GET: Caisses/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Caisse caisse = db.Caisses.Find(id);
            if (caisse == null)
            {
                return HttpNotFound();
            }
            return View(caisse);
        }

        // GET: Caisses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Caisses/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nom,Solde")] Caisse caisse)
        {
            if (ModelState.IsValid)
            {
                caisse.ID = Guid.NewGuid();
                db.Caisses.Add(caisse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(caisse);
        }

        // GET: Caisses/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Caisse caisse = db.Caisses.Find(id);
            if (caisse == null)
            {
                return HttpNotFound();
            }
            return View(caisse);
        }

        // POST: Caisses/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nom,Solde")] Caisse caisse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(caisse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(caisse);
        }

        // TODO: veiller a ne pas supprimer les caisses qui ont une historiqueou qui ont une operation
        // GET: Caisses/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Caisse caisse = db.Caisses.Find(id);
            if (caisse == null)
            {
                return HttpNotFound();
            }
            return View(caisse);
        }

        // POST: Caisses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Caisse caisse = db.Caisses.Find(id);
            db.Caisses.Remove(caisse);
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