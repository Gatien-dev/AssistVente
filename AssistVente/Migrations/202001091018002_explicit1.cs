namespace AssistVente.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class explicit1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Produits", "DureeDeLocationParDefaut");
            AddColumn("dbo.Produits", "DureeDeLocationParDefaut", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Produits", "DureeDeLocationParDefaut");
        }
    }
}
