using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yearly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ToNET8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublisherId",
                schema: "Domain",
                table: "Photos",
                newName: "PublisherId_Value");

            migrationBuilder.RenameColumn(
                name: "FoodId",
                schema: "Domain",
                table: "Photos",
                newName: "FoodId_Value");

            migrationBuilder.RenameColumn(
                name: "PrimirestMenuId",
                schema: "Domain",
                table: "Foods",
                newName: "PrimirestFoodIdentifier_MenuId");

            migrationBuilder.RenameColumn(
                name: "PrimirestItemId",
                schema: "Domain",
                table: "Foods",
                newName: "PrimirestFoodIdentifier_ItemId");

            migrationBuilder.RenameColumn(
                name: "PrimirestDayId",
                schema: "Domain",
                table: "Foods",
                newName: "PrimirestFoodIdentifier_DayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublisherId_Value",
                schema: "Domain",
                table: "Photos",
                newName: "PublisherId");

            migrationBuilder.RenameColumn(
                name: "FoodId_Value",
                schema: "Domain",
                table: "Photos",
                newName: "FoodId");

            migrationBuilder.RenameColumn(
                name: "PrimirestFoodIdentifier_MenuId",
                schema: "Domain",
                table: "Foods",
                newName: "PrimirestMenuId");

            migrationBuilder.RenameColumn(
                name: "PrimirestFoodIdentifier_ItemId",
                schema: "Domain",
                table: "Foods",
                newName: "PrimirestItemId");

            migrationBuilder.RenameColumn(
                name: "PrimirestFoodIdentifier_DayId",
                schema: "Domain",
                table: "Foods",
                newName: "PrimirestDayId");
        }
    }
}
