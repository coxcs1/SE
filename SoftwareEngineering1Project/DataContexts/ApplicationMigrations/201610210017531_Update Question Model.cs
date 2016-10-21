namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateQuestionModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Questions", "TeacherID", "dbo.Teachers");
            DropIndex("dbo.Questions", new[] { "TeacherID" });
            DropColumn("dbo.Questions", "TeacherID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Questions", "TeacherID", c => c.Int(nullable: false));
            CreateIndex("dbo.Questions", "TeacherID");
            AddForeignKey("dbo.Questions", "TeacherID", "dbo.Teachers", "ID", cascadeDelete: true);
        }
    }
}
