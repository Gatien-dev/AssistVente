using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AssistVente.Models
{
    public class MainDbInitializer : DropCreateDatabaseIfModelChanges<AssistVenteContext>
    {
        protected override void Seed(AssistVenteContext context)
        {
            context.Caisses.Add(new Caisse()
            {
                ID = Guid.NewGuid(),
                Nom = "Caisse Principale",
                Operations = new List<Operation>(),
                Solde = 0
            });
            base.Seed(context);
        }
    }
    public class AssistVenteContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        //
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public AssistVenteContext() : base("name=AssistVenteContext")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produit>().HasMany(p => p.DetailsAchat).WithRequired(d => d.Produit).WillCascadeOnDelete(false);
            modelBuilder.Entity<Produit>().HasMany(p => p.DetailsVente).WithRequired(d => d.Produit).WillCascadeOnDelete(false);
        }

        public System.Data.Entity.DbSet<AssistVente.Models.Produit> Produits { get; set; }

        public System.Data.Entity.DbSet<AssistVente.Models.Client> Clients { get; set; }

        public System.Data.Entity.DbSet<AssistVente.Models.Forfait> Forfaits { get; set; }

        public System.Data.Entity.DbSet<AssistVente.Models.Caisse> Caisses { get; set; }

        public System.Data.Entity.DbSet<AssistVente.Models.Abonnement> Abonnements { get; set; }

        public System.Data.Entity.DbSet<AssistVente.Models.Achat> Achats { get; set; }

        public System.Data.Entity.DbSet<AssistVente.Models.Location> Locations { get; set; }

        //public System.Data.Entity.DbSet<AssistVente.Models.Magasin> Magasins { get; set; }

        public System.Data.Entity.DbSet<AssistVente.Models.Operation> Operations { get; set; }
        
        public System.Data.Entity.DbSet<AssistVente.Models.StockLog> StockLogs { get; set; }

        public System.Data.Entity.DbSet<AssistVente.Models.Reglement> Reglements { get; set; }

        public System.Data.Entity.DbSet<AssistVente.Models.Vente> Ventes { get; set; }

        public DbSet<DetailAchat> DetailsAchat { get; set; }

        public DbSet<DetailVente> DetailsVente { get; set; }
    }
}
