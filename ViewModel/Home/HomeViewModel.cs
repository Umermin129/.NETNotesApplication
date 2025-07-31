using ViewModel.Notes;

namespace ViewModel.Home
{
    
    public class HomeViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<TaskEditViewModel> TaskLists { get; set; }
    }
}
