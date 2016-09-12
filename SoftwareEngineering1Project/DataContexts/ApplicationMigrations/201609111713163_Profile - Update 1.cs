namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProfileUpdate1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Profiles", "UserEmail", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Profiles", "UserEmail", c => c.Int(nullable: false));
        }
    }
}
