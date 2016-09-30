namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentModelUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Students", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Students", "Notes", c => c.String(nullable: false));
        }
    }
}
