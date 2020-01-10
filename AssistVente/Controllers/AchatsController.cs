using AssistVente.Filters;
using AssistVente.Models;
using AssistVente.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AssistVente.Controllers
{
    [Authorize(Roles = "Admin,Achats")]
    [LogFilter]
    public class AchatsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();
        private StockManager stockManager;

        public AchatsController()
        {
            stockManager = new StockManager(db);
        }
        // GET: Achats
        //TODO : Ajouter les datatables bootstrap
        public ActionResult Index()
        {
            var achats = db.Operations.OfType<Achat>().ToList();

            return View(achats);

            //if (achats.Count <= 0)
            //{
            //    return View(achats);
            //}
            //else
            //{
            //    // Recuperer la liste des detailAchat pour chaque achat
            //    foreach (var achat in achats)
            //    {
            //        var details = db.DetailsAchat.Where(d => d.AchatId == achat.Id).ToList();

            //        //recupere le produit pour chaque detail
            //        foreach (var detail in details)
            //        {
            //            var produit = db.Produits.Where(p => p.ID == detail.ProduitID).FirstOrDefault();
            //            detail.Produit = produit;
            //        }

            //        achat.Details = details;
            //    }

            //    return View(achats);
            //}
        }

        public ActionResult CreateAchat()
        {
            var achatVM = new AchatCreateVM()
            {
                Details = new List<DetailAchatVM>()
            };
            var produits = db.Produits.ToList();
            foreach (var produit in produits)
            {
                achatVM.Details.Add(new DetailAchatVM()
                {
                    NomProduit = produit.Nom,
                    ProduitId = produit.ID,
                    //PU = produit.PrixAchat,
                    Quantite = 0
                });
            }
            return View(achatVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAchat(AchatCreateVM achat)
        {
            if (ModelState.IsValid)
            {
                Achat newAchat = new Achat()
                {
                    Details = new List<DetailAchat>(),
                    Fournisseur = achat.Fournisseur,
                    Date = DateTime.Now,
                    NumFacture = achat.NumFactureFournisseur,
                    Id = Guid.NewGuid(),
                    UserId = User.Identity.GetUserId()
                };
                double total = 0;
                foreach (var detail in achat.Details)
                {
                    if (detail.Quantite > 0)
                        newAchat.Details.Add(new DetailAchat()
                        {
                            Produit = db.Produits.Find(detail.ProduitId),
                            QuantiteAchetee = detail.Quantite,
                            ProduitID = detail.ProduitId,
                            ID = Guid.NewGuid()
                        });
                    total += detail.Quantite * db.Produits.Find(detail.ProduitId).PrixAchat;
                }
                newAchat.Montant = total;
                //newAchat.MontantRestant = total;

                db.Operations.Add(newAchat);
                db.SaveChanges();
                //new CaisseManager(db).reglerVente(vente.MontantPaye, newVente);

                return RedirectToAction("Index");
            }

            return View(achat);
        }

        // GET: Achats/Create
        public ActionResult Create()
        {
            var achatVM = new AchatCreateVM() { Details = new List<DetailAchatVM>() };

            foreach (var produit in db.Produits.ToList())
            {
                achatVM.Details.Add(new DetailAchatVM()
                {
                    NomProduit = produit.Nom,
                    ProduitId = produit.ID,
                    Prix = 0,
                    Quantite = 0
                });
            }
            return View(achatVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AchatCreateVM achatVM)
        {
            if (ModelState.IsValid)
            {
                //Mettre a jour les infos vides et affecter le stock

                var achat = new Achat()
                {
                    Id = Guid.NewGuid(),
                    Montant = achatVM.MontantPaye,
                    Date = DateTime.Now,
                    Fournisseur = achatVM.Fournisseur,
                    NumFacture = achatVM.NumFactureFournisseur,
                    Details = new List<DetailAchat>(),
                    UserId = User.Identity.GetUserId()
                };

                foreach (var detailVM in achatVM.Details)
                {
                    if (detailVM.Quantite > 0)
                    {
                        var produit = db.Produits.Find(detailVM.ProduitId);

                        var detail = new DetailAchat()
                        {
                            ID = Guid.NewGuid(),
                            AchatId = achat.Id,
                            ProduitID = produit.ID,
                            Produit = produit,
                            PrixAchat = detailVM.Prix,
                            QuantiteAchetee = detailVM.Quantite
                        };

                        achat.Details.Add(detail);

                        stockManager.AddStock(produit.ID, detail.QuantiteAchetee, OperationType.Achat);
                    }
                }
                db.Achats.Add(achat);
                //db.Operations.Add(achat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(achatVM);

        }

        public ActionResult Details(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Achat achat = db.Operations.OfType<Achat>().Include(a => a.Details).First(a => a.Id == id);
            if (achat == null)
            {
                return HttpNotFound();
            }
            return View(achat);
        }

        public ActionResult Delete (Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Achat achat = db.Operations.OfType<Achat>().Include(a => a.Details).First(a => a.Id == id);
            if (achat == null)
            {
                return HttpNotFound();
            }

            return View(achat);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Achat achat = db.Operations.OfType<Achat>().Include(a => a.Details).FirstOrDefault(a => a.Id == id);
            foreach (var detail in achat.Details)
            {
                stockManager.RemoveStock(detail.Produit.ID, detail.QuantiteAchetee, OperationType.Vente);
            }
            db.Operations.Remove(achat);
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