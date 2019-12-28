using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using AssistVente.Models;

namespace AssistVente.DAO
{
    public class ClientDAO
    {
        public AssistVenteContext db = new AssistVenteContext();

        public List<Client> GetAllClients()
        {
            return db.Clients.ToList();
        }

        public Client GetClientById(Guid? id)
        {
            Client client = db.Clients.Find(id);
            return (client);
        }

        public bool AddClient(Client clt)
        {
            if (GetClientByName(clt) == null)
            {
                clt.ID = Guid.NewGuid();
                db.Clients.Add(clt);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public Client GetClientByName(Client clt)
        {
            Client client = db.Clients.Where(c => c.Nom == clt.Nom).FirstOrDefault();
            return (client);
        }

        public void UpdateClient(Client clt)
        {
            db.Entry(clt).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void DeleteClient(Guid id)
        {
            db.Clients.Remove(GetClientById(id));
            db.SaveChanges();

            //if (GetClientById(id) == null)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
    }
}