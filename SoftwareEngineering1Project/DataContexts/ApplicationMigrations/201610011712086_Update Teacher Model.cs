namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeacherModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teachers", "FirstName", c => c.String(nullable: false));
            AddColumn("dbo.Teachers", "LastName", c => c.String(nullable: false));
            DropColumn("dbo.Teachers", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Teachers", "Name", c => c.String(nullable: false));
            DropColumn("dbo.Teachers", "LastName");
            DropColumn("dbo.Teachers", "FirstName");
        }
    }
}
