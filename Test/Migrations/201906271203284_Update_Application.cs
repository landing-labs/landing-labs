namespace Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Application : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Applications", "IsAnswered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Applications", "IsAnswered");
        }
    }
}
