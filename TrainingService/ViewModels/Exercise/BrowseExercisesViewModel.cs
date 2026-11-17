namespace TrainingService.ViewModels.Exercise
{
    public class BrowseExercisesViewModel
    {
        public IEnumerable<ExerciseViewModel> Exercises { get; set; }
      = Enumerable.Empty<ExerciseViewModel>();

        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
