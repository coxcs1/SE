namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateQuestionModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Questions", "CourseID", "dbo.Courses");
            DropIndex("dbo.Questions", new[] { "CourseID" });
            AddColumn("dbo.Questions", "SectionID", c => c.Int(nullable: false));
            CreateIndex("dbo.Questions", "SectionID");
            AddForeignKey("dbo.Questions", "SectionID", "dbo.Sections", "ID", cascadeDelete: false);
            DropColumn("dbo.Questions", "CourseID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Questions", "CourseID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Questions", "SectionID", "dbo.Sections");
            DropIndex("dbo.Questions", new[] { "SectionID" });
            DropColumn("dbo.Questions", "SectionID");
            CreateIndex("dbo.Questions", "CourseID");
            AddForeignKey("dbo.Questions", "CourseID", "dbo.Courses", "ID", cascadeDelete: true);
        }
    }
}
