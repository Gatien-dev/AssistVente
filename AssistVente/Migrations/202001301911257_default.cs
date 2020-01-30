namespace AssistVente.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _default : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DetailVentes", "VenteId", "dbo.Operations");
            DropIndex("dbo.DetailVentes", new[] { "VenteId" });
            AddColumn("dbo.Operations", "DateOperation", c => c.DateTime());
            AlterColumn("dbo.DetailVentes", "VenteId", c => c.Guid(nullable: false));
            CreateIndex("dbo.DetailVentes", "VenteId");
            AddForeignKey("dbo.DetailVentes", "VenteId", "dbo.Operations", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DetailVentes", "VenteId", "dbo.Operations");
            DropIndex("dbo.DetailVentes", new[] { "VenteId" });
            AlterColumn("dbo.DetailVentes", "VenteId", c => c.Guid());
            DropColumn("dbo.Operations", "DateOperation");
            CreateIndex("dbo.DetailVentes", "VenteId");
            AddForeignKey("dbo.DetailVentes", "VenteId", "dbo.Operations", "Id");
        }
    }
}
