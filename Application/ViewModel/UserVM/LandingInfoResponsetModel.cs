namespace Application.ViewModel.UserVM
{
    public class LandingInfoResponsetModel : ApiResponse
    {
        public int UsersCount { get; set; }
        public int GroupCount { get; set; }
        public int ReminderCount { get; set; }
        public int EventCount { get; set; }

        public LandingInfoResponsetModel(int usersCount, int groupsCount, int reminderCount, int eventCount)
        {
            Success = true;
            Code = 200;
            Message = "Info Returned";
            UsersCount = usersCount;
            GroupCount = groupsCount;
            ReminderCount = reminderCount;
            EventCount = eventCount;
        }
    }
}
