using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yearly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPricingGroups",
                schema: "Domain",
                columns: table => new
                {
                    PricingGroupEnum = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPricingGroups", x => x.PricingGroupEnum);
                    table.ForeignKey(
                        name: "FK_UserPricingGroups_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Domain",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPricingGroups_UserId",
                schema: "Domain",
                table: "UserPricingGroups",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPricingGroups",
                schema: "Domain");
        }
    }
}
