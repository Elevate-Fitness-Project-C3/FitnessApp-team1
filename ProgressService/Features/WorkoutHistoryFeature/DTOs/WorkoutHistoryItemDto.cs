namespace ProgressService.Features.WorkoutHistoryFeature.DTOs
{
    public class WorkoutHistoryItemDto
    {
        public Guid Id { get; set; }
        public Guid WorkoutSessionId { get; set; }
        public Guid TrainingProgramId { get; set; }
        public Guid ProgramWorkoutSlotId { get; set; }
        public DateTime CompletedAtUtc { get; set; }
    }
}
