using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessProfileService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyMeasurement",
                columns: table => new
                {
                    MeasurementId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(5,1)", precision: 5, scale: 1, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,1)", precision: 5, scale: 1, nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyMeasurement", x => x.MeasurementId);
                });

            migrationBuilder.CreateTable(
                name: "FitnessProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    PrimaryFitnessGoal = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PhysicalActivityLevel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TrainingExperienceLevel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AvailableDaysPerWeek = table.Column<byte>(type: "tinyint", nullable: true),
                    IsOnboardingCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FitnessProfile", x => x.Id);
                    table.CheckConstraint("CK_FitnessProfile_AvailableDaysPerWeek", "AvailableDaysPerWeek IS NULL OR AvailableDaysPerWeek BETWEEN 1 AND 7");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyMeasurement_UserId_RecordedAt_MeasurementId",
                table: "BodyMeasurement",
                columns: new[] { "UserId", "CreatedAt", "MeasurementId" },
                descending: new[] { false, true, true });

            migrationBuilder.CreateIndex(
                name: "IX_FitnessProfile_UserId",
                table: "FitnessProfile",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyMeasurement");

            migrationBuilder.DropTable(
                name: "FitnessProfile");
        }
    }
}
