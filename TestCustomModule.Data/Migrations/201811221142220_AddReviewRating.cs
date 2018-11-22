namespace TestCustomModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReviewRating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerReview", "Rating", c => c.Int(nullable: false));
			CreateIndex("dbo.CustomerReview", "ProductId", name: "IX_CustomerReview_ProductId");
        }
        
        public override void Down()
        {
			DropIndex("dbo.CustomerReview", "IX_CustomerReview_ProductId");
			DropColumn("dbo.CustomerReview", "Rating");
        }
    }
}
