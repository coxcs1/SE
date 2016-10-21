namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateSectionModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Semester = c.Int(nullable: false),
                        AcademicYear = c.Int(nullable: false),
                        CourseID = c.Int(nullable: false),
                        TeacherID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TeacherID, cascadeDelete: true)
                .Index(t => t.CourseID)
                .Index(t => t.TeacherID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sections", "TeacherID", "dbo.Teachers");
            DropForeignKey("dbo.Sections", "CourseID", "dbo.Courses");
            DropIndex("dbo.Sections", new[] { "TeacherID" });
            DropIndex("dbo.Sections", new[] { "CourseID" });
            DropTable("dbo.Sections");
        }
    }
}
