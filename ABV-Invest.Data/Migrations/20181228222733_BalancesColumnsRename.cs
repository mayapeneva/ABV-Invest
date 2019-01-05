namespace ABV_Invest.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class BalancesColumnsRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllSecuritiesMarketPrice",
                table: "Balances",
                newName: "AllSecuritiesTotalMarketPrice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllSecuritiesTotalMarketPrice",
                table: "Balances",
                newName: "AllSecuritiesMarketPrice");
        }
    }
}