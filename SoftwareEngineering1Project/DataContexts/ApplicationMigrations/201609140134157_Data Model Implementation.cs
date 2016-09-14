namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataModelImplementation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CourseName = c.String(nullable: false, maxLength: 50),
                        CourseCRN = c.Int(nullable: false),
                        CourseAttributeNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CourseID = c.Int(nullable: false),
                        ProfileID = c.Int(nullable: false),
                        Text = c.String(nullable: false),
                        Answer = c.String(nullable: false),
                        Test_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.ProfileID, cascadeDelete: true)
                .ForeignKey("dbo.Tests", t => t.Test_ID)
                .Index(t => t.CourseID)
                .Index(t => t.ProfileID)
                .Index(t => t.Test_ID);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        EnterDate = c.DateTime(nullable: false),
                        Concentration = c.String(nullable: false),
                        Notes = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StudentID = c.Int(nullable: false),
                        Passed = c.Boolean(nullable: false),
                        DateTaken = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Students", t => t.StudentID, cascadeDelete: true)
                .Index(t => t.StudentID);
            
            CreateTable(
                "dbo.TestQuestions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TestID = c.Int(nullable: false),
                        QuestionID = c.Int(nullable: false),
                        QuestionScore = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Questions", t => t.QuestionID, cascadeDelete: true)
                .ForeignKey("dbo.Tests", t => t.TestID, cascadeDelete: true)
                .Index(t => t.TestID)
                .Index(t => t.QuestionID);
            
            CreateTable(
                "dbo.ProfileCourses",
                c => new
                    {
                        Profile_Id = c.Int(nullable: false),
                        Course_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Profile_Id, t.Course_ID })
                .ForeignKey("dbo.Profiles", t => t.Profile_Id, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.Course_ID, cascadeDelete: true)
                .Index(t => t.Profile_Id)
                .Index(t => t.Course_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestQuestions", "TestID", "dbo.Tests");
            DropForeignKey("dbo.TestQuestions", "QuestionID", "dbo.Questions");
            DropForeignKey("dbo.Tests", "StudentID", "dbo.Students");
            DropForeignKey("dbo.Questions", "Test_ID", "dbo.Tests");
            DropForeignKey("dbo.Questions", "ProfileID", "dbo.Profiles");
            DropForeignKey("dbo.Questions", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.ProfileCourses", "Course_ID", "dbo.Courses");
            DropForeignKey("dbo.ProfileCourses", "Profile_Id", "dbo.Profiles");
            DropIndex("dbo.ProfileCourses", new[] { "Course_ID" });
            DropIndex("dbo.ProfileCourses", new[] { "Profile_Id" });
            DropIndex("dbo.TestQuestions", new[] { "QuestionID" });
            DropIndex("dbo.TestQuestions", new[] { "TestID" });
            DropIndex("dbo.Tests", new[] { "StudentID" });
            DropIndex("dbo.Questions", new[] { "Test_ID" });
            DropIndex("dbo.Questions", new[] { "ProfileID" });
            DropIndex("dbo.Questions", new[] { "CourseID" });
            DropTable("dbo.ProfileCourses");
            DropTable("dbo.TestQuestions");
            DropTable("dbo.Tests");
            DropTable("dbo.Students");
            DropTable("dbo.Questions");
            DropTable("dbo.Courses");
        }
    }
}
