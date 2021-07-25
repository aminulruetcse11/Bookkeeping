namespace Bookkeeping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initdb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Heads",
                c => new
                    {
                        HeadID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 300),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HeadID);
            
            CreateTable(
                "dbo.IncomeExpenses",
                c => new
                    {
                        IEID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IEID);
            
            CreateTable(
                "dbo.Reconciliations",
                c => new
                    {
                        RecID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.Int(nullable: false),
                        HeadID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RecID)
                .ForeignKey("dbo.Heads", t => t.HeadID, cascadeDelete: true)
                .Index(t => t.HeadID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reconciliations", "HeadID", "dbo.Heads");
            DropIndex("dbo.Reconciliations", new[] { "HeadID" });
            DropTable("dbo.Reconciliations");
            DropTable("dbo.IncomeExpenses");
            DropTable("dbo.Heads");
        }
    }
}
