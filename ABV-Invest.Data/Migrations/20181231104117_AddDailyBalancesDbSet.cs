using Microsoft.EntityFrameworkCore.Migrations;

namespace ABV_Invest.Data.Migrations
{
    public partial class AddDailyBalancesDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyBalance_AbvInvestUsers_AbvInvestUserId",
                table: "DailyBalance");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyBalance_Balances_BalanceId",
                table: "DailyBalance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyBalance",
                table: "DailyBalance");

            migrationBuilder.RenameTable(
                name: "DailyBalance",
                newName: "DailyBalances");

            migrationBuilder.RenameIndex(
                name: "IX_DailyBalance_BalanceId",
                table: "DailyBalances",
                newName: "IX_DailyBalances_BalanceId");

            migrationBuilder.RenameIndex(
                name: "IX_DailyBalance_AbvInvestUserId",
                table: "DailyBalances",
                newName: "IX_DailyBalances_AbvInvestUserId");

            migrationBuilder.AddColumn<string>(
                name: "MIC",
                table: "Markets",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyBalances",
                table: "DailyBalances",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyBalances_AbvInvestUsers_AbvInvestUserId",
                table: "DailyBalances",
                column: "AbvInvestUserId",
                principalTable: "AbvInvestUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyBalances_Balances_BalanceId",
                table: "DailyBalances",
                column: "BalanceId",
                principalTable: "Balances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyBalances_AbvInvestUsers_AbvInvestUserId",
                table: "DailyBalances");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyBalances_Balances_BalanceId",
                table: "DailyBalances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyBalances",
                table: "DailyBalances");

            migrationBuilder.DropColumn(
                name: "MIC",
                table: "Markets");

            migrationBuilder.RenameTable(
                name: "DailyBalances",
                newName: "DailyBalance");

            migrationBuilder.RenameIndex(
                name: "IX_DailyBalances_BalanceId",
                table: "DailyBalance",
                newName: "IX_DailyBalance_BalanceId");

            migrationBuilder.RenameIndex(
                name: "IX_DailyBalances_AbvInvestUserId",
                table: "DailyBalance",
                newName: "IX_DailyBalance_AbvInvestUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyBalance",
                table: "DailyBalance",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyBalance_AbvInvestUsers_AbvInvestUserId",
                table: "DailyBalance",
                column: "AbvInvestUserId",
                principalTable: "AbvInvestUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyBalance_Balances_BalanceId",
                table: "DailyBalance",
                column: "BalanceId",
                principalTable: "Balances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
