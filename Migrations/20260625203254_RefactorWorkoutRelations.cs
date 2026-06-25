using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace All4GYM.Migrations
{
    /// <inheritdoc />
    public partial class RefactorWorkoutRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingPrograms_Users_UserId",
                table: "TrainingPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_TrainingPrograms_TrainingProgramId",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_TrainingPrograms_UserId",
                table: "TrainingPrograms");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TrainingPrograms");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Workouts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "TrainingPrograms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_UserId",
                table: "Workouts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_TrainingPrograms_TrainingProgramId",
                table: "Workouts",
                column: "TrainingProgramId",
                principalTable: "TrainingPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_Users_UserId",
                table: "Workouts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_TrainingPrograms_TrainingProgramId",
                table: "Workouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_Users_UserId",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_UserId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "TrainingPrograms");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TrainingPrograms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPrograms_UserId",
                table: "TrainingPrograms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingPrograms_Users_UserId",
                table: "TrainingPrograms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_TrainingPrograms_TrainingProgramId",
                table: "Workouts",
                column: "TrainingProgramId",
                principalTable: "TrainingPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
