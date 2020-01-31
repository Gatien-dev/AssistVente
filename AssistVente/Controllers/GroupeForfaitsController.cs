using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using AssistVente.Models;

namespace AssistVente.Controllers
{
    [Authorize(Roles = "Admin,Groupes de forfaits")]
    public class GroupeForfaitsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();

        // GET: GroupeForfaits
        public ActionResult Index()
        {
            return View(db.GroupeForfaits.ToList());
        }

        // GET: GroupeForfaits/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupeForfait groupeForfait = db.GroupeForfaits.Include(g=>g.Forfaits.Select(f=>f.Abonnements.Select(a=>a.Client))).First(f=>f.Id==id);
            if (groupeForfait == null)
            {
                return HttpNotFound();
            }
            return View(groupeForfait);
        }

        // GET: GroupeForfaits/Create
        [Authorize(Roles = "Admin,Groupes de forfaits-edition")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: GroupeForfaits/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nom,Description")] GroupeForfait groupeForfait)
        {
            if (ModelState.IsValid)
            {
                groupeForfait.Id = Guid.NewGuid();
                db.GroupeForfaits.Add(groupeForfait);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(groupeForfait);
        }

        // GET: GroupeForfaits/Edit/5
        [Authorize(Roles = "Admin,Groupes de forfaits-edition")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupeForfait groupeForfait = db.GroupeForfaits.Find(id);
            if (groupeForfait == null)
            {
                return HttpNotFound();
            }
            return View(groupeForfait);
        }

        // POST: GroupeForfaits/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nom,Description")] GroupeForfait groupeForfait)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupeForfait).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(groupeForfait);
        }

        // GET: GroupeForfaits/Delete/5
        [Authorize(Roles = "Admin,Groupes de forfaits-suppression")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupeForfait groupeForfait = db.GroupeForfaits.Find(id);
            if (groupeForfait == null)
            {
                return HttpNotFound();
            }
            return View(groupeForfait);
        }

        // POST: GroupeForfaits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            GroupeForfait groupeForfait = db.GroupeForfaits.Find(id);
            db.GroupeForfaits.Remove(groupeForfait);
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
