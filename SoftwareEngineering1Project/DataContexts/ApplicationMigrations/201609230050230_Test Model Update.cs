namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestModelUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Questions", "Test_ID", "dbo.Tests");
            DropIndex("dbo.Questions", new[] { "Test_ID" });
            DropColumn("dbo.Questions", "Test_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Questions", "Test_ID", c => c.Int());
            CreateIndex("dbo.Questions", "Test_ID");
            AddForeignKey("dbo.Questions", "Test_ID", "dbo.Tests", "ID");
        }
    }
}
