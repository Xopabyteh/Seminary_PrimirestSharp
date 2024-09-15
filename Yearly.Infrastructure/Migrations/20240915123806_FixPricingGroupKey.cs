using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yearly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPricingGroupKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPricingGroups",
                schema: "Domain",
                table: "UserPricingGroups");

            migrationBuilder.DropIndex(
                name: "IX_UserPricingGroups_UserId",
                schema: "Domain",
                table: "UserPricingGroups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPricingGroups",
                schema: "Domain",
                table: "UserPricingGroups",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPricingGroups",
                schema: "Domain",
                table: "UserPricingGroups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPricingGroups",
                schema: "Domain",
                table: "UserPricingGroups",
                column: "PricingGroupEnum");

            migrationBuilder.CreateIndex(
                name: "IX_UserPricingGroups_UserId",
                schema: "Domain",
                table: "UserPricingGroups",
                column: "UserId",
                unique: true);
        }
    }
}
