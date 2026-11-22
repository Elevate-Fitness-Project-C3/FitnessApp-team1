
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingService.Migrations;

public partial class AddUniqueIndex_WorkoutTemplateId_OrderIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF OBJECT_ID(N'dbo.WorkoutExercises', N'U') IS NOT NULL
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE object_id = OBJECT_ID(N'dbo.WorkoutExercises')
                      AND name = N'IX_WorkoutExercises_WorkoutTemplateId'
                )
                BEGIN
                    DROP INDEX [IX_WorkoutExercises_WorkoutTemplateId]
                    ON [dbo].[WorkoutExercises];
                END

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE object_id = OBJECT_ID(N'dbo.WorkoutExercises')
                      AND name = N'IX_WorkoutExercises_WorkoutTemplateId_OrderIndex'
                )
                BEGIN
                    CREATE UNIQUE INDEX
                        [IX_WorkoutExercises_WorkoutTemplateId_OrderIndex]
                    ON [dbo].[WorkoutExercises]
                        ([WorkoutTemplateId], [OrderIndex])
                    WHERE [IsDeleted] = 0;
                END
            END
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF OBJECT_ID(N'dbo.WorkoutExercises', N'U') IS NOT NULL
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE object_id = OBJECT_ID(N'dbo.WorkoutExercises')
                      AND name = N'IX_WorkoutExercises_WorkoutTemplateId_OrderIndex'
                )
                BEGIN
                    DROP INDEX
                        [IX_WorkoutExercises_WorkoutTemplateId_OrderIndex]
                    ON [dbo].[WorkoutExercises];
                END

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE object_id = OBJECT_ID(N'dbo.WorkoutExercises')
                      AND name = N'IX_WorkoutExercises_WorkoutTemplateId'
                )
                BEGIN
                    CREATE INDEX
                        [IX_WorkoutExercises_WorkoutTemplateId]
                    ON [dbo].[WorkoutExercises] ([WorkoutTemplateId]);
                END
            END
            """);
    }
}
