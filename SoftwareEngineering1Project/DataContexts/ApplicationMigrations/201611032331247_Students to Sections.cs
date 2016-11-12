namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentstoSections : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentSections",
                c => new
                    {
                        Student_ID = c.Int(nullable: false),
                        Section_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Student_ID, t.Section_ID })
                .ForeignKey("dbo.Students", t => t.Student_ID, cascadeDelete: true)
                .ForeignKey("dbo.Sections", t => t.Section_ID, cascadeDelete: true)
                .Index(t => t.Student_ID)
                .Index(t => t.Section_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentSections", "Section_ID", "dbo.Sections");
            DropForeignKey("dbo.StudentSections", "Student_ID", "dbo.Students");
            DropIndex("dbo.StudentSections", new[] { "Section_ID" });
            DropIndex("dbo.StudentSections", new[] { "Student_ID" });
            DropTable("dbo.StudentSections");
        }
    }
}
