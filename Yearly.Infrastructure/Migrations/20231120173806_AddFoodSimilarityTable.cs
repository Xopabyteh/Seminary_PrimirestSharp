using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yearly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodSimilarityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Domain");

            migrationBuilder.RenameTable(
                name: "WeeklyMenus",
                newName: "WeeklyMenus",
                newSchema: "Domain");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "Domain");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UserRoles",
                newSchema: "Domain");

            migrationBuilder.RenameTable(
                name: "UserPhotoIds",
                newName: "UserPhotoIds",
                newSchema: "Domain");

            migrationBuilder.RenameTable(
                name: "Photos",
                newName: "Photos",
                newSchema: "Domain");

            migrationBuilder.RenameTable(
                name: "MenuFoodIds",
                newName: "MenuFoodIds",
                newSchema: "Domain");

            migrationBuilder.RenameTable(
                name: "Foods",
                newName: "Foods",
                newSchema: "Domain");

            migrationBuilder.RenameTable(
                name: "FoodPhotoIds",
                newName: "FoodPhotoIds",
                newSchema: "Domain");

            migrationBuilder.RenameTable(
                name: "DailyMenus",
                newName: "DailyMenus",
                newSchema: "Domain");

            migrationBuilder.CreateTable(
                name: "FoodSimilarities",
                schema: "Domain",
                columns: table => new
                {
                    NewlyPersistedFoodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PotentialAliasOriginId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Similarity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodSimilarities", x => new { x.NewlyPersistedFoodId, x.PotentialAliasOriginId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodSimilarities",
                schema: "Domain");

            migrationBuilder.RenameTable(
                name: "WeeklyMenus",
                schema: "Domain",
                newName: "WeeklyMenus");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "Domain",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "Domain",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "UserPhotoIds",
                schema: "Domain",
                newName: "UserPhotoIds");

            migrationBuilder.RenameTable(
                name: "Photos",
                schema: "Domain",
                newName: "Photos");

            migrationBuilder.RenameTable(
                name: "MenuFoodIds",
                schema: "Domain",
                newName: "MenuFoodIds");

            migrationBuilder.RenameTable(
                name: "Foods",
                schema: "Domain",
                newName: "Foods");

            migrationBuilder.RenameTable(
                name: "FoodPhotoIds",
                schema: "Domain",
                newName: "FoodPhotoIds");

            migrationBuilder.RenameTable(
                name: "DailyMenus",
                schema: "Domain",
                newName: "DailyMenus");
        }
    }
}
