namespace Application.ViewModel.UserVM
{
    public class ProfileInfoResponseModel : ApiResponse
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }

        public ProfileInfoResponseModel(string FirstName, string LastName, string Username, string birthday, string email, string imagePath)
        {
            Success = true;
            Code = 200;
            Message = "Info Returned";
            this.FirstName = FirstName;
            this.LastName = LastName;
            UserName = Username;
            Birthday = birthday;
            Email = email;
            ImagePath = imagePath;
        }
    }
}
