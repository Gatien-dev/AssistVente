using AssistVente.Models;
using AssistVente.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AssistVente.Controllers
{
    [Authorize(Roles = "Admin,Achats")]
    public class AchatsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();
        private StockManager stockManager = new StockManager();

        // GET: Achats
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

        public ActionResult Create()
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
                    PU = produit.PrixAchat,
                    Quantite = 0
                });
            }
            return View(achatVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AchatCreateVM achat)
        {
            if (ModelState.IsValid)
            {
                Achat newAchat = new Achat()
                {
                    Details = new List<DetailAchat>(),
                    Fournisseur = achat.Fournisseur,
                    Date = DateTime.Now,
                    NumFacture=achat.NumFactureFournisseur,
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
        public ActionResult CreateAchat()
        {
            var achat = new Achat() { Details = new List<DetailAchat>() };

            foreach (var produit in db.Produits.ToList())
            {
                achat.Details.Add(new DetailAchat() { 
                    ProduitID = produit.ID,
                    Produit = produit,
                    QuantiteAchetee = 0
                });
            }
            return View(achat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAchat(Achat achat)
        {
            if (ModelState.IsValid)
            {
                //Mettre a jour les infos vides et affecter le stock
                achat.Id = Guid.NewGuid();
                //achat.Date = null;
                achat.Date = DateTime.Now;

                foreach (var detail in achat.Details)
                {
                    if (detail.QuantiteAchetee > 0)
                    {
                        var produit = db.Produits.Find(detail.Produit.ID);

                        detail.ID = Guid.NewGuid();
                        detail.AchatId = achat.Id;
                        detail.Achat = achat;
                        detail.ProduitID = detail.Produit.ID;
                        detail.Produit = produit;

                        //db.DetailsAchat.Add(detail);
                        //db.SaveChanges();

                        stockManager.AddStock(produit.ID, detail.QuantiteAchetee, OperationType.Achat);
                    }
                }

                db.Operations.Add(achat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            foreach (var detail in achat.Details)
            {
                detail.Produit = db.Produits.Find(detail.Produit.ID);
            }

            return View(achat);
            
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