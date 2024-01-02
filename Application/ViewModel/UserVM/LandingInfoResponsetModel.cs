namespace Application.ViewModel.UserVM
{
    public class LandingInfoResponsetModel : ApiResponse
    {
        int UsersCount { get; set; }
        int GroupCount { get; set; }
        int ReminderCount { get; set; }
        int EventCount { get; set; }

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
