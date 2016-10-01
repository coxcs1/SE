namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTeacherModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ENumber = c.String(nullable: false),
                        Title = c.String(nullable: false),
                        Status = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Questions", "TeacherID", c => c.Int(nullable: false));
            CreateIndex("dbo.Questions", "TeacherID");
            AddForeignKey("dbo.Questions", "TeacherID", "dbo.Teachers", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Questions", "TeacherID", "dbo.Teachers");
            DropIndex("dbo.Questions", new[] { "TeacherID" });
            DropColumn("dbo.Questions", "TeacherID");
            DropTable("dbo.Teachers");
        }
    }
}
