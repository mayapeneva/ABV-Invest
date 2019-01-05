namespace ABV_Invest.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddCouponInDealAndTotalPriceBuyInBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Coupon",
                table: "Deals",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPriceInBGN",
                table: "Deals",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AllSecuritiesTotalPriceBuy",
                table: "Balances",
                type: "decimal(18, 4)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coupon",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "TotalPriceInBGN",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "AllSecuritiesTotalPriceBuy",
                table: "Balances");
        }
    }
}