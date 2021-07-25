namespace Bookkeeping.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class typeremoefromrecontabe : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Reconciliations", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reconciliations", "Type", c => c.Int(nullable: false));
        }
    }
}
