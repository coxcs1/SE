namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeacherModel2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Teachers", "Title", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Teachers", "Title", c => c.String(nullable: false));
        }
    }
}
