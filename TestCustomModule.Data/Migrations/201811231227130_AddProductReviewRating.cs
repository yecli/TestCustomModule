namespace TestCustomModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductReviewRating : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductRating",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ProductId = c.String(nullable: false, maxLength: 128),
                        Rating = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
			CreateIndex("dbo.ProductRating", "ProductId", name: "IX_ProductRating_ProductId");

		}

		public override void Down()
        {
			DropIndex("dbo.ProductRating", "IX_ProductRating_ProductId");
			DropTable("dbo.ProductRating");
        }
    }
}
