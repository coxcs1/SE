namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTestArchive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tests", "Archived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tests", "Archived");
        }
    }
}
