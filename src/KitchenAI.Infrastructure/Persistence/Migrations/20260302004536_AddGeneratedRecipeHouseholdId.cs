using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KitchenAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneratedRecipeHouseholdId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HouseholdId",
                table: "GeneratedRecipes",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "LlmResponseJson",
                table: "GeneratedRecipes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromptTemplate",
                table: "GeneratedRecipes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedRecipes_HouseholdId",
                table: "GeneratedRecipes",
                column: "HouseholdId");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneratedRecipes_Households_HouseholdId",
                table: "GeneratedRecipes",
                column: "HouseholdId",
                principalTable: "Households",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneratedRecipes_Households_HouseholdId",
                table: "GeneratedRecipes");

            migrationBuilder.DropIndex(
                name: "IX_GeneratedRecipes_HouseholdId",
                table: "GeneratedRecipes");

            migrationBuilder.DropColumn(
                name: "HouseholdId",
                table: "GeneratedRecipes");

            migrationBuilder.DropColumn(
                name: "LlmResponseJson",
                table: "GeneratedRecipes");

            migrationBuilder.DropColumn(
                name: "PromptTemplate",
                table: "GeneratedRecipes");
        }
    }
}
