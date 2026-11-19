using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingService.Migrations
{
    /// <inheritdoc />
    public partial class addIndex_OrderIndexWithWorkoutTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_WorkoutTemplateId",
                table: "WorkoutExercises");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutTemplateId_OrderIndex",
                table: "WorkoutExercises",
                columns: new[] { "WorkoutTemplateId", "OrderIndex" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_WorkoutTemplateId_OrderIndex",
                table: "WorkoutExercises");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutTemplateId",
                table: "WorkoutExercises",
                column: "WorkoutTemplateId");
        }
    }
}
