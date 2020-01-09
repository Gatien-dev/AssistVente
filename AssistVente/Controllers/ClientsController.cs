using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AssistVente.Models;
using AssistVente.DAO;
using AssistVente.Filters;

namespace AssistVente.Controllers
{
    [Authorize]
    [LogFilter]
    public class ClientsController : Controller
    {
        private AssistVenteContext db = new AssistVenteContext();
        private ClientDAO ClientManager = new ClientDAO();

        // GET: Clients
        public ActionResult Index()
        {
            return View(ClientManager.GetAllClients());
        }

        // GET: Clients/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = ClientManager.GetClientById(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nom,Default,Solde,Adresse,Telephone,Email,NomUrgence,AddresseUrgence,TelephoneUrgence,PhotosAllowed,CertficatMedical,DateNaissance,LieuNaiss")] Client client)
        {
            if (ModelState.IsValid)
            {
                if (ClientManager.AddClient(client))
                {
                    return RedirectToAction("Index");
                }
                
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = ClientManager.GetClientById(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nom,Default,Solde,Adresse,Telephone,Email,NomUrgence,AddresseUrgence,TelephoneUrgence,PhotosAllowed,CertficatMedical,DateNaissance,LieuNaiss")] Client client)
        {
            if (ModelState.IsValid)
            {
                ClientManager.UpdateClient(client);
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = ClientManager.GetClientById(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ClientManager.DeleteClient(id);
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
