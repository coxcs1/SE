namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStudentArchiveStudents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "Archived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "Archived");
        }
    }
}
