namespace AzureSQLPerfTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveToStringDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tests", "DateOfBirth", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tests", "DateOfBirth", c => c.DateTime(nullable: false));
        }
    }
}
