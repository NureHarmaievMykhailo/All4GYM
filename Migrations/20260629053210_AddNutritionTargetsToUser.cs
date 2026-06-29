using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace All4GYM.Migrations
{
    /// <inheritdoc />
    public partial class AddNutritionTargetsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetCalories",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "TargetCarbs",
                table: "Users",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TargetFats",
                table: "Users",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TargetProteins",
                table: "Users",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetCalories",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TargetCarbs",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TargetFats",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TargetProteins",
                table: "Users");
        }
    }
}
