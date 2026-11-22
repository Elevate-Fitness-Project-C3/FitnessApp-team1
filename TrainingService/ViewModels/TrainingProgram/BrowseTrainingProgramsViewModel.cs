namespace TrainingService.ViewModels.TrainingProgram
{
    public class BrowseTrainingProgramsViewModel
    {
        public IEnumerable<BrowseTrainingProgramViewModel> Items { get; set; }
        = Enumerable.Empty<BrowseTrainingProgramViewModel>();

        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
