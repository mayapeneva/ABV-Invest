using Microsoft.EntityFrameworkCore.Migrations;

namespace ABV_Invest.Data.Migrations
{
    public partial class AddIdsOfRelevantCollectionEntitiesInDealAndSecuritiesPerClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_DailyDeals_DailyDealsId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_SecuritiesPerClient_DailySecuritiesPerClient_DailySecuritiesPerClientId",
                table: "SecuritiesPerClient");

            migrationBuilder.AlterColumn<int>(
                name: "DailySecuritiesPerClientId",
                table: "SecuritiesPerClient",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DailyDealsId",
                table: "Deals",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_DailyDeals_DailyDealsId",
                table: "Deals",
                column: "DailyDealsId",
                principalTable: "DailyDeals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SecuritiesPerClient_DailySecuritiesPerClient_DailySecuritiesPerClientId",
                table: "SecuritiesPerClient",
                column: "DailySecuritiesPerClientId",
                principalTable: "DailySecuritiesPerClient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_DailyDeals_DailyDealsId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_SecuritiesPerClient_DailySecuritiesPerClient_DailySecuritiesPerClientId",
                table: "SecuritiesPerClient");

            migrationBuilder.AlterColumn<int>(
                name: "DailySecuritiesPerClientId",
                table: "SecuritiesPerClient",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "DailyDealsId",
                table: "Deals",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_DailyDeals_DailyDealsId",
                table: "Deals",
                column: "DailyDealsId",
                principalTable: "DailyDeals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SecuritiesPerClient_DailySecuritiesPerClient_DailySecuritiesPerClientId",
                table: "SecuritiesPerClient",
                column: "DailySecuritiesPerClientId",
                principalTable: "DailySecuritiesPerClient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
