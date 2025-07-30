using UseNotesApplication.ViewModels.Notes;

namespace UseNotesApplication.ViewModels.Home
{
    
    public class HomeViewModel
    {
        public string UserName { get; set; }
        public string email { get; set; }
        public string Name { get; set; }
        public List<TaskEditViewModel> TaskLists { get; set; }
    }
}
