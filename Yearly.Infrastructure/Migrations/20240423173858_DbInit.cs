using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yearly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DbInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Domain");

            migrationBuilder.EnsureSchema(
                name: "Infrastructure");

            migrationBuilder.CreateTable(
                name: "Foods",
                schema: "Domain",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AliasForFoodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Allergens = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PrimirestFoodIdentifier_DayId = table.Column<int>(type: "int", nullable: false),
                    PrimirestFoodIdentifier_ItemId = table.Column<int>(type: "int", nullable: false),
                    PrimirestFoodIdentifier_MenuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "Infrastructure",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                schema: "Domain",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResourceLink = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ThumbnailResourceLink = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    FoodId_Value = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublisherId_Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Domain",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyMenus",
                schema: "Domain",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyMenus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodPhotoIds",
                schema: "Domain",
                columns: table => new
                {
                    PhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPhotoIds", x => x.PhotoId);
                    table.ForeignKey(
                        name: "FK_FoodPhotoIds_Foods_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "Domain",
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPhotoIds",
                schema: "Domain",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhotoIds", x => new { x.UserId, x.Id });
                    table.ForeignKey(
                        name: "FK_UserPhotoIds_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Domain",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "Domain",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.Id });
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Domain",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyMenus",
                schema: "Domain",
                columns: table => new
                {
                    WeeklyMenuId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyMenus", x => new { x.WeeklyMenuId, x.Id });
                    table.ForeignKey(
                        name: "FK_DailyMenus_WeeklyMenus_WeeklyMenuId",
                        column: x => x.WeeklyMenuId,
                        principalSchema: "Domain",
                        principalTable: "WeeklyMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuFoodIds",
                schema: "Domain",
                columns: table => new
                {
                    FoodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailyMenuId = table.Column<int>(type: "int", nullable: false),
                    DailyMenuWeeklyMenuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuFoodIds", x => x.FoodId);
                    table.ForeignKey(
                        name: "FK_MenuFoodIds_DailyMenus_DailyMenuWeeklyMenuId_DailyMenuId",
                        columns: x => new { x.DailyMenuWeeklyMenuId, x.DailyMenuId },
                        principalSchema: "Domain",
                        principalTable: "DailyMenus",
                        principalColumns: new[] { "WeeklyMenuId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodPhotoIds_FoodId",
                schema: "Domain",
                table: "FoodPhotoIds",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuFoodIds_DailyMenuWeeklyMenuId_DailyMenuId",
                schema: "Domain",
                table: "MenuFoodIds",
                columns: new[] { "DailyMenuWeeklyMenuId", "DailyMenuId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodPhotoIds",
                schema: "Domain");

            migrationBuilder.DropTable(
                name: "FoodSimilarities",
                schema: "Domain");

            migrationBuilder.DropTable(
                name: "MenuFoodIds",
                schema: "Domain");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "Infrastructure");

            migrationBuilder.DropTable(
                name: "Photos",
                schema: "Domain");

            migrationBuilder.DropTable(
                name: "UserPhotoIds",
                schema: "Domain");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Domain");

            migrationBuilder.DropTable(
                name: "Foods",
                schema: "Domain");

            migrationBuilder.DropTable(
                name: "DailyMenus",
                schema: "Domain");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Domain");

            migrationBuilder.DropTable(
                name: "WeeklyMenus",
                schema: "Domain");
        }
    }
}
