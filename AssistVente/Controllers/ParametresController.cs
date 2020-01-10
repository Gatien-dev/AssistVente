using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AssistVente.Models;
using Hangfire;

namespace AssistVente.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ParametresController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();

        // GET: Parametres
        public ActionResult Index()
        {
            return View(db.Parametres.ToList());
        }

        // GET: Parametres/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parametre parametre = db.Parametres.Find(id);
            if (parametre == null)
            {
                return HttpNotFound();
            }
            return View(parametre);
        }

        // GET: Parametres/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Parametres/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EmailNotifications,HourNotifications")] Parametre parametre)
        {
            if (ModelState.IsValid)
            {
                parametre.Id = Guid.NewGuid();
                if (parametre.HourNotifications > 24) parametre.HourNotifications = parametre.HourNotifications % 24;
                db.Parametres.Add(parametre);
                db.SaveChanges();
                //RecurringJob.AddOrUpdate("chargement", () => cm.load(), Cron.MinuteInterval(20))
                RecurringJob.AddOrUpdate("notifications", () => Utilities.SendNotifications(), Cron.Daily(parametre.HourNotifications));

                return RedirectToAction("Index");
            }

            return View(parametre);
        }

        // GET: Parametres/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parametre parametre = db.Parametres.Find(id);
            if (parametre == null)
            {
                return HttpNotFound();
            }
            return View(parametre);
        }

        // POST: Parametres/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EmailNotifications,HourNotifications")] Parametre parametre)
        {
            if (ModelState.IsValid)
            {
                db.Entry(parametre).State = EntityState.Modified;
                if (parametre.HourNotifications > 24) parametre.HourNotifications = parametre.HourNotifications % 24;
                db.SaveChanges();
                RecurringJob.AddOrUpdate("notifications", () => Utilities.SendNotifications(), Cron.Daily(parametre.HourNotifications));
                return RedirectToAction("Index");
            }
            return View(parametre);
        }

        // GET: Parametres/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parametre parametre = db.Parametres.Find(id);
            if (parametre == null)
            {
                return HttpNotFound();
            }
            return View(parametre);
        }

        // POST: Parametres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Parametre parametre = db.Parametres.Find(id);
            db.Parametres.Remove(parametre);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult InitialiseData()
        {
            return RedirectToAction("Index", "Home", null);
        }
        public void InsertInitializationData()
        {
            db.Caisses.Add(new Caisse() { ID = Guid.NewGuid(), Nom = "Caisse Principale", Solde = 0 });
            db.SaveChanges();

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
