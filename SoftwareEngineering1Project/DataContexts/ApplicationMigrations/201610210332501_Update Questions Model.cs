namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateQuestionsModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "Archived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Questions", "Archived");
        }
    }
}
