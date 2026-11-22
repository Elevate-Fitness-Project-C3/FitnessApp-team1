namespace Elevate.Contracts;
    public record WorkoutCompletedMessage
    {
        public Guid WorkoutSessionId { get; init; }
        public Guid UserId { get; init; }
        public Guid TrainingProgramId { get; init; }
        public Guid ProgramWorkoutSlotId { get; init; }
        public DateTime CompletedAtUtc { get; init; }
        public int Version { get; init; } = 1;
    }
