using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace All4GYM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMealLogModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealLogItems");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "MealLogs",
                newName: "MealType");

            migrationBuilder.AddColumn<int>(
                name: "FoodItemId",
                table: "MealLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Grams",
                table: "MealLogs",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateIndex(
                name: "IX_MealLogs_FoodItemId",
                table: "MealLogs",
                column: "FoodItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealLogs_FoodItems_FoodItemId",
                table: "MealLogs",
                column: "FoodItemId",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealLogs_FoodItems_FoodItemId",
                table: "MealLogs");

            migrationBuilder.DropIndex(
                name: "IX_MealLogs_FoodItemId",
                table: "MealLogs");

            migrationBuilder.DropColumn(
                name: "FoodItemId",
                table: "MealLogs");

            migrationBuilder.DropColumn(
                name: "Grams",
                table: "MealLogs");

            migrationBuilder.RenameColumn(
                name: "MealType",
                table: "MealLogs",
                newName: "Type");

            migrationBuilder.CreateTable(
                name: "MealLogItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodItemId = table.Column<int>(type: "int", nullable: false),
                    MealLogId = table.Column<int>(type: "int", nullable: false),
                    Grams = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealLogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealLogItems_FoodItems_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "FoodItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealLogItems_MealLogs_MealLogId",
                        column: x => x.MealLogId,
                        principalTable: "MealLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealLogItems_FoodItemId",
                table: "MealLogItems",
                column: "FoodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MealLogItems_MealLogId",
                table: "MealLogItems",
                column: "MealLogId");
        }
    }
}
