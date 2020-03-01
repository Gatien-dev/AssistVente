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
using AssistVente.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using IdentitySample.Models;

namespace AssistVente.Controllers
{
    [Authorize(Roles = "Admin,Ventes")]
    [LogFilter]
    public class VentesController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();

        // GET: Ventes
        public ActionResult Index()
        {
            var operations = db.Operations.OfType<Vente>().Include(v => v.Client).Include(v => v.Details).OrderByDescending(v => v.Date);
            ViewBag.caisseDefined = db.Caisses.Any();
            return View(operations.ToList());
        }
        public ActionResult VentesProduit(Guid idProduit)
        {
            var operations = db.Operations.OfType<Vente>().Where(v => v.Details.Any(d => d.Produit.ID == idProduit)).Include(v => v.Client).Include(v => v.Details).OrderByDescending(v => v.Date);
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
            ViewBag.userName = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().Users.FirstOrDefault(u => u.Id == vente.UserId).UserName;
            return View(vente);
        }

        public ActionResult Confirmer(Guid? id)
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
        public ActionResult recu(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vente abonnement = db.Operations.OfType<Vente>().Include(a => a.Details).First(a => a.Id == id);
            if (abonnement == null)
            {
                return HttpNotFound();
            }
            return View(abonnement);
        }
        // GET: Ventes/Create
        [Authorize(Roles = "Admin,Ventes-edition")]
        public ActionResult Create()
        {
            if (!db.Caisses.Any()) return RedirectToAction("Index");
            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom");
            var venteVM = new VenteVM()
            {
                Details = new List<DetailVenteVM>()
            };
            var produits = db.Produits.OrderBy(p => p.Nom).ToList();
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
        public ActionResult Create(VenteVM vente, Guid clientId, string reglement)
        {
            if (ModelState.IsValid)
            {
                StockManager stockManager = new StockManager(db);
                Vente newVente = new Vente()
                {
                    Details = new List<DetailVente>(),
                    Client = db.Clients.Find(clientId),
                    Date = DateTime.Now,
                    Id = Guid.NewGuid(),
                    ClientId = clientId,
                    MontantRegle = 0,
                    DateOperation = DateTime.Now,
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
                new CaisseManager(db).reglerVente(vente.MontantPaye, newVente, "Paiement de vente", reglement);
                return RedirectToAction("Confirmer", new { id = newVente.Id });
            }

            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom", vente.ClientId);
            return View(vente);
        }

        // GET: Ventes/Edit/5
        [Authorize(Roles = "Admin,Ventes-edition")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vente vente = db.Operations.OfType<Vente>().Include(o => o.Details).FirstOrDefault(o => o.Id == id);
            if (vente == null)
            {
                return HttpNotFound();
            }
            if (!db.Caisses.Any()) return RedirectToAction("Index");
            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom");
            var venteVM = new VenteVM()
            {
                DateOperation = vente.DateOperation,
                Details = new List<DetailVenteVM>()
            };
            var produits = db.Produits.OrderBy(p => p.Nom).ToList();
            foreach (var produit in produits)
            {
                venteVM.Details.Add(new DetailVenteVM()
                {
                    NomProduit = produit.Nom,
                    ProduitId = produit.ID,
                    PU = produit.PrixVente,
                    Quantite = 0
                });
                var detail = vente.Details.FirstOrDefault(d => d.Produit.ID == produit.ID);
                if (detail != null)
                {
                    venteVM.Details.Last().Quantite = detail.QuantiteVendue;
                }
            }
            return View(venteVM);
        }

        // POST: Ventes/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VenteVM vente, Guid clientId, string reglement)
        {
            if (ModelState.IsValid)
            {
                StockManager stockManager = new StockManager(db);
                Vente oldVente = db.Ventes.Where(v => v.Id == vente.Id).Include(v => v.Details.Select(d => d.Produit)).First();
                //Restitution du stock
                foreach (var detail in oldVente.Details)
                {
                    stockManager.AddStock(detail.ProduitID, detail.QuantiteVendue, OperationType.Vente);
                }
                //Annulation des reglements
                var caisseManager = new CaisseManager(db);
                caisseManager.AnnulerReglementsVente(oldVente, "suppression");
                //Suppression des anciens details
                oldVente.Montant = 0;
                oldVente.MontantRegle = 0;
                oldVente.MontantRestant = 0;
                for (int i = 0; i < oldVente.Details.Count; i++)
                {
                    db.DetailsVente.Remove(oldVente.Details[i]);
                }
                db.SaveChanges();
                //Enregistrement des nouveaux details commande
                double total = 0;
                foreach (var detail in vente.Details)
                {
                    if (detail.Quantite > 0)
                        oldVente.Details.Add(new DetailVente()
                        {
                            Produit = db.Produits.Find(detail.ProduitId),
                            QuantiteVendue = detail.Quantite,
                            ProduitID = detail.ProduitId,
                            ID = Guid.NewGuid()
                        });
                //Sortie de stock
                    stockManager.RemoveStock(detail.ProduitId, detail.Quantite, OperationType.Vente);
                    total += detail.Quantite * db.Produits.Find(detail.ProduitId).PrixVente;
                }

                //Reglements

                oldVente.Montant = total;
                oldVente.MontantRestant = total;
                db.SaveChanges();
                caisseManager.reglerVente(vente.MontantPaye, oldVente, "Paiement de vente", reglement);
                return RedirectToAction("Confirmer", new { id = oldVente.Id });
            }

            ViewBag.ClientId = new SelectList(db.Clients, "ID", "Nom", vente.ClientId);
            return View(vente);
        }



        // GET: Ventes/Delete/5
        [Authorize(Roles = "Admin,Ventes-suppression")]
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

        [Authorize(Roles = "Admin,Ventes-edition")]
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
        public ActionResult ReglementConfirmed(Guid id, double mtRegle, string reglement)
        {
            Vente vente = (Vente)db.Operations.Find(id);
            new CaisseManager(db).reglerVente(mtRegle, vente, "Paiement de vente", reglement);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Ventes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            StockManager manager = new StockManager(db);
            Vente vente = db.Operations.OfType<Vente>().Include(v => v.Details).FirstOrDefault(o => o.Id == id);
            foreach (var detail in vente.Details)
            {
                manager.AddStock(detail.ProduitID, detail.QuantiteVendue, OperationType.Vente);
            }
            var caisseManager = new CaisseManager(db);
            caisseManager.AnnulerReglementsVente(vente, "suppression");
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
