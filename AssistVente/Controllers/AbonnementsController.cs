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
    [Authorize(Roles = "Admin,Abonnements")]
    public class AbonnementsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();
        // GET: Abonnements
        public ActionResult Index()
        {
            checkAbonnements();
            ViewBag.caisseDefined = db.Caisses.Any();
            var abonnements = db.Operations.OfType<Abonnement>().Include(a => a.Forfait).OrderByDescending(a => a.Termine).OrderByDescending(l => l.Date).ToList();

            foreach (var abonnement in abonnements)
            {

                if ((abonnement.DateFin - DateTime.Now).TotalDays < -1 && !abonnement.Suspendu)
                {
                    abonnement.Termine = true;
                    db.Entry(abonnement).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

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
        public ActionResult recu(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Abonnement abonnement = db.Operations.OfType<Abonnement>().Include(a => a.Forfait).First(a => a.Id == id);
            if (abonnement == null)
            {
                return HttpNotFound();
            }
            return View(abonnement);
        }
        // GET: Abonnements/Create
        [Authorize(Roles = "Admin, Abonnements-edition")]
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
        public ActionResult Create([Bind(Include = "Id,ForfaitId,ClientId,DateDebut,DateFin,ResteAPayer,DateSuspension,Montant,Date,UserId,SommePaye")] Abonnement abonnement, string reglement)
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

                abonnement.Montant = forfait.Montant;
                abonnement.SommePaye = forfait.Montant;
                abonnement.ResteAPayer = 0;
                abonnement.Date = DateTime.Now;
                abonnement.DateFin = abonnement.DateDebut.AddDays(forfait.Duree);
                abonnement.Suspendu = false;
                abonnement.UserId = User.Identity.GetUserId();
                //abonnement.ResteAPayer = abonnement.Montant - abonnement.SommePaye;

                //if (abonnement.ResteAPayer < 0)
                //{
                //    abonnement.ResteAPayer = 0;
                //}
                abonnement.DateSuspension = abonnement.DateFin;
                db.Operations.Add(abonnement);
                db.SaveChanges();
                var caisseManager = new CaisseManager(db);
                string raison = "Reglement abonnement";
                caisseManager.reglerAbonnement(abonnement.SommePaye, abonnement, raison, reglement);

                //Enregistrement sous forme de vente pour les traces
                Vente newVente = new Vente()
                {
                    Details = new List<DetailVente>(),
                    //Client = db.Clients.Find(clientId),
                    Date = DateTime.Now,
                    Id = Guid.NewGuid(),
                    ClientId = abonnement.ClientId,
                    MontantRegle = 0,
                    Montant=forfait.Montant,
                    DateOperation = DateTime.Now,
                    UserId = User.Identity.GetUserId()
                };
                double total = 0;
                var produit = db.Produits.FirstOrDefault(p => p.Nom == forfait.Nom);
                if (produit == null)
                {
                    produit = new Produit()
                    {
                        ID = Guid.NewGuid(),
                        Nom = forfait.Nom,
                        DateCreation = DateTime.Now,
                        PrixVente = forfait.Montant
                    };
                    db.Produits.Add(produit);
                    db.SaveChanges();
                }

                newVente.Details.Add(new DetailVente()
                {
                    Produit = produit,
                    QuantiteVendue = 1,
                    ProduitID = produit.ID,
                    ID = Guid.NewGuid()
                });
                newVente.Montant = 0;
                newVente.MontantRestant = 0;
                newVente.MontantRegle = forfait.Montant;

                db.Operations.Add(newVente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom");
            ViewBag.ProduitId = new SelectList(db.Produits, "ID", "Nom", abonnement.ForfaitId);
            return View(abonnement);
        }


        //GET: Abonnements/suspendre/5
        [Authorize(Roles = "Admin, Abonnements-edition")]
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
        [HttpPost, ActionName("Suspendre")]
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

        //GET: Abonnements/reprendre/5
        [Authorize(Roles = "Admin, Abonnements-edition")]
        public ActionResult Reprendre(Guid? id)
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

        // POST: Abonnements/Reprendre/5
        [HttpPost, ActionName("Reprendre")]
        [ValidateAntiForgeryToken]
        public ActionResult Reprendre(Guid id)
        {
            var abonnement = (Abonnement)db.Operations.Find(id);
            abonnement.DateFin = DateTime.Now.AddDays(abonnement.Forfait.Duree - (abonnement.DateSuspension - abonnement.DateDebut).TotalDays);
            abonnement.Suspendu = false;
            //abonnement.DateSuspension = DateTime.Now;

            db.Entry(abonnement).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, Abonnements-edition")]
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
            string raison = "Reglement abonnement";
            string modeReglement = "";
            new CaisseManager(db).reglerAbonnement(montantReglement, abonnement, raison, modeReglement);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, Abonnements-suppression")]
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

            if (!abonnement.Termine)
            {
                //foreach (var reglement in db.Reglements.Where(r => r.IdOperation == abonnement.Id).ToList())
                //{
                //    db.Reglements.Remove(reglement);
                //    db.SaveChanges();
                //}

                string raison = "Résiliation d'abonnement";
                string modeReglement = "";
                new CaisseManager(db).reglerAbonnement(-abonnement.Montant, abonnement, raison, modeReglement);
            }

            db.Operations.Remove(abonnement);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        void checkAbonnements()
        {
            foreach (var abo in db.Abonnements)
            {
                if (abo.DateFin <= DateTime.Now) abo.Termine = true;
            }
            db.SaveChanges();
        }
    }
}