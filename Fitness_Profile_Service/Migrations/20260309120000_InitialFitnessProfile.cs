using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessProfileService.Migrations;

public partial class InitialFitnessProfile : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "FitnessProfiles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TrainingExperienceLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                AvailableDaysPerWeek = table.Column<int>(type: "int", nullable: false),
                PhysicalActivityLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FitnessProfiles", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_FitnessProfiles_UserId",
            table: "FitnessProfiles",
            column: "UserId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "FitnessProfiles");
    }
}
