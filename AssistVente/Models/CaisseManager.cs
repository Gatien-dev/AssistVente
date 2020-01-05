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
        public void reglerVente(double montant, Vente vente, double montantRecu = 0, double montantRendu = 0)
        {
            var caisseParDefaut = getCaisseParDefaut();
            if (montant > vente.MontantRestant || montant <= 0) return;
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
                MontantRendu = montantRendu
            });
            db.Ventes.First(v => v.Id == vente.Id).MontantRegle += montant;
            db.Ventes.First(v => v.Id == vente.Id).MontantRestant -= montant;
            db.SaveChanges();
        }
        public Caisse getCaisseParDefaut()
        {
            return db.Caisses.Include(c=>c.Reglements).First();
        }
    }
}