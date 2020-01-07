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
    public class AbonnementsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();
        // GET: Abonnements
        public ActionResult Index()
        {
            var abonnements = db.Operations.OfType<Abonnement>().Include(a => a.Forfait).OrderByDescending(a => a.Termine).OrderByDescending(l => l.Date);
            return View(abonnements);
        }

        // GET: Abonnements/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Abonnement abonnement = (Abonnement)db.Operations.Find(id);
            if (abonnement == null)
            {
                return HttpNotFound();
            }
            return View(abonnement);
        }

        // GET: Abonnements/Create
        public ActionResult Create()
        {
            var forfaits = new List<Forfait>();
            foreach (var f in db.Forfaits)
            {
                forfaits.Add(new Forfait()
                {
                    Abonnements = f.Abonnements,
                    Description = f.Description,
                    Duree = f.Duree,
                    Id = f.Id,
                    Montant = f.Montant,
                    Nom = f.Nom + " (" + f.Montant + ")"
                });
            }
            ViewBag.ForfaitId = new SelectList(forfaits, "ID", "Nom", "Montant");
            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ForfaitId,ClientId,DateDebut,DateFin,ResteAPayer,DateSuspension,Montant,Date,UserId,SommePaye")] Abonnement abonnement)
        {
            if (ModelState.IsValid)
            {
                abonnement.Id = Guid.NewGuid();
                abonnement.UserId = User.Identity.GetUserId();
                var forfait = db.Forfaits.Find(abonnement.ForfaitId);

                if (forfait == null)
                {
                    ModelState.AddModelError("QuantitePrise", "La quantité de produit à louer n'est pas disponible");
                    ViewBag.ProduitId = new SelectList(db.Produits, "ID", "Nom", abonnement.ForfaitId);
                    return View(abonnement);
                }

                abonnement.Date = DateTime.Now;
                abonnement.DateFin = abonnement.DateDebut.AddDays(forfait.Duree);
                abonnement.Suspendu = false;
                abonnement.UserId = User.Identity.GetUserId();
                abonnement.ResteAPayer = abonnement.Montant - abonnement.SommePaye;

                if (abonnement.ResteAPayer < 0)
                {
                    abonnement.ResteAPayer = 0;
                }
                abonnement.DateSuspension = abonnement.DateFin;
                db.Operations.Add(abonnement);
                db.SaveChanges();
                var caisseManager = new CaisseManager(db);
                caisseManager.reglerAbonnement(abonnement.SommePaye, abonnement);
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom");
            ViewBag.ProduitId = new SelectList(db.Produits, "ID", "Nom", abonnement.ForfaitId);
            return View(abonnement);
        }


        //GET: Abonnements/suspendre/5
        public ActionResult Suspendre(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Abonnement abonnement = (Abonnement)db.Operations.Find(id);
            if (abonnement == null)
            {
                return HttpNotFound();
            }
            return View(abonnement);
        }

        // POST: Abonnements/Suspendre/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Suspendre(Guid id)
        {
            var abonnement = (Abonnement)db.Operations.Find(id);
            abonnement.Suspendu = true;
            abonnement.DateSuspension = DateTime.Now;

            db.Entry(abonnement).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Reglement(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Abonnement abonnement = (Abonnement)db.Operations.Find(id);
            if (abonnement == null)
            {
                return HttpNotFound();
            }
            return View(abonnement);
        }

        [HttpPost, ActionName("Reglement")]
        [ValidateAntiForgeryToken]
        public ActionResult Reglement(Guid id, double montantReglement)
        {
            Abonnement abonnement = (Abonnement)db.Operations.Find(id);
            abonnement.SommePaye += montantReglement;
            if (abonnement.SommePaye > abonnement.Montant)
            {

                abonnement.SommePaye = abonnement.Montant;
            }

            abonnement.ResteAPayer = abonnement.Montant - abonnement.SommePaye;
            if (abonnement.ResteAPayer < 0)
            {
                abonnement.ResteAPayer = 0;
            }
            new CaisseManager(db).reglerAbonnement(montantReglement, abonnement);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Abonnement abonnement = (Abonnement)db.Operations.Find(id);
            if (abonnement == null)
            {
                return HttpNotFound();
            }
            return View(abonnement);
        }

        // POST: Abonnements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Abonnement abonnement = (Abonnement)db.Operations.Find(id);

            db.Operations.Remove(abonnement);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}