using Microsoft.EntityFrameworkCore.Migrations;

namespace ABV_Invest.Data.Migrations
{
    public partial class AddCurrencyCodeInBalanceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Balances_Currencies_CurrencyId",
                table: "Balances");

            migrationBuilder.DropIndex(
                name: "IX_Balances_CurrencyId",
                table: "Balances");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Balances");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Balances",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Balances");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Balances",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Balances_CurrencyId",
                table: "Balances",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Balances_Currencies_CurrencyId",
                table: "Balances",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
