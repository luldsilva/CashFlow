using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CashFlow.Infrastructure.Migrations
{
    [DbContext(typeof(CashFlowDbContext))]
    [Migration("20260410093000_AddOfficialExpenseCategoriesLink")]
    public partial class AddOfficialExpenseCategoriesLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ExpenseCategoryId",
                table: "FinancialObligations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ExpenseCategoryId",
                table: "Expenses",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialObligations_ExpenseCategoryId",
                table: "FinancialObligations",
                column: "ExpenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialObligations_ExpenseCategories_ExpenseCategoryId",
                table: "FinancialObligations",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialObligations_ExpenseCategories_ExpenseCategoryId",
                table: "FinancialObligations");

            migrationBuilder.DropIndex(
                name: "IX_FinancialObligations_ExpenseCategoryId",
                table: "FinancialObligations");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ExpenseCategoryId",
                table: "FinancialObligations");

            migrationBuilder.DropColumn(
                name: "ExpenseCategoryId",
                table: "Expenses");
        }
    }
}
