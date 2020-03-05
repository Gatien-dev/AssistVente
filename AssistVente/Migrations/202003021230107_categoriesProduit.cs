namespace AssistVente.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class categoriesProduit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategorieProduits",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Operations", "NumVente", c => c.Int(identity: true));
            AddColumn("dbo.Produits", "CategorieProduitId", c => c.Int());
            CreateIndex("dbo.Produits", "CategorieProduitId");
            AddForeignKey("dbo.Produits", "CategorieProduitId", "dbo.CategorieProduits", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Produits", "CategorieProduitId", "dbo.CategorieProduits");
            DropIndex("dbo.Produits", new[] { "CategorieProduitId" });
            DropColumn("dbo.Produits", "CategorieProduitId");
            DropColumn("dbo.Operations", "NumVente");
            DropTable("dbo.CategorieProduits");
        }
    }
}
