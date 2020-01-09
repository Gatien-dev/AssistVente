using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace AssistVente.Models
{
    public class CaisseManager
    {
        AssistVenteContext db;
        public CaisseManager(AssistVenteContext context)
        {
            if (context == null)
                db = new AssistVenteContext();
            else
                db = context;
        }

        public void reglerVente(double montant, Vente vente, string raison, string modeReglement, double montantRecu = 0, double montantRendu = 0)
        {
            var caisseParDefaut = getCaisseParDefaut();
            if (montant <= 0) return;
            if (vente.MontantRestant < montant) montant = vente.MontantRestant;
            caisseParDefaut.Solde += montant;
            if (caisseParDefaut.Reglements == null) caisseParDefaut.Reglements = new List<Reglement>();
            caisseParDefaut.Reglements.Add(new Reglement()
            {
                Id=Guid.NewGuid(),
                Caisse = caisseParDefaut,
                CaisseId = caisseParDefaut.ID,
                Date=DateTime.Now,
                IdOperation = vente.Id,
                MontantRegle = montant,
                MontantRecu = montantRecu,
                MontantRendu = montantRendu,
                Raison=raison,
                ModeReglement=modeReglement
            });
            db.Ventes.First(v => v.Id == vente.Id).MontantRegle += montant;
            db.Ventes.First(v => v.Id == vente.Id).MontantRestant -= montant;
            db.SaveChanges();
        }

        public void reglerLocation(double montant, Location location, string raison, string modeReglement, double montantRecu = 0, double montantRendu = 0)
        {
            var caisseParDefaut = getCaisseParDefaut();
            if (montant <= 0) return;
            if (location.MontantRestant < montant) montant = location.MontantRestant;
            caisseParDefaut.Solde += montant;
            if (caisseParDefaut.Reglements == null) caisseParDefaut.Reglements = new List<Reglement>();
            caisseParDefaut.Reglements.Add(new Reglement()
            {
                Id = Guid.NewGuid(),
                Caisse = caisseParDefaut,
                CaisseId = caisseParDefaut.ID,
                Date = DateTime.Now,
                IdOperation = location.Id,
                MontantRegle = montant,
                MontantRecu = montantRecu,
                MontantRendu = montantRendu,
                Raison = raison,
                ModeReglement = modeReglement
            });
            db.Locations.First(v => v.Id == location.Id).MontantPaye += montant;
            db.Locations.First(v => v.Id == location.Id).MontantRestant -= montant;
            db.SaveChanges();
        }

        public void reglerAbonnement(double montant, Abonnement abonnement, string raison, string modeReglement)
        {
            var caisseParDefaut = getCaisseParDefaut();
            //if (montant <= 0) return;
            //if (abonnement.ResteAPayer < montant) montant = abonnement.ResteAPayer;

            caisseParDefaut.Solde += montant;

            if (caisseParDefaut.Reglements == null) caisseParDefaut.Reglements = new List<Reglement>();

            caisseParDefaut.Reglements.Add(new Reglement() {
                Id = Guid.NewGuid(),
                Caisse = caisseParDefaut,
                CaisseId = caisseParDefaut.ID,
                Date = DateTime.Now,
                IdOperation = abonnement.Id,
                MontantRegle = montant,
                MontantRecu = 0,
                MontantRendu = 0,
                Raison = raison,
                ModeReglement = modeReglement
            });

            //db.Abonnements.First(a => a.Id == abonnement.Id).SommePaye += montant;
            //db.Abonnements.First(a => a.Id == abonnement.Id).ResteAPayer -= montant;
            db.SaveChanges();
        }

        public Caisse getCaisseParDefaut()
        {
            return db.Caisses.Include(c=>c.Reglements).First();
        }
    }

    public enum OperationCaisseType
    {
        Initialisation, Reinitialisation
    }
}