using Microsoft.IdentityModel.Tokens.Experimental;
using TrainingService.Features.TrainingPrograms.Common.GetProgramStructure.Dtos;

namespace TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Rules
{
    public static class ProgramPublishRules
    {
        public static List<string> Check(ProgramStructureDto program)
        {
            var problems = new List<string>();

            if (string.IsNullOrWhiteSpace(program.Title))
                problems.Add("Title is required.");
            if (program.DurationWeeks < 1)
                problems.Add("Duration must be at least 1 week.");
            if (program.DaysPerWeek is < 1 or > 7)
                problems.Add("DaysPerWeek must be between 1 and 7.");

            if (problems.Count > 0) return problems;    

            //programWorkoutSlots validation
            var expected = program.DurationWeeks * program.DaysPerWeek;
            if (program.Slots.Count != expected)
                problems.Add($"Expected {expected} slots but found {program.Slots.Count}.");

            // workout validation
            var slots = program.Slots.ToLookup(s => (s.WeekNumber, s.DayNumber));

            for (var week = 1; week <= program.DurationWeeks; week++)
            {
                for (var day = 1; day <= program.DaysPerWeek; day++)
                {
                    var slot = slots[(week, day)].FirstOrDefault();

                    if (slot is null)
                        problems.Add($"Week {week}, Day {day} is missing.");
                    else if (slot.ExercisesCount == 0)
                        problems.Add($"Week {week}, Day {day}: the workout is missing or has no exercises.");
                }
            }

            return problems;
        }
    }
}
