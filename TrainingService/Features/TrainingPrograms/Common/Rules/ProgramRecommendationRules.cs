using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos;

namespace TrainingService.Features.TrainingPrograms.Common.Rules
{
    // we used tuple to return both score and warnings, but we can also create a class to hold both values if needed
    public class ProgramRecommendationRules
    {
        public static (int Score, List<string> Warnings) Calculate(TrainingProgram program,UserProfileDto userProfile)
        {
            var warnings = new List<string>();
            var score = 0;

            if (program.Goal.ToString().Equals(userProfile.FitnessGoal, StringComparison.OrdinalIgnoreCase))
                score += 50;
            
            else
                warnings.Add($"Program goal is {program.Goal}, but your goal is {userProfile.FitnessGoal}.");
            
            if (program.RequiredLevel.ToString().Equals(userProfile.TrainingExperienceLevel, StringComparison.OrdinalIgnoreCase))
                score += 30;
            
            else if (program.RequiredLevel == ExperienceLevel.Beginner &&
                     userProfile.TrainingExperienceLevel?.Equals("Intermediate", StringComparison.OrdinalIgnoreCase) == true)
            {
                score += 15; 
                warnings.Add($"Program level is {program.RequiredLevel}, which is easier than your level {userProfile.TrainingExperienceLevel}.");
            }
            else if (program.RequiredLevel == ExperienceLevel.Advanced &&
                     userProfile.TrainingExperienceLevel?.Equals("Beginner", StringComparison.OrdinalIgnoreCase) == true)
           
                warnings.Add($"Program requires {program.RequiredLevel} level, but you are {userProfile.TrainingExperienceLevel}.");
           

            if (userProfile.AvailableDaysPerWeek.HasValue)
            {
                if (program.DaysPerWeek <= userProfile.AvailableDaysPerWeek.Value)
                    score += 20;
                
                else
                    warnings.Add($"Program requires {program.DaysPerWeek} days, but you only have {userProfile.AvailableDaysPerWeek} available.");
                
            }
            else
            
                warnings.Add("Your available days per week are not set.");
            

            return (score, warnings);
        }
    }
}
