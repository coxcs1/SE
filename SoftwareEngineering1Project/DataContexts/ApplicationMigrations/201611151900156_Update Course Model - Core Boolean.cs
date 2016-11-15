namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCourseModelCoreBoolean : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "Core", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "Core");
        }
    }
}
