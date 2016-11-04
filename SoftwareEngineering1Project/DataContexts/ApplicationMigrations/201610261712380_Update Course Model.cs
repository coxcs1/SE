namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCourseModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "DeptAbbreviation", c => c.String(nullable: false, maxLength: 4));
            DropColumn("dbo.Courses", "CourseCRN");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courses", "CourseCRN", c => c.Int(nullable: false));
            DropColumn("dbo.Courses", "DeptAbbreviation");
        }
    }
}
