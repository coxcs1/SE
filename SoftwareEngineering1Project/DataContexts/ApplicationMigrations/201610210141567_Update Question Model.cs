namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateQuestionModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Questions", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.Questions", "TeacherID", "dbo.Teachers");
            DropIndex("dbo.Questions", new[] { "CourseID" });
            DropIndex("dbo.Questions", new[] { "TeacherID" });
            AddColumn("dbo.Questions", "SectionID", c => c.Int(nullable: false));
            CreateIndex("dbo.Questions", "SectionID");
            AddForeignKey("dbo.Questions", "SectionID", "dbo.Sections", "ID", cascadeDelete: true);
            DropColumn("dbo.Questions", "CourseID");
            DropColumn("dbo.Questions", "TeacherID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Questions", "TeacherID", c => c.Int(nullable: false));
            AddColumn("dbo.Questions", "CourseID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Questions", "SectionID", "dbo.Sections");
            DropIndex("dbo.Questions", new[] { "SectionID" });
            DropColumn("dbo.Questions", "SectionID");
            CreateIndex("dbo.Questions", "TeacherID");
            CreateIndex("dbo.Questions", "CourseID");
            AddForeignKey("dbo.Questions", "TeacherID", "dbo.Teachers", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Questions", "CourseID", "dbo.Courses", "ID", cascadeDelete: true);
        }
    }
}
